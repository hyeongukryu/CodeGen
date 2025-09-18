import { existsSync } from 'fs';
import { mkdir, opendir, readFile, rm, writeFile } from 'fs/promises';
import path from 'path';

const ServerRoot = process.argv[2]?.replace(/\/$/, '');
if (!ServerRoot) {
    console.error('Usage: node update-api.js <server-root>');
    console.error('Environment variables:');
    console.error('    CODEGEN_CLI_WATCH');
    console.error('        Set to "Y" to enable watch mode (default: off)');
    console.error('    CODEGEN_CLI_WATCH_INTERVAL');
    console.error('        Interval in milliseconds for watch mode (default: 1000)');
    process.exit(1);
}

const Watch = process.env['CODEGEN_CLI_WATCH'] === 'Y';
const Interval = process.env['CODEGEN_CLI_WATCH_INTERVAL'] ?
    parseInt(process.env['CODEGEN_CLI_WATCH_INTERVAL']) : 1000;

async function getCode(swr: boolean, configFilePath: string): Promise<string | null> {
    try {
        const params = new URLSearchParams();
        params.append('format', 'typescript-api');
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

        const operation = await (async () => {
            if (!existsSync(filePath)) {
                return { code: 'create', message: 'Created' } as const;
            }
            const content = await readFile(filePath, 'utf-8');
            if (content !== file.content) {
                return { code: 'update', message: 'Updated' } as const;
            }
            return { code: 'none', message: null } as const;
        })();

        if (operation.code === 'none') {
            continue;
        }
        await writeFile(filePath, file.content);
        writeLog(`${operation.message}: ${filePath}`);
    }
}

async function clean(dir: string, files: string[]): Promise<void> {
    if (!Watch) {
        await rm(dir, { recursive: true, force: false });
        writeLog(`Removed: ${dir}`);
        return;
    }

    const entries = await opendir(dir);
    for await (const entry of entries) {
        if (!entry.isFile()) {
            throw new Error(`Unexpected directory entry: ${entry.name}`);
        }
        if (!files.includes(entry.name)) {
            const filePath = path.join(dir, entry.name);
            await rm(filePath);
            writeLog(`Removed: ${filePath}`);
        }
    }
}

function getLocalTimestamp(): string {
    const now = new Date();

    const year = now.getFullYear().toString().padStart(4, '0');
    const month = (now.getMonth() + 1).toString().padStart(2, '0');
    const day = now.getDate().toString().padStart(2, '0');
    const hour = now.getHours().toString().padStart(2, '0');
    const minute = now.getMinutes().toString().padStart(2, '0');
    const second = now.getSeconds().toString().padStart(2, '0');

    return `${year}-${month}-${day} ${hour}:${minute}:${second}`;
}

function writeLog(message: string) {
    console.log(`[${getLocalTimestamp()}] ${message}`);
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
            const serverCode = await getCode(false, '../server.config');
            if (Watch && (clientCode === null || serverCode === null)) {
                writeLog('Failed to get code');
                continue;
            }

            validateCode(clientCode);
            validateCode(serverCode);

            if (prevClientCode !== clientCode || prevServerCode !== serverCode) {
                writeLog('Updating API code');

                const clientFiles = splitCode(clientCode);
                const serverFiles = splitCode(serverCode);

                await clean('src/api/client', clientFiles.map(f => f.fileName));
                await clean('src/api/server', serverFiles.map(f => f.fileName));

                await writeFiles(clientFiles, ClientRoot);
                await writeFiles(serverFiles, ServerRoot);

                prevClientCode = clientCode;
                prevServerCode = serverCode;

                writeLog('Updated API code');
            }

            if (!Watch) {
                break;
            }
        } catch (e) {
            console.error(e);
            if (!Watch) {
                process.exit(1);
            }
        } finally {
            if (Watch) {
                await new Promise(resolve => setTimeout(resolve, Interval));
            }
        }
    }
}
main();
