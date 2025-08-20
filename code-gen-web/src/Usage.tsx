const snippet = `// Program.cs

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// ...

builder.Services.AddCodeGen(false); // preserveReferences를 사용하려면 true로 설정

// ...

app.MapCodeGen();`;

const supportedAttributes = [
  {
    label: 'Microsoft.AspNetCore.Http.TagsAttribute',
    href: 'https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.http.tagsattribute.tags',
  }
];

function Usage() {
  return <div>
    <h2 className="text-xl font-bold mt-4 mb-2">
      사용 설정
    </h2>
    <pre className="bg-gray-100 max-w-[900px] p-4 rounded font-mono text-sm whitespace-pre-wrap border border-gray-300">
      {snippet}
    </pre>
    <h2 className="text-xl font-bold mt-4 mb-2">
      지원하는 특성
    </h2>
    <ul className="list-disc list-inside space-y-1 font-mono">
      {supportedAttributes.map(({ label, href }) => <li key={label}>
        <a
          className="text-blue-700 underline decoration-blue-300 underline-offset-2 hover:text-blue-900"
          href={href}
          target="_blank"
          rel="noopener noreferrer nofollow"
        >
          {label}
        </a>
      </li>)}
    </ul>
  </div>;
}

export default Usage;
