// biome-ignore-all format: auto-generated
// biome-ignore-all lint: auto-generated
// biome-ignore-all assist/source/organizeImports: auto-generated
// auto-generated
/* eslint-disable */
'use client';

import _dayjs, { type Dayjs as _Dayjs } from 'dayjs';

import { _hasOwnPropertyRef, _hasOwnPropertyValues } from './_util';
import type {
    BuiltInsDto,
    _api_BuiltInsDto,
    Department,
    Person,
    _api_Department,
    _api_Person,
    EchoResponse,
    _api_EchoResponse,
    EchoRequest,
    _api_EchoRequest,
    WeatherForecast,
    _api_WeatherForecast,
} from './_types';
export function _convert_string_TO_string(from: string): string {
    return from;
}

export function _convert_string_TO_number(from: string): number {
    return Number(from);
}

export function _convert_number_TO_string(from: number): string {
    return from.toString();
}

export function _convert_string_TO_bigint(from: string): bigint {
    return BigInt(from);
}

export function _convert_bigint_TO_string(from: bigint): string {
    return from.toString();
}

export function _convert_boolean_TO_boolean(from: boolean): boolean {
    return from;
}

export function _convert_Uint8Array_TO_string(from: Uint8Array): string {
    const globals = _getBase64Globals();
    if (globals.Buffer !== undefined) {
        return globals.Buffer.from(from).toString('base64');
    }

    if (globals.btoa !== undefined) {
        return globals.btoa(_bytesToBinaryString(from));
    }

    throw new Error('Base64 encoding is not available in this JavaScript runtime.');
}

