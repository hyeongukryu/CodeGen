import { useState, useEffect } from 'react';
import { copyToClipboard } from './clipboard';

function CodeGen() {
  const [code, setCode] = useState<string>();
  const [swr, setSwr] = useState(true);
  const [split, setSplit] = useState(false);
  const [configFilePath, setConfigFilePath] = useState('./api.config');

  useEffect(() => {
    const params = new URLSearchParams();
    params.append('swr', swr ? 'true' : 'false');
    params.append('split', split ? 'true' : 'false');
    params.append('configFilePath', configFilePath);
    fetch('code-gen-api?' + params.toString())
      .then(res => res.json())
      .then(json => setCode(json));
  }, [swr, split, configFilePath]);

  return <div>
    <div>
      <input id="swr" type="checkbox" checked={swr} onChange={(e) => setSwr(e.target.checked)} />
      <label htmlFor="swr">useSWR React Hook 사용</label>
      <p style={{ fontSize: '12px', marginTop: '4px' }}>Node.js 프로그램처럼 SWR을 사용하기 곤란한 환경에서는 해제할 수 있습니다.</p>

      <input id="split" type="checkbox" checked={split} onChange={(e) => setSplit(e.target.checked)} />
      <label htmlFor="split">여러 파일로 분리</label>
      <p style={{ fontSize: '12px', marginTop: '4px' }}>Tree shaking이 잘 되도록 여러 파일로 분리합니다. 새로운 파일이 시작하는 부분이 표시되어 있습니다.</p>

      <label htmlFor="configFilePath" style={{ marginRight: '4px' }}>설정 파일 경로</label>
      <input id="configFilePath" type="input" value={configFilePath} onChange={(e) => setConfigFilePath(e.target.value)} />
      <p style={{ fontSize: '12px', marginTop: '4px' }}>CodeGenConfig를 내보내는 API 설정 파일 경로입니다.</p>

      <button onClick={() => copyToClipboard(code)}>복사하기</button>
    </div>
    <hr />
    {code === undefined ? <p>Loading...</p> : <code>{code}</code>}
  </div>
}

export default CodeGen;
