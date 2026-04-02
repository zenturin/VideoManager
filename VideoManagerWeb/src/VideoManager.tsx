import { useState, type BlockquoteHTMLAttributes } from 'react'
import FolderSearch from './FolderSearch'
import RepoBreakdown from './repoBreakdown'
import FileBrowser from './FileBrowser'
import RepoSelector from './RepoSelector'
import ApiProvider from './ServerWrapper'

export default VideoManager

function VideoManager() {
    const [selectedRepo, setSelectedRepo] = useState<number | null>(null)
    const [selectedVideo,setSelectedVideo] = useState<string | null>(null)
    let fileBrowser = null
    if (selectedRepo != null) {
        fileBrowser = (<FileBrowser repoId={selectedRepo} onVideoSelected={setSelectedVideo}></FileBrowser>)
    }
    return (
        <ApiProvider>
            <div className='panel'>
                <div className='top-bar'>
                    <h1>Video Manager</h1>
                </div>
                <div className='manager-interface'>
                    {fileBrowser}
                    <div>
                        <RepoSelector RepoSelected={setSelectedRepo}></RepoSelector>
                        <h3>Report</h3>
                        <RepoBreakdown repoId={selectedRepo} currentVideo={selectedVideo}></RepoBreakdown>
                    </div>
                </div>
            </div>
        </ApiProvider>
    )
}
