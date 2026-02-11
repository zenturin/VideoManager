import { useState } from "react"

export default function FolderSearch () {
    const [response,setResponse] = useState("");
    function handleSearch() {
        
    }
return(
    <>
        <div className='search'>
            <h3> Search </h3>
            <textarea></textarea>
            <button onClick={handleSearch}>Select Repository</button>
            <p className="search-status">{}</p>
        </div>
    </>
)
}