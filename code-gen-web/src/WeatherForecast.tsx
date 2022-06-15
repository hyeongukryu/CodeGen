import { WeatherForecast } from './api';

function WeatherForecastPage() {
  const { error, data } = WeatherForecast.useSWRGet(10, 1234, BigInt(12), { refreshInterval: 1000 });
  if (error) {
    return <div>error</div>;
  }
  if (!data) {
    return <div>loading</div>;
  }
  return <div>
    {data.map(d => <div key={d.date.valueOf()}>
      {d.date.toISOString()} {d.temperatureC} {d.temperatureF} {d.summary} {d.value.toString()}
    </div>)}
  </div>;
}

export default WeatherForecastPage;