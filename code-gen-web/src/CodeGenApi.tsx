import { useState, useEffect } from 'react';
import { copyToClipboard } from './clipboard';
import { CodeGenConfig, TypeScriptApiFile, TypeScriptApiResult } from './types';
import { ServerRoot } from './config';

function CodeGenApi(props: { config: CodeGenConfig }) {
  const [result, setResult] = useState<TypeScriptApiResult>();
  const [swr, setSwr] = useState(true);
  const [split, setSplit] = useState(false);
  const [configFilePath, setConfigFilePath] = useState('./api.config');
  const [tag, setTag] = useState(0);

  useEffect(() => {
    (async () => {
      const params = new URLSearchParams();
      params.append('format', 'typescript-api');
      params.append('swr', swr ? 'true' : 'false');
      params.append('split', split ? 'true' : 'false');
      params.append('configFilePath', configFilePath);
      if (tag !== 0) {
        params.append('tag', props.config.tags[tag - 1]);
      }

      const res = await fetch(`${ServerRoot}code-gen-api?` + params.toString());
      setResult(await res.json());
    })();
  }, [swr, split, configFilePath, tag]);

  function renderSplitFiles(files: TypeScriptApiFile[]) {
    return <div className='space-y-4'>
      {files.map((file) => <div key={file.fileName} className='border border-gray-300 rounded'>
        <div className='flex items-center justify-between px-3 py-2 bg-gray-50 border-b border-gray-300'>
          <span className='font-mono text-sm'>{file.fileName}</span>
          <button className='px-3 py-1 text-sm font-bold text-white bg-blue-500 rounded hover:bg-blue-700'
            onClick={() => copyToClipboard(file.content)}>복사하기</button>
        </div>
        <div className='p-2 text-sm leading-snug text-gray-700 whitespace-pre overflow-x-auto'>
          {file.content}
        </div>
      </div>)}
    </div>;
  }

  return <div>
    <div>
      <div className='mb-2'>
        <label htmlFor="tag" className='mr-2'>태그</label>
        <select className='min-w-48 p-1 mb-1 text-sm border border-gray-300 rounded' id="tag"
          value={tag} onChange={(e) => setTag(parseInt(e.target.value))}>
          <option value="0">모든 태그</option>
          {props.config.tags.map((t, i) => (
            <option key={i} value={i + 1}>{t}</option>
          ))}
        </select>
        <p className='text-xs text-gray-600'>
          Microsoft.AspNetCore.Http.TagsAttribute로 태그 목록을 지정하고, 태그별로 조회할 수 있습니다.
        </p>
      </div>

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
          Tree shaking이 잘 되도록 여러 파일로 분리합니다.
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
    {result === undefined ?
      <p className='text-gray-700'>
        Loading...
      </p> : <div>
        {result.errorMessages.length > 0 && <div className='p-4 mb-4 text-red-500 bg-red-100 border border-red-300 rounded'>
          <p className='font-bold'>오류</p>
          <ul className='list-disc list-inside'>
            {result.errorMessages.map((msg, i) => <li key={i}>{msg}</li>)}
          </ul>
        </div>}
        {split ? renderSplitFiles(result.files) : <>
          <button className="px-4 py-2 mb-4 font-bold text-white bg-blue-500 rounded hover:bg-blue-700"
            onClick={() => copyToClipboard(result.typeScriptApi)}>복사하기</button>
          <div className="p-2 text-sm leading-snug text-gray-700 whitespace-pre border border-gray-300 rounded">
            {result.typeScriptApi}
          </div>
        </>}
      </div>}
  </div>;
}

export default CodeGenApi;
