function _convert_string_TO_string(from: string): string {
    return from;
}

function _convert_string_TO_number(from: string): number {
    return Number(from);
}

function _convert_number_TO_string(from: number): string {
    return from.toString();
}

function _convert_string_TO_bigint(from: string): bigint {
    return BigInt(from);
}

function _convert_bigint_TO_string(from: bigint): string {
    return from.toString();
}

function _convert_boolean_TO_boolean(from: boolean): boolean {
    return from;
}

function _convert_Uint8Array_TO_string(from: Uint8Array): string {
    const globals = _getBase64Globals();
    if (globals.Buffer !== undefined) {
        return globals.Buffer.from(from).toString('base64');
    }

    if (globals.btoa !== undefined) {
        return globals.btoa(_bytesToBinaryString(from));
    }

    throw new Error('Base64 encoding is not available in this JavaScript runtime.');
}

function _convert_string_TO_Uint8Array(from: string): Uint8Array {
    const globals = _getBase64Globals();
    if (globals.Buffer !== undefined) {
        return new Uint8Array(globals.Buffer.from(from, 'base64'));
    }

    if (globals.atob === undefined) {
        throw new Error('Base64 decoding is not available in this JavaScript runtime.');
    }

    const binary = globals.atob(from);
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes;
}

function _getBase64Globals(): {
    atob?: (value: string) => string;
    btoa?: (value: string) => string;
    Buffer?: {
        from(value: Uint8Array): { toString(encoding: string): string };
        from(value: string, encoding: string): Uint8Array;
    };
} {
    return globalThis as unknown as {
        atob?: (value: string) => string;
        btoa?: (value: string) => string;
        Buffer?: {
            from(value: Uint8Array): { toString(encoding: string): string };
            from(value: string, encoding: string): Uint8Array;
        };
    };
}

function _bytesToBinaryString(bytes: Uint8Array): string {
    const chunkSize = 0x8000;
    let binary = '';
    for (let offset = 0; offset < bytes.length; offset += chunkSize) {
        let chunk = '';
        const end = Math.min(offset + chunkSize, bytes.length);
        for (let i = offset; i < end; i++) {
            chunk += String.fromCharCode(bytes[i]);
        }
        binary += chunk;
    }
    return binary;
}

function _convert_string_TO__Dayjs(from: string): _Dayjs {
    return _dayjs(from);
}

function _convert__Dayjs_TO_string(from: _Dayjs): string {
    return from.toISOString();
}
