import { useEffect, useState } from "react"

type Props = {
    onSend: (value: Record<string,unknown>) => void;
}

export default function FolderSearch ({onSend} : Props) {
    const [response,setResponse] = useState("");

    
    async function handleSearch() {
        // Post request to tell server what directory to look at
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
        } catch (error){
            console.log("Error: ", error)
        }

        try {
            const serverResponse : Response = await fetch("http://localhost:5271/repository",{
                method: "GET"
            })
            const data = await serverResponse.json()
            console.log("DATA")
            console.log(data)
            onSend(data)
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