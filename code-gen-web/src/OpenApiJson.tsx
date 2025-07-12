import { useState, useEffect } from 'react';
import { copyToClipboard } from './clipboard';
import { CodeGenConfig } from './types';
import { ServerRoot } from './config';

function CodeGenApi(props: { config: CodeGenConfig }) {
  const [code, setCode] = useState<string>();
  const [tag, setTag] = useState(0);

  useEffect(() => {
    (async () => {
      const params = new URLSearchParams();
      params.append('format', 'openapi-json');
      if (tag !== 0) {
        params.append('tag', props.config.tags[tag - 1]);
      }

      const res = await fetch(`${ServerRoot}code-gen-api?` + params.toString());
      setCode(await res.json());
    })();
  }, [tag]);

  return <div>
    <div>
      <div className='mb-2'>
        <label htmlFor="tag" className='mr-2'>태그</label>
        <select className='p-1 mb-1 text-sm border border-gray-300 rounded' id="tag"
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
