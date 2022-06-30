import http from './http';
import { SWRConfig } from 'swr'
import Organizations from './Organizations'
import WeatherForecast from './WeatherForecast'
import CodeGen from './CodeGen';

const fetcher = (url: string) => http.get(url).then(res => res.data);

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <CodeGen />
        {/* <SWRConfig value={{ fetcher }}>
          <Organizations />
          <WeatherForecast />
        </SWRConfig> */}
      </header>
    </div>
  )
}

export default App
