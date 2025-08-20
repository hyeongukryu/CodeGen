export interface CodeGenConfig {
    tags: string[];
}

export interface TypeScriptApiResult {
    typeScriptApi: string;
    files: TypeScriptApiFile[];
    errorMessages: string[];
}

export interface TypeScriptApiFile {
    fileName: string;
    content: string;
}
