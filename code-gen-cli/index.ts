import { existsSync } from 'fs';
import { mkdir, rm, writeFile } from 'fs/promises';
import path from 'path';

const ServerRoot = process.argv[2].replace(/\/$/, '');
if (!ServerRoot) {
    console.error('Usage: node update-api.js <server-root>');
    process.exit(1);
}

async function getCode(swr: boolean, configFilePath: string): Promise<string | null> {
    try {
        const params = new URLSearchParams();
        params.append('swr', swr ? 'true' : 'false');
        params.append('split', 'true');
        params.append('configFilePath', configFilePath);
        const res = await fetch(`${ServerRoot}/code-gen-api?${params.toString()}`);
        const code = await res.json();
        return code as string;
    } catch {
    }
    return null;
}

function validateCode(code: string | null): asserts code is string {
    if (code === null) {
        console.error('Failed to get code');
        process.exit(1);
    }
    if (code.startsWith('ERROR\nERROR_BEGIN\n')) {
        const errorEnd = code.indexOf('\nERROR_END\n');
        const error = code.substring('ERROR\nERROR_BEGIN\n'.length, errorEnd);
        console.error(error);
        process.exit(1);
    }
}

async function generateCode(code: string, codePath: string): Promise<void> {
    await mkdir(codePath, { recursive: true });

    const lines = code.split('\n');

    let currentFileName: string | null = null;
    let currentFileContent: string = '';

    async function beginFile(name: string) {
        await endFile();
        currentFileName = name;
    }

    async function endFile() {
        if (currentFileName === null) {
            return;
        }
        await writeFile(path.join(codePath, currentFileName), currentFileContent);
        currentFileName = null;
        currentFileContent = '';
    }

    for (const line of lines) {
        if (line.startsWith('// __CODEGEN_VERSION_2_FILE_BOUNDARY__ ')) {
            const fileName = line.split(' ')[2];
            await beginFile(fileName);
            continue;
        }
        if (currentFileName === null) {
            console.error('Unexpected line outside of file boundary');
            process.exit(1);
        }
        currentFileContent += line + '\n';
    }

    await endFile();
}

async function main() {
    if (!existsSync('src/api/client') || !existsSync('src/api/server')) {
        console.error('src/api/client and src/api/server must exist');
        process.exit(1);
    }

    const clientCode = await getCode(true, '../client.config');
    validateCode(clientCode);
    console.log(clientCode);

    const serverCode = await getCode(false, '../server.config');
    validateCode(serverCode);
    console.log(serverCode);

    await rm('src/api/client', { recursive: true });
    await rm('src/api/server', { recursive: true });
    await generateCode(clientCode, 'src/api/client');
    await generateCode(serverCode, 'src/api/server');
}
main();
