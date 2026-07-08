// biome-ignore-all format: auto-generated
// biome-ignore-all lint: auto-generated
// biome-ignore-all assist/source/organizeImports: auto-generated
// auto-generated
/* eslint-disable */
'use client';

import _dayjs, { type Dayjs as _Dayjs } from 'dayjs';

import {
    _convert_bigint_TO_string,
} from './_converters';
export function _BuiltIns_POST_Echo_url(): string {
    return `built-ins/echo`;
}

export function _Organizations_GET_ReturnsEmpty_url(): string {
    return `organizations/empty`;
}

export function _Organizations_GET_GetAll_url(): string {
    return `organizations`;
}

export function _Organizations_POST_Echo_url(): string {
    return `organizations`;
}

export function _Organizations_GET_TagA_url(): string {
    return `organizations/tag-a`;
}

export function _Organizations_GET_TagATagB_url(): string {
    return `organizations/tag-a-tag-b`;
}

export function _WeatherForecast_GET_Get_url(count: number, temp: number, value: bigint): string {
    const _params = new URLSearchParams();
    const _converted_value = _convert_bigint_TO_string(value);
    if (_converted_value !== null) {
        _params.append('value', _converted_value.toString());
    }
    const _queryString = _params.toString();
    return `weather-forecast/${count.toString()}/${temp.toString()}`+ (_queryString.length ? '?' + _queryString : '');
}
