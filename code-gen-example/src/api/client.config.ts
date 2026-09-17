import axios from 'axios';
import type { CodeGenConfig } from './client';

const config: CodeGenConfig = {
    createHttp: () => axios.create({ baseURL: 'http://localhost:5000' }),
};

export default config;
