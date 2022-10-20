import _useSWR, { SWRConfiguration as _SWRConfiguration, Middleware as _Middleware, SWRHook as _SWRHook } from 'swr';

export function _createSWRMiddleware(_convert: (from: any) => any): _Middleware {
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
