import { useContext, useEffect, useState } from "react"
import { useApi } from "./ServerWrapper";
import type { JSX } from "react/jsx-runtime";

type props = {
    RepoSelected : (repo:number) => void;
}

export default function RepoSelector ({RepoSelected} : props){
    const api = useApi()
    const [repoList,setRepoList] = useState<string[]>([])

    useEffect(() => {
        const getRepos = async () => {
            setRepoList(await api.getAllRepos())
        }
        getRepos()
    },[])

    let tiles: JSX.Element[] = []
    repoList.forEach((repo) => {
        tiles.push(
            <>
                <button onClick={() => {RepoSelected(repoList.indexOf(repo))}}>{repo}</button>
            </>
        )
    })

    return (
        <>
            <h1>REPO SELECTOR</h1>
            {tiles}
        </>
    )
}