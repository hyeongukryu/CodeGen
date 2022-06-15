import { _isComputed } from 'mobx/dist/internal';
import { useEffect, useState } from 'react';
import { SWRConfig } from 'swr'
import http from './http';
import Organizations from './Organizations'
import WeatherForecast from './WeatherForecast'

const fetcher = (url: string) => http.get(url).then(res => res.data);

function App() {
  const [code, setCode] = useState<string>();
  useEffect(() => {
    fetch('/code-gen-api').then(res => res.json()).then(json => setCode(json));
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        {code === undefined ? <p>Loading...</p> : <code>{code}</code>}
      </header>
    </div>
  )
}

export default App
