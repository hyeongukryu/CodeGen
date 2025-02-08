import { existsSync } from 'fs';
import { mkdir, rm, writeFile } from 'fs/promises';
import path from 'path';

const ServerRoot = process.argv[2].replace(/\/$/, '');
if (!ServerRoot) {
    console.error('Usage: node update-api.js <server-root>');
    process.exit(1);
}
const Watch = process.env['CODEGEN_CLI_WATCH'] === 'Y';
const Interval = process.env['CODEGEN_CLI_WATCH_INTERVAL'] ?
    parseInt(process.env['CODEGEN_CLI_WATCH_INTERVAL']) : 1000;

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
        throw new Error('Failed to get code');
    }
    if (code.startsWith('ERROR\nERROR_BEGIN\n')) {
        const errorEnd = code.indexOf('\nERROR_END\n');
        const error = code.substring('ERROR\nERROR_BEGIN\n'.length, errorEnd);
        throw new Error(error);
    }
}

interface WriteFileRequest {
    fileName: string;
    content: string;
}

function splitCode(code: string): WriteFileRequest[] {
    const lines = code.split('\n');

    let currentFileName: string | null = null;
    let currentFileContent: string = '';

    const files: WriteFileRequest[] = [];

    function beginFile(name: string) {
        endFile();
        currentFileName = name;
    }

    function endFile() {
        if (currentFileName === null) {
            return;
        }
        files.push({ fileName: currentFileName, content: currentFileContent });
        currentFileName = null;
        currentFileContent = '';
    }

    for (const line of lines) {
        if (line.startsWith('// __CODEGEN_VERSION_2_FILE_BOUNDARY__ ')) {
            const fileName = line.split(' ')[2];
            beginFile(fileName);
            continue;
        }
        if (currentFileName === null) {
            throw new Error('Unexpected line outside of file boundary');
        }
        currentFileContent += line + '\n';
    }

    endFile();
    return files;
}

async function writeFiles(files: WriteFileRequest[], codePath: string): Promise<void> {
    await mkdir(codePath, { recursive: true });

    for (const file of files) {
        const filePath = path.join(codePath, file.fileName);
        await writeFile(filePath, file.content);
    }
}

async function main() {
    const ClientRoot = 'src/api/client';
    const ServerRoot = 'src/api/server';

    if (!existsSync(ClientRoot) || !existsSync(ServerRoot)) {
        console.error('src/api/client and src/api/server must exist');
        process.exit(1);
    }

    let prevClientCode: string | null = null;
    let prevServerCode: string | null = null;

    for (; ;) {
        try {
            const clientCode = await getCode(true, '../client.config');
            validateCode(clientCode);
            const serverCode = await getCode(false, '../server.config');
            validateCode(serverCode);

            if (prevClientCode !== clientCode || prevServerCode !== serverCode) {
                const clientFiles = splitCode(clientCode);
                const serverFiles = splitCode(serverCode);

                await rm('src/api/client', { recursive: true });
                await rm('src/api/server', { recursive: true });

                await writeFiles(clientFiles, ClientRoot);
                await writeFiles(serverFiles, ServerRoot);
            }

            if (!Watch) {
                break;
            }
        } catch (e) {
            console.error(e);
            if (!Watch) {
                process.exit(1);
            }
        }

        await new Promise(resolve => setTimeout(resolve, Interval));
    }
}
main();
