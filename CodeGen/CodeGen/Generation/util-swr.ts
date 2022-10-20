import _useSWR, { Middleware as _Middleware, SWRConfiguration, SWRHook as _SWRHook } from 'swr';

export function createSWRMiddleware(_convert: (from: any) => any): _Middleware {
    return (useSWRNext: _SWRHook) => (key, fetcher, config) => {
        if (fetcher === null) {
            return useSWRNext(key, fetcher, config);
        }
        const _fetchAndConvert = async (...args: any[]) => {
            const data: any = await Promise.resolve(fetcher(...args));
            return _restoreCircularReferences(_convert(data), _createObject);
        };
        return useSWRNext(key, _fetchAndConvert, config);
    };
}
