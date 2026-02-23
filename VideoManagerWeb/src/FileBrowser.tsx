import { useEffect, useState } from "react";
import './FileBrowser.css'
import loaderImg from "./assets/loader.svg"
import folderImg from "./assets/folder.svg"
export default FileBrowser

type Props = {
    state: Record<string,unknown>;
}
type item = {
    name: string,
    path: string
}
type TileProps = {
    name: string,
    path:string,
    imgsrc:string | undefined
}


class TreeFolder{
    Folders : Array<TreeFolder> = []
    Files : Array<TreeFile> = []
    name : string = ""
    path : string = ""
    constructor(name:string,path:string,folders:TreeFolder[],files:TreeFile[]){
        this.name = name;
        this.path = path;
        this.Folders = folders;
        this.Files = files;
    }

    static fromJson(obj : any) : TreeFolder {
        return new TreeFolder(
            obj.name,
            obj.path,
            (obj.Folders ?? []).map((folder: any) => TreeFolder.fromJson(folder)),
            (obj.Files ?? []).map((file: any) => new TreeFile(file.name,file.path))
        )
    }
}
class TreeFile{
    name: string = ""
    path: string = ""

    constructor(name:string,path:string){
        this.name = name;
        this.path = path
    }
}

function FileBrowser({state} : Props){
    const [tree,setTree] = useState<TreeFolder>(new TreeFolder("","",[],[]))
    const [currentDir,setCurrentDir] = useState<string>()

    async function getTree(){
        try {
            const serverResponse : Response = await fetch("http://localhost:5271/repository/tree",{
                method: "GET"
            })
            const data = await serverResponse.json()
            setTree(TreeFolder.fromJson(data))
            setCurrentDir(tree?.path)
        } catch (error){
            console.log("Error: ", error)
            return (<p>error</p>)
        }
    }

    useEffect(() => {
        getTree();
    },[state])

    return (
    <div className="file-browser">
        <h3>File Browser</h3>
        <p>Current Directory : {currentDir}</p>
        {TileGrid(tree)}
    </div>
    )
}

function Tile ({name,path,imgsrc} : TileProps){
    if (imgsrc == undefined) imgsrc = loaderImg

    return (
    <div key={path} className="tile">
        <img src={imgsrc}></img>
        <p>{name}</p>
    </div>
    )
}
function TileGrid (folder : TreeFolder){
    let tiles = []
    // Folders
    tiles = folder.Folders.map(
        (f : TreeFolder) => {
        return <Tile name={f.name} path={f.path} imgsrc={folderImg}></Tile>
    })
    tiles = tiles.concat(folder.Files.map(
        (f : TreeFile) => {
            return <Tile name={f.name} path={f.path} imgsrc={undefined}></Tile>
        }
    ))
    return <div className="tile-container">
        {tiles}
    </div>
}