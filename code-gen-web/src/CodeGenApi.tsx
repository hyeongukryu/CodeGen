import { useState, useEffect } from 'react';
import { copyToClipboard } from './clipboard';

function CodeGenApi() {
  const [code, setCode] = useState<string>();
  const [swr, setSwr] = useState(true);
  const [split, setSplit] = useState(false);
  const [configFilePath, setConfigFilePath] = useState('./api.config');

  useEffect(() => {
    (async () => {
      const params = new URLSearchParams();
      params.append('swr', swr ? 'true' : 'false');
      params.append('split', split ? 'true' : 'false');
      params.append('configFilePath', configFilePath);

      const res = await fetch('code-gen-api?' + params.toString());
      setCode(await res.json());
    })();
  }, [swr, split, configFilePath]);

  return <div>
    <div>
      <div className='mb-2'>
        <input className='mr-2'
          id="swr" type="checkbox" checked={swr} onChange={(e) => setSwr(e.target.checked)} />
        <label htmlFor="swr">useSWR React Hook 사용</label>
        <p className='text-xs text-gray-600'>
          Node.js 프로그램처럼 SWR을 사용하기 곤란한 환경에서는 해제할 수 있습니다.
        </p>
      </div>

      <div className='mb-2'>
        <input className="mr-2"
          id="split" type="checkbox" checked={split} onChange={(e) => setSplit(e.target.checked)} />
        <label htmlFor="split">여러 파일로 분리</label>
        <p className='text-xs text-gray-600'>
          Tree shaking이 잘 되도록 여러 파일로 분리합니다. 새로운 파일이 시작하는 부분이 표시되어 있습니다.
        </p>
      </div>

      <div className='mb-2'>
        <label htmlFor="configFilePath" className='mr-2'>설정 파일 경로</label>
        <input className="p-1 text-sm border border-gray-300 rounded"
          id="configFilePath" type="input" value={configFilePath} onChange={(e) => setConfigFilePath(e.target.value)} />
        <p className='mt-1 text-xs text-gray-600'>
          CodeGenConfig를 내보내는 API 설정 파일 경로입니다.
        </p>
      </div>
    </div>
    <hr className='my-4' />
    {code === undefined ?
      <p className='text-gray-700'>
        Loading...
      </p> : <div>
        <button className="px-4 py-2 mb-4 font-bold text-white bg-blue-500 rounded hover:bg-blue-700"
          onClick={() => copyToClipboard(code)}>복사하기</button>
        <div className="p-2 text-sm leading-snug text-gray-700 whitespace-pre border border-gray-300 rounded">
          {code}
        </div>
      </div>}
  </div>;
}

export default CodeGenApi;
