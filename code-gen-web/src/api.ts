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

function _convert_string_string(from: string): string {
    return from;
}

function _convert_string_number(from: string): number {
    return Number(from);
}

function _convert_string_bigint(from: string): bigint {
    return BigInt(from);
}

function _convert_boolean_boolean(from: boolean): boolean {
    return from;
}

function _convert_string__Dayjs(from: string): _Dayjs {
    return _dayjs(from);
}

export const Organizations = {
    async getAll(): Promise<Department[]> {
        const _url = _Organizations_GET_GetAll_url();
        const _response = await _http.get(_url);
        return _restoreCircularReferences(_convert__Department_Department_Array(_response.data), _createObject);
    },
    async echo(): Promise<EchoResponse> {
        const _url = _Organizations_POST_Echo_url();
        const _response = await _http.post(_url);
        return _restoreCircularReferences(_convert__EchoResponse_EchoResponse(_response.data), _createObject);
    },
}


export const WeatherForecast = {
}

export interface _Person {
        id: string;
        name: string;
        registered: string;
        department: _Department;
}


export interface _Department {
        id: string;
        name: string | null;
        people: _Person[];
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


export interface _EchoResponse {
        content: string;
}

function _convert_string_string_Nullable(from: string | null): string | null {
    if (from === null) {
        return null;
    }
    return _convert_string_string(from);
}

function _convert__Person_Person(from: _Person): Person {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    const to: Person = {
        id: _convert_string_bigint(from.id),
        name: _convert_string_string(from.name),
        registered: _convert_string__Dayjs(from.registered),
        department: _convert__Department_Department(from.department),
    };
    return { ...from, ...to };
}

function _convert__Person_Person_Array(from: _Person[]): Person[] {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    if (from.hasOwnProperty('$values')) {
        from = (from as any).$values;
        const to: Person[] = from.map(element => _convert__Person_Person(element));
        return { ...from, $values: to } as any;
    }
    const to: Person[] = from.map(element => _convert__Person_Person(element));
    return to;
}

function _convert__Department_Department(from: _Department): Department {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    const to: Department = {
        id: _convert_string_number(from.id),
        name: _convert_string_string_Nullable(from.name),
        people: _convert__Person_Person_Array(from.people),
    };
    return { ...from, ...to };
}

function _convert__Department_Department_Array(from: _Department[]): Department[] {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    if (from.hasOwnProperty('$values')) {
        from = (from as any).$values;
        const to: Department[] = from.map(element => _convert__Department_Department(element));
        return { ...from, $values: to } as any;
    }
    const to: Department[] = from.map(element => _convert__Department_Department(element));
    return to;
}

function _convert__EchoResponse_EchoResponse(from: _EchoResponse): EchoResponse {
    if (from.hasOwnProperty('$ref')) {
        return from as any;
    }
    const to: EchoResponse = {
        content: _convert_string_string(from.content),
    };
    return { ...from, ...to };
}
