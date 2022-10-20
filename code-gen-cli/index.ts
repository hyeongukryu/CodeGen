import axios from 'axios';
import { existsSync } from 'fs';
import { mkdir, rm, writeFile } from 'fs/promises';
import path from 'path';

async function getCode(): Promise<string | null> {
    try {
        const res = await axios.get<string>('http://localhost:5000/code-gen-api?swr=true&split=true');
        const code = res.data;
        return code;
    } catch {
    }
    return null;
}

async function main() {
    if (!existsSync('src/api')) {
        console.error('src/api directory does not exist');
        process.exit(1);
    }

    const code = await getCode();
    if (code === null) {
        console.error('Failed to get code');
        process.exit(1);
    }
    console.log(code);

    await rm('src/api', { recursive: true });
    await mkdir('src/api');

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
        await writeFile(path.join('src/api', currentFileName), currentFileContent);
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
main();