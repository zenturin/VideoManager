import { useState, type BlockquoteHTMLAttributes } from 'react'
import FolderSearch from './FolderSearch'
import RepoBreakdown from './repoBreakdown'

export default VideoManager

function VideoManager() {
    const [repoState,setRepoState] = useState<Record<string,unknown> | null>(null)
    console.log("RepoState")
    console.log(repoState)

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
                    <RepoBreakdown repoInfo={repoState}></RepoBreakdown>
                </div>
            </div>
        </div>
    )
}
