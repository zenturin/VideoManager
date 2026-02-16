import { useEffect, useState } from "react"

type Props = {
    onSend: (value: object) => void;
}

export default function FolderSearch ({onSend} : Props) {
    const [response,setResponse] = useState("");

    
    async function handleSearch() {
        try {
            const serverResponse : Response = await fetch("http://localhost:5271/repository",{
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    path: response
                })
            })
            const data = serverResponse;
            console.log("Success", data)
            onSend(serverResponse)
        } catch (error){
            console.log("Error: ", error)
        }
    }
return(
    <>
        <div className='search'>
            <h3> Search </h3>
            <textarea value={response} onChange={e => {setResponse(e.target.value);}}></textarea>
            <button onClick={handleSearch}>Select Repository</button>
            <p className="search-status">{}</p>
        </div>
    </>
)
}