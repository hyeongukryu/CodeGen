// auto-generated

import _httpInstance from './http';
import { AxiosRequestConfig as _AxiosRequestConfig, AxiosResponse as _AxiosResponse } from 'axios';
import _dayjs, { Dayjs as _Dayjs } from 'dayjs';
import _useSWR, { Middleware as _Middleware, SWRConfiguration, SWRHook as _SWRHook } from 'swr';

const _http: _Http = _httpInstance;
interface _Http {
    get<T = any, R = _AxiosResponse<T>>(url: string, config?: _AxiosRequestConfig | undefined): Promise<R>;
    delete<T = any, R = _AxiosResponse<T>>(url: string, config?: _AxiosRequestConfig | undefined): Promise<R>;
    head<T = any, R = _AxiosResponse<T>>(url: string, config?: _AxiosRequestConfig | undefined): Promise<R>;
    post<T = any, R = _AxiosResponse<T>>(url: string, data?: any, config?: _AxiosRequestConfig | undefined): Promise<R>;
    put<T = any, R = _AxiosResponse<T>>(url: string, data?: any, config?: _AxiosRequestConfig | undefined): Promise<R>;
    patch<T = any, R = _AxiosResponse<T>>(url: string, data?: any, config?: _AxiosRequestConfig | undefined): Promise<R>;
}

let _createObject: (obj: any) => any = (obj) => obj;
export function setCreateObject(createObject: (obj: any) => any) {
    _createObject = createObject;
}

export const restoreCircularReferences = _restoreCircularReferences;
function _restoreCircularReferences(obj: any, createObject: (obj: any) => any) {
    const root = createObject({ obj });
    const cache = new Map<string, any>();
    const deferred: (() => void)[] = [];
    const traverse = (parent: any, key: string) => {
        let obj = parent[key];
        if (obj === null || typeof obj !== 'object') {
            return;
        }
        if (obj.hasOwnProperty('$ref')) {
            const ref = obj.$ref;
            deferred.push(() => { parent[key] = cache.get(ref); });
            delete obj.$ref;
        } else if (obj.hasOwnProperty('$values')) {
            const values = obj.$values;
            delete obj.$values;
            cache.set(obj.$id, values);
            delete obj.$id;
            deferred.push(() => { parent[key] = values; });
            obj = values;
        } else if (obj.hasOwnProperty('$id')) {
            cache.set(obj.$id, obj);
            delete obj.$id;
        }
        for (const key in obj) {
            traverse(obj, key);
        }
    };
    traverse(root, 'obj');
    deferred.forEach(task => task());
    return root.obj;
}

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

function _convert_string_TO__Dayjs(from: string): _Dayjs {
    return _dayjs(from);
}

function _convert__Dayjs_TO_string(from: _Dayjs): string {
    return from.toISOString();
}

///

// API
export const Organizations = {
    async returnsEmpty(): Promise<void> {
        const _url: string = _Organizations_GET_ReturnsEmpty_url();
        const _response = await _http.get(_url);
    },
    async getAll(): Promise<Department[]> {
        const _url: string = _Organizations_GET_GetAll_url();
        const _response = await _http.get(_url);
        return _restoreCircularReferences(_convert__api_Department_TO_Department_Array(_response.data), _createObject);
    },
    useSWRGetAll(_config: SWRConfiguration = {}) {
        const _url: string = _Organizations_GET_GetAll_url();
        const _middleware: _Middleware = (useSWRNext: _SWRHook) => (key, fetcher, config) => {
            if (fetcher === null) {
                return useSWRNext(key, fetcher, config);
            }
            const _fetchAndConvert = async (...args: any[]) => {
                const data: any = await Promise.resolve(fetcher(...args));
        return _restoreCircularReferences(_convert__api_Department_TO_Department_Array(data), _createObject);
            };
            return useSWRNext(key, _fetchAndConvert, config);
        };
        return _useSWR<Department[]>(_url, { ..._config, use: [_middleware] });
    },
    async echo(request: EchoRequest): Promise<EchoResponse> {
        const _url: string = _Organizations_POST_Echo_url();
        const _payload: _api_EchoRequest = _convert_EchoRequest_TO__api_EchoRequest(request);
        const _response = await _http.post(_url, _payload);
        return _restoreCircularReferences(_convert__api_EchoResponse_TO_EchoResponse(_response.data), _createObject);
    },
}

export const WeatherForecast = {
    async get(count: number, temp: number, value: bigint): Promise<WeatherForecast[]> {
        const _url: string = _WeatherForecast_GET_Get_url(count, temp, value);
        const _response = await _http.get(_url);
        return _restoreCircularReferences(_convert__api_WeatherForecast_TO_WeatherForecast_Array(_response.data), _createObject);
    },
    useSWRGet(count: number, temp: number, value: bigint, _config: SWRConfiguration = {}) {
        const _url: string = _WeatherForecast_GET_Get_url(count, temp, value);
        const _middleware: _Middleware = (useSWRNext: _SWRHook) => (key, fetcher, config) => {
            if (fetcher === null) {
                return useSWRNext(key, fetcher, config);
            }
            const _fetchAndConvert = async (...args: any[]) => {
                const data: any = await Promise.resolve(fetcher(...args));
        return _restoreCircularReferences(_convert__api_WeatherForecast_TO_WeatherForecast_Array(data), _createObject);
            };
            return useSWRNext(key, _fetchAndConvert, config);
        };
        return _useSWR<WeatherForecast[]>(_url, { ..._config, use: [_middleware] });
    },
}

// Types
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

