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
    BuiltInsDto,
} from './_types';
import {
    _convert_BuiltInsDto_TO__api_BuiltInsDto,
    _convert__api_BuiltInsDto_TO_BuiltInsDto,
} from './_converters';
import {
    _BuiltIns_POST_Echo_url,
} from './_url-builders';
async function $echo(request: BuiltInsDto, _axiosRequestConfig?: _AxiosRequestConfig): Promise<BuiltInsDto> {
    const _response: any = await _createHttp().post(_BuiltIns_POST_Echo_url(), _convert_BuiltInsDto_TO__api_BuiltInsDto(request), _axiosRequestConfig);
    return _restoreCircularReferences(_convert__api_BuiltInsDto_TO_BuiltInsDto(_response.data), _createObject);
}
export { $echo as echo };
