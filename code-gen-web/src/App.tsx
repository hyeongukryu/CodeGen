import { _isComputed } from 'mobx/dist/internal';
import { useEffect, useState } from 'react';
import http from './http';
import { SWRConfig } from 'swr'
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
        {/* <SWRConfig value={{ fetcher }}>
          <Organizations />
          <WeatherForecast />
        </SWRConfig> */}
      </header>
    </div>
  )
}

export default App
