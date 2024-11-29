import { useState } from 'react';
import CodeGenApi from './CodeGenApi';
import CodeGenCli from './CodeGenCli';

function App() {
  const [currentTab, setCurrentTab] = useState<'api' | 'cli'>('api');
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
      {currentTab === 'api' ? <CodeGenApi /> : <CodeGenCli />}
    </div>
  </div>
}

export default App;
