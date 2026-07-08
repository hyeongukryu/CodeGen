// biome-ignore-all format: auto-generated
// biome-ignore-all lint: auto-generated
// biome-ignore-all assist/source/organizeImports: auto-generated
// auto-generated
/* eslint-disable */
'use client';

import _dayjs, { type Dayjs as _Dayjs } from 'dayjs';

import { _createHttp, _createObject, _restoreCircularReferences } from './_util';
import type { AxiosRequestConfig as _AxiosRequestConfig } from 'axios';
import _useSWR, { type SWRConfiguration as _SWRConfiguration } from 'swr';
import { _createSWRMiddleware } from './_util';
import type {
    Department,
    EchoResponse,
    EchoRequest,
} from './_types';
import {
    _convert_string_TO_string,
    _convert__api_Department_TO_Department_Array,
    _convert__api_EchoResponse_TO_EchoResponse,
    _convert_EchoRequest_TO__api_EchoRequest,
} from './_converters';
import {
    _Organizations_GET_ReturnsEmpty_url,
    _Organizations_GET_GetAll_url,
    _Organizations_POST_Echo_url,
    _Organizations_GET_TagA_url,
    _Organizations_GET_TagATagB_url,
} from './_url-builders';
export async function returnsEmpty(_axiosRequestConfig?: _AxiosRequestConfig): Promise<void> {
    await _createHttp().get(_Organizations_GET_ReturnsEmpty_url(), _axiosRequestConfig);
}
export async function getAll(_axiosRequestConfig?: _AxiosRequestConfig): Promise<Department[]> {
    const _response: any = await _createHttp().get(_Organizations_GET_GetAll_url(), _axiosRequestConfig);
    return _restoreCircularReferences(_convert__api_Department_TO_Department_Array(_response.data), _createObject);
}
export function useSWRGetAll(_config: _SWRConfiguration = {}, _shouldFetch: boolean = true) {
    return _useSWR<Department[]>(_shouldFetch ? _Organizations_GET_GetAll_url() : null, { ..._config, use: [_createSWRMiddleware(_convert__api_Department_TO_Department_Array)] });
}
export async function echo(request: EchoRequest, _axiosRequestConfig?: _AxiosRequestConfig): Promise<EchoResponse> {
    const _response: any = await _createHttp().post(_Organizations_POST_Echo_url(), _convert_EchoRequest_TO__api_EchoRequest(request), _axiosRequestConfig);
    return _restoreCircularReferences(_convert__api_EchoResponse_TO_EchoResponse(_response.data), _createObject);
}
export async function tagA(_axiosRequestConfig?: _AxiosRequestConfig): Promise<string> {
    const _response: any = await _createHttp().get(_Organizations_GET_TagA_url(), _axiosRequestConfig);
    return _restoreCircularReferences(_convert_string_TO_string(_response.data), _createObject);
}
export function useSWRTagA(_config: _SWRConfiguration = {}, _shouldFetch: boolean = true) {
    return _useSWR<string>(_shouldFetch ? _Organizations_GET_TagA_url() : null, { ..._config, use: [_createSWRMiddleware(_convert_string_TO_string)] });
}
export async function tagATagB(_axiosRequestConfig?: _AxiosRequestConfig): Promise<string> {
    const _response: any = await _createHttp().get(_Organizations_GET_TagATagB_url(), _axiosRequestConfig);
    return _restoreCircularReferences(_convert_string_TO_string(_response.data), _createObject);
}
export function useSWRTagATagB(_config: _SWRConfiguration = {}, _shouldFetch: boolean = true) {
    return _useSWR<string>(_shouldFetch ? _Organizations_GET_TagATagB_url() : null, { ..._config, use: [_createSWRMiddleware(_convert_string_TO_string)] });
}