export function _convert_string_TO_Uint8Array(from: string): Uint8Array {
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

export function _getBase64Globals(): {
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

export function _bytesToBinaryString(bytes: Uint8Array): string {
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

export function _convert_string_TO__Dayjs(from: string): _Dayjs {
    return _dayjs(from);
}

export function _convert__Dayjs_TO_string(from: _Dayjs): string {
    return from.toISOString();
}

export function _convert_BuiltInsDto_TO__api_BuiltInsDto(from: BuiltInsDto): _api_BuiltInsDto {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: _api_BuiltInsDto = {
        stringValue: _convert_string_TO_string(from.stringValue),
        booleanValue: _convert_boolean_TO_boolean(from.booleanValue),
        byteValue: _convert_number_TO_string(from.byteValue),
        signedByteValue: _convert_number_TO_string(from.signedByteValue),
        int16Value: _convert_number_TO_string(from.int16Value),
        int32Value: _convert_number_TO_string(from.int32Value),
        int64Value: _convert_bigint_TO_string(from.int64Value),
        unsignedInt16Value: _convert_number_TO_string(from.unsignedInt16Value),
        unsignedInt32Value: _convert_number_TO_string(from.unsignedInt32Value),
        unsignedInt64Value: _convert_bigint_TO_string(from.unsignedInt64Value),
        singleValue: _convert_number_TO_string(from.singleValue),
        doubleValue: _convert_number_TO_string(from.doubleValue),
        decimalValue: _convert_number_TO_string(from.decimalValue),
        dateTimeValue: _convert_string_TO_string(from.dateTimeValue),
        dateTimeOffsetValue: _convert_string_TO_string(from.dateTimeOffsetValue),
        dateOnlyValue: _convert_string_TO_string(from.dateOnlyValue),
        timeOnlyValue: _convert_string_TO_string(from.timeOnlyValue),
        guidValue: _convert_string_TO_string(from.guidValue),
        uriValue: _convert_string_TO_string(from.uriValue),
        instantValue: _convert__Dayjs_TO_string(from.instantValue),
        localDateValue: _convert_string_TO_string(from.localDateValue),
        localTimeValue: _convert_string_TO_string(from.localTimeValue),
        localDateTimeValue: _convert_string_TO_string(from.localDateTimeValue),
        bytes: _convert_Uint8Array_TO_string(from.bytes),
    };
    return { ...from, ...to };
}

export function _convert__api_BuiltInsDto_TO_BuiltInsDto(from: _api_BuiltInsDto): BuiltInsDto {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: BuiltInsDto = {
        stringValue: _convert_string_TO_string(from.stringValue),
        booleanValue: _convert_boolean_TO_boolean(from.booleanValue),
        byteValue: _convert_string_TO_number(from.byteValue),
        signedByteValue: _convert_string_TO_number(from.signedByteValue),
        int16Value: _convert_string_TO_number(from.int16Value),
        int32Value: _convert_string_TO_number(from.int32Value),
        int64Value: _convert_string_TO_bigint(from.int64Value),
        unsignedInt16Value: _convert_string_TO_number(from.unsignedInt16Value),
        unsignedInt32Value: _convert_string_TO_number(from.unsignedInt32Value),
        unsignedInt64Value: _convert_string_TO_bigint(from.unsignedInt64Value),
        singleValue: _convert_string_TO_number(from.singleValue),
        doubleValue: _convert_string_TO_number(from.doubleValue),
        decimalValue: _convert_string_TO_number(from.decimalValue),
        dateTimeValue: _convert_string_TO_string(from.dateTimeValue),
        dateTimeOffsetValue: _convert_string_TO_string(from.dateTimeOffsetValue),
        dateOnlyValue: _convert_string_TO_string(from.dateOnlyValue),
        timeOnlyValue: _convert_string_TO_string(from.timeOnlyValue),
        guidValue: _convert_string_TO_string(from.guidValue),
        uriValue: _convert_string_TO_string(from.uriValue),
        instantValue: _convert_string_TO__Dayjs(from.instantValue),
        localDateValue: _convert_string_TO_string(from.localDateValue),
        localTimeValue: _convert_string_TO_string(from.localTimeValue),
        localDateTimeValue: _convert_string_TO_string(from.localDateTimeValue),
        bytes: _convert_string_TO_Uint8Array(from.bytes),
    };
    return { ...from, ...to };
}

export function _convert_string_TO_string_Nullable(from: string | null): string | null {
    if (from === null) {
        return null;
    }
    return _convert_string_TO_string(from);
}

export function _convert_Person_TO__api_Person(from: Person): _api_Person {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: _api_Person = {
        id: _convert_bigint_TO_string(from.id),
        name: _convert_string_TO_string(from.name),
        registered: _convert__Dayjs_TO_string(from.registered),
        department: _convert_Department_TO__api_Department(from.department),
    };
    return { ...from, ...to };
}

export function _convert_Person_TO__api_Person_Array(from: Person[]): _api_Person[] {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    if (_hasOwnPropertyValues(from)) {
        const values: Person[] = (from as any).$values;
        const to: _api_Person[] = values.map(element => _convert_Person_TO__api_Person(element));
        return { ...from, $values: to } as any;
    }
    const to: _api_Person[] = from.map(element => _convert_Person_TO__api_Person(element));
    return to;
}

export function _convert_Department_TO__api_Department(from: Department): _api_Department {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: _api_Department = {
        id: _convert_number_TO_string(from.id),
        name: _convert_string_TO_string_Nullable(from.name),
        people: _convert_Person_TO__api_Person_Array(from.people),
    };
    return { ...from, ...to };
}

export function _convert_Department_TO__api_Department_Array(from: Department[]): _api_Department[] {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    if (_hasOwnPropertyValues(from)) {
        const values: Department[] = (from as any).$values;
        const to: _api_Department[] = values.map(element => _convert_Department_TO__api_Department(element));
        return { ...from, $values: to } as any;
    }
    const to: _api_Department[] = from.map(element => _convert_Department_TO__api_Department(element));
    return to;
}

export function _convert__api_Person_TO_Person(from: _api_Person): Person {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: Person = {
        id: _convert_string_TO_bigint(from.id),
        name: _convert_string_TO_string(from.name),
        registered: _convert_string_TO__Dayjs(from.registered),
        department: _convert__api_Department_TO_Department(from.department),
    };
    return { ...from, ...to };
}

export function _convert__api_Person_TO_Person_Array(from: _api_Person[]): Person[] {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    if (_hasOwnPropertyValues(from)) {
        const values: _api_Person[] = (from as any).$values;
        const to: Person[] = values.map(element => _convert__api_Person_TO_Person(element));
        return { ...from, $values: to } as any;
    }
    const to: Person[] = from.map(element => _convert__api_Person_TO_Person(element));
    return to;
}

export function _convert__api_Department_TO_Department(from: _api_Department): Department {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: Department = {
        id: _convert_string_TO_number(from.id),
        name: _convert_string_TO_string_Nullable(from.name),
        people: _convert__api_Person_TO_Person_Array(from.people),
    };
    return { ...from, ...to };
}

export function _convert__api_Department_TO_Department_Array(from: _api_Department[]): Department[] {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    if (_hasOwnPropertyValues(from)) {
        const values: _api_Department[] = (from as any).$values;
        const to: Department[] = values.map(element => _convert__api_Department_TO_Department(element));
        return { ...from, $values: to } as any;
    }
    const to: Department[] = from.map(element => _convert__api_Department_TO_Department(element));
    return to;
}

export function _convert_EchoResponse_TO__api_EchoResponse(from: EchoResponse): _api_EchoResponse {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: _api_EchoResponse = {
        content: _convert_string_TO_string(from.content),
    };
    return { ...from, ...to };
}

export function _convert__api_EchoResponse_TO_EchoResponse(from: _api_EchoResponse): EchoResponse {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: EchoResponse = {
        content: _convert_string_TO_string(from.content),
    };
    return { ...from, ...to };
}

export function _convert_EchoRequest_TO__api_EchoRequest(from: EchoRequest): _api_EchoRequest {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: _api_EchoRequest = {
        a: _convert_bigint_TO_string(from.a),
        b: _convert_number_TO_string(from.b),
        c: _convert_string_TO_string(from.c),
        d: _convert__Dayjs_TO_string(from.d),
    };
    return { ...from, ...to };
}

export function _convert__api_EchoRequest_TO_EchoRequest(from: _api_EchoRequest): EchoRequest {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: EchoRequest = {
        a: _convert_string_TO_bigint(from.a),
        b: _convert_string_TO_number(from.b),
        c: _convert_string_TO_string(from.c),
        d: _convert_string_TO__Dayjs(from.d),
    };
    return { ...from, ...to };
}

export function _convert_WeatherForecast_TO__api_WeatherForecast(from: WeatherForecast): _api_WeatherForecast {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: _api_WeatherForecast = {
        date: _convert__Dayjs_TO_string(from.date),
        temperatureC: _convert_number_TO_string(from.temperatureC),
        temperatureF: _convert_number_TO_string(from.temperatureF),
        summary: _convert_string_TO_string_Nullable(from.summary),
        value: _convert_bigint_TO_string(from.value),
    };
    return { ...from, ...to };
}

export function _convert_WeatherForecast_TO__api_WeatherForecast_Array(from: WeatherForecast[]): _api_WeatherForecast[] {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    if (_hasOwnPropertyValues(from)) {
        const values: WeatherForecast[] = (from as any).$values;
        const to: _api_WeatherForecast[] = values.map(element => _convert_WeatherForecast_TO__api_WeatherForecast(element));
        return { ...from, $values: to } as any;
    }
    const to: _api_WeatherForecast[] = from.map(element => _convert_WeatherForecast_TO__api_WeatherForecast(element));
    return to;
}

export function _convert__api_WeatherForecast_TO_WeatherForecast(from: _api_WeatherForecast): WeatherForecast {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    const to: WeatherForecast = {
        date: _convert_string_TO__Dayjs(from.date),
        temperatureC: _convert_string_TO_number(from.temperatureC),
        temperatureF: _convert_string_TO_number(from.temperatureF),
        summary: _convert_string_TO_string_Nullable(from.summary),
        value: _convert_string_TO_bigint(from.value),
    };
    return { ...from, ...to };
}

export function _convert__api_WeatherForecast_TO_WeatherForecast_Array(from: _api_WeatherForecast[]): WeatherForecast[] {
    if (_hasOwnPropertyRef(from)) {
        return from as any;
    }
    if (_hasOwnPropertyValues(from)) {
        const values: _api_WeatherForecast[] = (from as any).$values;
        const to: WeatherForecast[] = values.map(element => _convert__api_WeatherForecast_TO_WeatherForecast(element));
        return { ...from, $values: to } as any;
    }
    const to: WeatherForecast[] = from.map(element => _convert__api_WeatherForecast_TO_WeatherForecast(element));
    return to;
}
