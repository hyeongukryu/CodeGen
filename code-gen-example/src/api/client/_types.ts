// biome-ignore-all format: auto-generated
// biome-ignore-all lint: auto-generated
// biome-ignore-all assist/source/organizeImports: auto-generated
// auto-generated
/* eslint-disable */
'use client';

import _dayjs, { type Dayjs as _Dayjs } from 'dayjs';

export interface BuiltInsDto {
    stringValue: string;
    booleanValue: boolean;
    byteValue: number;
    signedByteValue: number;
    int16Value: number;
    int32Value: number;
    int64Value: bigint;
    unsignedInt16Value: number;
    unsignedInt32Value: number;
    unsignedInt64Value: bigint;
    singleValue: number;
    doubleValue: number;
    decimalValue: number;
    dateTimeValue: string;
    dateTimeOffsetValue: string;
    dateOnlyValue: string;
    timeOnlyValue: string;
    guidValue: string;
    uriValue: string;
    instantValue: _Dayjs;
    localDateValue: string;
    localTimeValue: string;
    localDateTimeValue: string;
    bytes: Uint8Array;
}

export interface _api_BuiltInsDto {
    stringValue: string;
    booleanValue: boolean;
    byteValue: string;
    signedByteValue: string;
    int16Value: string;
    int32Value: string;
    int64Value: string;
    unsignedInt16Value: string;
    unsignedInt32Value: string;
    unsignedInt64Value: string;
    singleValue: string;
    doubleValue: string;
    decimalValue: string;
    dateTimeValue: string;
    dateTimeOffsetValue: string;
    dateOnlyValue: string;
    timeOnlyValue: string;
    guidValue: string;
    uriValue: string;
    instantValue: string;
    localDateValue: string;
    localTimeValue: string;
    localDateTimeValue: string;
    bytes: string;
}

export interface _api_Person {
    id: string;
    name: string;
    registered: string;
    department: _api_Department;
}

export interface _api_Department {
    id: string;
    name: string | null;
    people: _api_Person[];
}

export interface Person {
    id: bigint;
    name: string;
    registered: _Dayjs;
    department: Department;
}

export interface Department {
    id: number;
    name: string | null;
    people: Person[];
}

export interface EchoResponse {
    content: string;
}

export interface _api_EchoResponse {
    content: string;
}

export interface EchoRequest {
    a: bigint;
    b: number;
    c: string;
    d: _Dayjs;
}

export interface _api_EchoRequest {
    a: string;
    b: string;
    c: string;
    d: string;
}

export interface WeatherForecast {
    date: _Dayjs;
    temperatureC: number;
    temperatureF: number;
    summary: string | null;
    value: bigint;
}

export interface _api_WeatherForecast {
    date: string;
    temperatureC: string;
    temperatureF: string;
    summary: string | null;
    value: string;
}