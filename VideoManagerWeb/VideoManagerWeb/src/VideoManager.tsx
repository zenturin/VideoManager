import { useState } from 'react'
import FolderSearch from './FolderSearch'

export default VideoManager

function VideoManager() {
    const [repoState,setRepoState] = useState<object>()

    return (
        <div className='panel'>
            <div className='top-bar'>
                <h1>Video Manager</h1>
            </div>
            <div className='manager-interface'>
                <div>
                    browser
                </div>
                <div>
                    <FolderSearch onSend={setRepoState}></FolderSearch>
                    <h3>Report</h3>
                </div>
            </div>
        </div>
    )
}
