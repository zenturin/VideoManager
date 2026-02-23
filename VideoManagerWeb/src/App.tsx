import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import VideoManager from './VideoManager'

function App() {

  return (
    <>
      <div className='manager-container'>
        <VideoManager></VideoManager>
      </div>
    </>
  )
}

export default App
