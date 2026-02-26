import { useState, type BlockquoteHTMLAttributes } from 'react'
import FolderSearch from './FolderSearch'
import RepoBreakdown from './repoBreakdown'
import FileBrowser from './FileBrowser'

export default VideoManager

function VideoManager() {
    const [repoState,setRepoState] = useState<Record<string,unknown> | null>(null)
    const [selectedVideo,setSelectedVideo] = useState<string | null>(null)
    let fileBrowser = null
    if (repoState != null) {
        fileBrowser = (<FileBrowser state={repoState} onVideoSelected={setSelectedVideo}></FileBrowser>)
    }
    return (
        <div className='panel'>
            <div className='top-bar'>
                <h1>Video Manager</h1>
            </div>
            <div className='manager-interface'>
                {fileBrowser}
                <div>
                    <FolderSearch onSend={setRepoState} ></FolderSearch>
                    <h3>Report</h3>
                    <RepoBreakdown repoInfo={repoState} currentVideo={selectedVideo}></RepoBreakdown>
                </div>
            </div>
        </div>
    )
}
