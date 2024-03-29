import _axios, { AxiosRequestConfig as _AxiosRequestConfig, AxiosResponse as _AxiosResponse } from 'axios';

export let _createHttp: () => CodeGenHttp = () => _axios.create();
export function setCreateHttp(createHttp: () => CodeGenHttp) {
    _createHttp = createHttp;
}

export interface CodeGenHttp {
    get<T = any, R = _AxiosResponse<T>>(url: string, config?: _AxiosRequestConfig | undefined): Promise<R>;
    delete<T = any, R = _AxiosResponse<T>>(url: string, config?: _AxiosRequestConfig | undefined): Promise<R>;
    head<T = any, R = _AxiosResponse<T>>(url: string, config?: _AxiosRequestConfig | undefined): Promise<R>;
    post<T = any, R = _AxiosResponse<T>>(url: string, data?: any, config?: _AxiosRequestConfig | undefined): Promise<R>;
    put<T = any, R = _AxiosResponse<T>>(url: string, data?: any, config?: _AxiosRequestConfig | undefined): Promise<R>;
    patch<T = any, R = _AxiosResponse<T>>(url: string, data?: any, config?: _AxiosRequestConfig | undefined): Promise<R>;
}

export let _createObject: (obj: any) => any = (obj) => obj;
export function setCreateObject(createObject: (obj: any) => any) {
    _createObject = createObject;
}

export function restoreCircularReferences(obj: any, createObject: (obj: any) => any) {
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

export function _hasOwnPropertyRef(o: any): boolean {
    return o.hasOwnProperty('$ref');
}

export function _hasOwnPropertyValues(o: any): boolean {
    return o.hasOwnProperty('$values');
}
