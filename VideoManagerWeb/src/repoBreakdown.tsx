import { useEffect, useState } from "react";
import "./RepoBreakdown.css"

export default RepoBreakdown;

type Props = {
    repoInfo: Record<string,unknown> | null;
    currentVideo: string | null;
}

function RepoBreakdown ({repoInfo,currentVideo} : Props) {
    if (repoInfo == null) return <p>No repo selected</p>

    const info = Object.entries(repoInfo).map(([key,value]) => {
        if (typeof value == "string") {
            switch (key){
                case "Size": {
                    let size = Number(value)
                    const units = ["B","KB","MB","GB","TB"]
                    let count = 0;
                    while (size > 1000){
                        size = size / 1000
                        count +=1
                    }
                    size = Math.round(size * 100) / 100
                    return (
                        <tr>
                            <th>{key}</th>
                            <td>{size + units[count]}</td>
                        </tr>
                )
                }
            }
            return (
            <tr>
                <th>{key}</th>
                <td>{value}</td>
            </tr>
            )
        }
    }
        
    )
    console.log("Info")
    console.log(info)
    return (
    <div className="breakdown">
        <table>
            <tr>
                <th> Repo Breakdown </th>
                <td> ------ </td>
            </tr>
            {info}
            <VideoBreakdown selectedVideo={currentVideo}></VideoBreakdown>
        </table>
    </div>
        
    )
}

type VideoProps = {
    selectedVideo : string | null
}

function VideoBreakdown ({selectedVideo} : VideoProps) {
    console.log(selectedVideo)
    if (selectedVideo == null) return;
    const [videoInfo,setVideoInfo] = useState<Record<string,unknown> | null>( null )
    
    async function getVideoInfo(path: string){
        try {
            const serverResponse : Response = await fetch("http://localhost:5271/repository/video/" + path,{
                method: "GET"
            })
            const data = await serverResponse.json()
            setVideoInfo(data)
        } catch (error){
            console.log("Error: ", error)
            return (<p>error</p>)
        }
    }

    useEffect(()=>{
        getVideoInfo(selectedVideo)
    },[selectedVideo])

    if (videoInfo == null) return
    console.log("Video Info")
    console.log(videoInfo)

    const info = Object.entries(videoInfo).map(([key,value]) => {
        switch (key){
            case "size": {
                let size = Number(value)
                const units = ["B","KB","MB","GB","TB"]
                let count = 0;
                while (size > 1000){
                    size = size / 1000
                    count +=1
                }
                size = Math.round(size * 100) /100
                return (
                    <tr>
                        <th>{key}</th>
                        <td>{size + units[count]}</td>
                    </tr>
                )
            }
            case "duration": {
                return (
                    <tr>
                        <th>{key}</th>
                        <td>{String(value).split(".")[0]}</td>
                    </tr>
                )
            }
        }
        return (
            <tr>
                <th>{key}</th>
                <td>{String(value)}</td>
            </tr>
        )

    })

    return(
        <>
        <tr>
            <th> Video Info </th>
            <td> ----- </td>
        </tr>
        {info}
        </>

    )
    
    
}