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
        if (_hasOwnPropertyRef(obj)) {
            const ref = obj.$ref;
            deferred.push(() => { parent[key] = cache.get(ref); });
            delete obj.$ref;
        } else if (_hasOwnPropertyValues(obj)) {
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

function _hasOwnPropertyRef(o: any): boolean {
    return o.hasOwnProperty('$ref');
}

function _hasOwnPropertyValues(o: any): boolean {
    return o.hasOwnProperty('$values');
}

///