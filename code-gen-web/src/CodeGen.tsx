import { useState, useEffect } from 'react';
import { copyToClipboard } from './clipboard';

function CodeGen() {
  const [code, setCode] = useState<string>();
  const [swr, setSwr] = useState(true);

  useEffect(() => {
    fetch(`code-gen-api${!swr ? '' : '?swr=true'}`)
      .then(res => res.json())
      .then(json => setCode(json));
  }, [swr]);

  return <div>
    <div>
      <input id="swr" type="checkbox" checked={swr} onChange={(e) => setSwr(e.target.checked)} />
      <label htmlFor="swr">useSWR React Hook 사용</label>
      <p style={{ fontSize: '0.9em' }}>Node.js 프로그램처럼 SWR을 사용하기 곤란한 환경에서는 해제할 수 있습니다.</p>
      <button onClick={() => copyToClipboard(code)}>복사하기</button>
    </div>
    <hr />
    {code === undefined ? <p>Loading...</p> : <code>{code}</code>}
  </div>
}

export default CodeGen;
