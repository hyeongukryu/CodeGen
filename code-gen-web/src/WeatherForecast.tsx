import { WeatherForecastController } from './apiold';

function WeatherForecastPage() {
  const { error, data } = WeatherForecastController.useGet(10, BigInt(12), { refreshInterval: 1000 });
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