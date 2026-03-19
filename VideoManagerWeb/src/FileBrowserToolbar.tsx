import "./FileBrowserToolbar.css"
import ImgHome from "./assets/home.svg"
import ImgBack from "./assets/arrow-left.svg"

export default FileBrowserToolbar

type props = {
    onHomeClick : () => void
    onBackClick : () => void
    currentDir : string
}

function FileBrowserToolbar ({onHomeClick,onBackClick, currentDir} : props) {

    return (
    <div className="toolbar">
        <button onClick={e => {onHomeClick()}}>
            <img src={ImgHome} alt="Home Button" />
        </button>
        <button onClick={e => {onBackClick()}}>
            <img src={ImgBack} alt="Back Button" />
        </button>
        <p>{currentDir}</p>
    </div>
    )
}