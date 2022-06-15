import { Organizations } from './api';

function OrganizationsPage() {
  const { error, data } = Organizations.useSWRGetAll();
  if (error) {
    return <div>error</div>;
  }
  if (!data) {
    return <div>loading</div>;
  }
  return <div>
    {data.map(d => <div key={d.id}>
      부서 이름: {d.name ?? '미지정'}
      {d.people.map(p =>
        <p key={p.id.toString()}>{p.name} {p.registered.toISOString()} {p.department.name}</p>
      )}
    </div>)
    }
  </div >;
}

export default OrganizationsPage;
