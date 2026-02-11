import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import FolderSearch from './FolderSearch'

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <div className='manager-container'>
        <h1> Video Manager </h1>
      <div className='panel'>
        <div className='top-bar'>
          <h1>Video Manager</h1>
        </div>
        <div className='manager-interface'>
          <div>
            browser
          </div>
            <div>
              <FolderSearch></FolderSearch>
              <h3>Report</h3>
            </div>
          </div>
        </div>
      </div>
    </>
  )
}

export default App
