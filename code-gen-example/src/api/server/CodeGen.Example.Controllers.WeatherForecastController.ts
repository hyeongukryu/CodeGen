// biome-ignore-all format: auto-generated
// biome-ignore-all lint: auto-generated
// biome-ignore-all assist/source/organizeImports: auto-generated
// auto-generated
/* eslint-disable */

import _dayjs, { type Dayjs as _Dayjs } from 'dayjs';

import { _createHttp, _createObject, _restoreCircularReferences } from './_util';
import type { AxiosRequestConfig as _AxiosRequestConfig } from 'axios';
import type {
    WeatherForecast,
} from './_types';
import {
    _convert__api_WeatherForecast_TO_WeatherForecast_Array,
} from './_converters';
import {
    _WeatherForecast_GET_Get_url,
} from './_url-builders';
async function $get(count: number, temp: number, value: bigint, _axiosRequestConfig?: _AxiosRequestConfig): Promise<WeatherForecast[]> {
    const _response: any = await _createHttp().get(_WeatherForecast_GET_Get_url(count, temp, value), _axiosRequestConfig);
    return _restoreCircularReferences(_convert__api_WeatherForecast_TO_WeatherForecast_Array(_response.data), _createObject);
}
export { $get as get };
