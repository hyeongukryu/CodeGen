import { copyToClipboard } from './clipboard';
import CliScript from './bundle.js?raw';

function CodeGenCli() {
  const PackageJsonPrefix = `{\n  "scripts": {\n    `;
  const PackageJsonPostfix = `\n  }\n}`;
  const PackageJson = `"api": "node update-api.js ${window.location.origin}/"`;

  return <div>
    <div className="mb-8">
      <h2 className="mb-2 text-lg">
        <span className='font-bold'>1. </span>
        <span className='font-mono text-base'>package.json</span>
      </h2>
      <button className="px-4 py-2 mb-4 font-bold text-white bg-blue-500 rounded hover:bg-blue-700"
        onClick={() => copyToClipboard(PackageJson)}>복사하기</button>
      <div className="p-2 font-mono leading-snug text-gray-700 break-all whitespace-pre border border-gray-300 rounded">
        <span className='text-gray-400'>
          {PackageJsonPrefix}
        </span>
        <span className='font-bold text-blue-500'>
          {PackageJson}
        </span>
        <span className='text-gray-400'>
          {PackageJsonPostfix}
        </span>
      </div>
    </div>
    <div className="mb-8">
      <h2 className="mb-2 text-lg">
        <span className='font-bold'>2. </span>
        <span className='font-mono text-base'>update-api.js</span>
      </h2>
      <button className="px-4 py-2 mb-4 font-bold text-white bg-blue-500 rounded hover:bg-blue-700"
        onClick={() => copyToClipboard(CliScript)}>복사하기</button>
      <div className="p-2 text-[8pt] leading-snug text-gray-700 break-all border border-gray-300 rounded">
        {CliScript}
      </div>
    </div>
  </div>;
}

export default CodeGenCli;
