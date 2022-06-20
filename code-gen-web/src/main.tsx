import React from 'react'
import ReactDOM from 'react-dom/client'
import { setCreateHttp } from './api'
import App from './App'
import './index.css'
import axios from 'axios';

setCreateHttp(() => axios.create({ baseURL: 'http://localhost:5000/' }));

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
)
