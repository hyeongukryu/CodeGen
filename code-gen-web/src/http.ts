import axios from 'axios';

const instance = axios.create({ baseURL: 'http://localhost:5068/' });
export default instance;