// Converters
function _convert_string_TO_string_Nullable(from: string | null): string | null {
    if (from === null) {
        return null;
    }
    return _convert_string_TO_string(from);
}

function _convert_Person_TO__api_Person(from: Person): _api_Person {
    if (from.hasOwnProperty('$ref')) {
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

function _convert_Person_TO__api_Person_Array(from: Person[]): _api_Person[] {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    if (from.hasOwnProperty('$values')) {
        from = (from as any).$values;
        const to: _api_Person[] = from.map(element => _convert_Person_TO__api_Person(element));
        return { ...from, $values: to } as any;
    }
    const to: _api_Person[] = from.map(element => _convert_Person_TO__api_Person(element));
    return to;
}

function _convert_Department_TO__api_Department(from: Department): _api_Department {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    const to: _api_Department = {
        id: _convert_number_TO_string(from.id),
        name: _convert_string_TO_string_Nullable(from.name),
        people: _convert_Person_TO__api_Person_Array(from.people),
    };
    return { ...from, ...to };
}

function _convert_Department_TO__api_Department_Array(from: Department[]): _api_Department[] {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    if (from.hasOwnProperty('$values')) {
        from = (from as any).$values;
        const to: _api_Department[] = from.map(element => _convert_Department_TO__api_Department(element));
        return { ...from, $values: to } as any;
    }
    const to: _api_Department[] = from.map(element => _convert_Department_TO__api_Department(element));
    return to;
}

function _convert__api_Person_TO_Person(from: _api_Person): Person {
    if (from.hasOwnProperty('$ref')) {
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

function _convert__api_Person_TO_Person_Array(from: _api_Person[]): Person[] {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    if (from.hasOwnProperty('$values')) {
        from = (from as any).$values;
        const to: Person[] = from.map(element => _convert__api_Person_TO_Person(element));
        return { ...from, $values: to } as any;
    }
    const to: Person[] = from.map(element => _convert__api_Person_TO_Person(element));
    return to;
}

function _convert__api_Department_TO_Department(from: _api_Department): Department {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    const to: Department = {
        id: _convert_string_TO_number(from.id),
        name: _convert_string_TO_string_Nullable(from.name),
        people: _convert__api_Person_TO_Person_Array(from.people),
    };
    return { ...from, ...to };
}

function _convert__api_Department_TO_Department_Array(from: _api_Department[]): Department[] {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    if (from.hasOwnProperty('$values')) {
        from = (from as any).$values;
        const to: Department[] = from.map(element => _convert__api_Department_TO_Department(element));
        return { ...from, $values: to } as any;
    }
    const to: Department[] = from.map(element => _convert__api_Department_TO_Department(element));
    return to;
}

function _convert_EchoResponse_TO__api_EchoResponse(from: EchoResponse): _api_EchoResponse {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    const to: _api_EchoResponse = {
        content: _convert_string_TO_string(from.content),
    };
    return { ...from, ...to };
}

function _convert__api_EchoResponse_TO_EchoResponse(from: _api_EchoResponse): EchoResponse {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    const to: EchoResponse = {
        content: _convert_string_TO_string(from.content),
    };
    return { ...from, ...to };
}

function _convert_EchoRequest_TO__api_EchoRequest(from: EchoRequest): _api_EchoRequest {
    if (from.hasOwnProperty('$ref')) {
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

function _convert__api_EchoRequest_TO_EchoRequest(from: _api_EchoRequest): EchoRequest {
    if (from.hasOwnProperty('$ref')) {
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

function _convert_WeatherForecast_TO__api_WeatherForecast(from: WeatherForecast): _api_WeatherForecast {
    if (from.hasOwnProperty('$ref')) {
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

function _convert_WeatherForecast_TO__api_WeatherForecast_Array(from: WeatherForecast[]): _api_WeatherForecast[] {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    if (from.hasOwnProperty('$values')) {
        from = (from as any).$values;
        const to: _api_WeatherForecast[] = from.map(element => _convert_WeatherForecast_TO__api_WeatherForecast(element));
        return { ...from, $values: to } as any;
    }
    const to: _api_WeatherForecast[] = from.map(element => _convert_WeatherForecast_TO__api_WeatherForecast(element));
    return to;
}

function _convert__api_WeatherForecast_TO_WeatherForecast(from: _api_WeatherForecast): WeatherForecast {
    if (from.hasOwnProperty('$ref')) {
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

function _convert__api_WeatherForecast_TO_WeatherForecast_Array(from: _api_WeatherForecast[]): WeatherForecast[] {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    if (from.hasOwnProperty('$values')) {
        from = (from as any).$values;
        const to: WeatherForecast[] = from.map(element => _convert__api_WeatherForecast_TO_WeatherForecast(element));
        return { ...from, $values: to } as any;
    }
    const to: WeatherForecast[] = from.map(element => _convert__api_WeatherForecast_TO_WeatherForecast(element));
    return to;
}

// URL builders
function _Organizations_GET_ReturnsEmpty_url(): string {
    return `organizations/empty`;
}

function _Organizations_GET_GetAll_url(): string {
    return `organizations`;
}

function _Organizations_POST_Echo_url(): string {
    return `organizations`;
}

function _WeatherForecast_GET_Get_url(count: number, temp: number, value: bigint): string {
    const _params = new URLSearchParams();
    if (value !== null) {
        _params.append('value', value.toString());
    }
    const _queryString = _params.toString();
    return `weather-forecast/${count.toString()}/${temp.toString()}`+ (_queryString.length ? '?' + _queryString : '');
}