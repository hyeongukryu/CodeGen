import { useEffect, useState } from 'react';
import CodeGenApi from './CodeGenApi';
import CodeGenCli from './CodeGenCli';
import { CodeGenConfig } from './types';

function App() {
  const [currentTab, setCurrentTab] = useState<'api' | 'cli'>('api');

  const [config, setConfig] = useState<CodeGenConfig | null>();
  useEffect(() => {
    (async () => {
      const res = await fetch('http://localhost:5000/code-gen-config');
      if (res.ok) {
        setConfig(await res.json());
      } else {
        setConfig(null);
      }
    })();
  }, []);

  if (config === null) {
    return <div className='p-4 text-red-500'>
      <p>CodeGenConfig를 불러오는 중 오류가 발생했습니다.</p>
    </div>;
  }

  if (config === undefined) {
    return <div className='p-4 text-gray-700'>
      <p>CodeGenConfig를 불러오는 중입니다.</p>
    </div>;
  }

  return <div className='p-4'>
    <div className='flex gap-2'>
      <button className="w-10 text-lg border-b-2 border-transparent aria-expanded:border-blue-500 aria-expanded:font-bold"
        aria-expanded={currentTab === 'api'}
        onClick={() => setCurrentTab('api')}>API</button>
      <button className="w-10 text-lg border-b-2 border-transparent aria-expanded:border-blue-500 aria-expanded:font-bold"
        aria-expanded={currentTab === 'cli'}
        onClick={() => setCurrentTab('cli')}>CLI</button>
    </div>
    <div className='mt-4'>
      {currentTab === 'api' ? <CodeGenApi config={config} /> : <CodeGenCli />}
    </div>
  </div>
}

export default App;
