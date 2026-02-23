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
    const [currentFolder,setCurrentFolder] = useState<TreeFolder | null>(null)

    async function getTree(){
        try {
            const serverResponse : Response = await fetch("http://localhost:5271/repository/tree",{
                method: "GET"
            })
            const data = await serverResponse.json()
            setTree(TreeFolder.fromJson(data))
            setCurrentFolder(TreeFolder.fromJson(data))
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
        setCurrentFolder(findFolder(tree,path))
    }

    useEffect(() => {
        getTree();
    },[state])

    if (currentFolder == null) setCurrentFolder(tree);
    if (currentFolder == null) return

    return (
    <div className="file-browser">
        <h3>File Browser</h3>
        <button onClick={e => {setCurrentFolder(tree)}}>Home</button>
        <p>Current Directory : {currentFolder.path}</p>
        {TileGrid(currentFolder,tileClickedCallback)}
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