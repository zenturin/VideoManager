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
    imgsrc:string | undefined,
    onClick: (name:string,path:string) => void
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
    const [previousFolderStack, setPreviousFolderStack] = useState<TreeFolder[]>([])

    async function getTree(){
        try {
            const serverResponse : Response = await fetch("http://localhost:5271/repository/tree",{
                method: "GET"
            })
            const data = await serverResponse.json()
            setTree(TreeFolder.fromJson(data))
            setPreviousFolderStack([TreeFolder.fromJson(data)])
        } catch (error){
            console.log("Error: ", error)
            return (<p>error</p>)
        }
    }

    function findFolder (startFolder:TreeFolder, path : string) : TreeFolder | null {
        let folder : TreeFolder | null = null
        if (startFolder.path == path) return startFolder;
        return startFolder.Folders.find((f) => findFolder(f,path)) ?? null
    }

    function tileClickedCallback (name: string, path: string) {
        console.log("clicked : " + name )
        setPreviousFolderStack(previousFolderStack.concat([findFolder(tree,path) ?? tree]))
    }

    useEffect(() => {
        getTree();
    },[state])

    if (previousFolderStack.length == 0) {
        setPreviousFolderStack([tree])
        return
    }
    return (
    <div className="file-browser">
        <h3>File Browser</h3>
        <button onClick={e => {setPreviousFolderStack([tree])}}>Home</button>
        <button onClick={e => {setPreviousFolderStack(previousFolderStack.slice(0,-2))}}>Back</button>
        <p>Current Directory : {previousFolderStack[previousFolderStack.length-1].path}</p>
        {TileGrid(previousFolderStack[previousFolderStack.length-1],tileClickedCallback)}
    </div>
    )
}

function Tile ({name,path,imgsrc,onClick} : TileProps){
    if (imgsrc == undefined) imgsrc = loaderImg
    function tileSelected () {
        onClick(name,path)
    }
    return (
    <div key={path} className="tile" onClick={() => tileSelected()}>
        <img src={imgsrc}></img>
        <p>{name}</p>
    </div>
    )
}
function TileGrid (folder : TreeFolder, tileClicked : (name:string,path:string) => void){
    let tiles = []
    // Folders
    tiles = folder.Folders.map(
        (f : TreeFolder) => {
        return <Tile name={f.name} path={f.path} imgsrc={folderImg} onClick={tileClicked}></Tile>
    })
    tiles = tiles.concat(folder.Files.map(
        (f : TreeFile) => {
            return <Tile name={f.name} path={f.path} imgsrc={undefined} onClick={tileClicked}></Tile>
        }
    ))
    return <div className="tile-container">
        {tiles}
    </div>
}