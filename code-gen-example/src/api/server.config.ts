import axios from 'axios';

export default {
    createHttp: () => {
        const http = axios.create({
            baseURL: 'http://localhost:5000',
        });

        http.interceptors.request.use(
            request => {
                console.log('Request', {
                    method: request.method,
                    url: request.url,
                    data: request.data,
                });
                return request;
            },
            error => {
                console.error('Request error', error);
                return Promise.reject(error);
            });

        http.interceptors.response.use(
            response => {
                console.log('Response', {
                    status: response.status,
                    url: response.config.url,
                    data: response.data,
                });
                return response;
            },
            error => {
                console.error('Response error', error);
                return Promise.reject(error);
            });

        return http;
    },
};
