import { useContext,useMemo,createContext, type ReactNode } from "react";

const ApiContext = createContext<Server | null>(null);

type providerProps = {
    children : ReactNode
}

export default function ApiProvider({ children } : providerProps) {
    const api = useMemo(() => new Server(), []);

    return (
        <ApiContext.Provider value={api}>
        {children}
        </ApiContext.Provider>
    );
}

export function useApi() {
    const context = useContext(ApiContext);

    if (!context) {
        throw new Error("useApi must be used within an ApiProvider");
    }

    return context;
}


export class Server {

    IP : string;
    PORT : number; 

    constructor (IP:string = "localhost",PORT:number = 5271) {
        this.IP = IP;
        this.PORT = PORT;
    }

    async getAllRepoSummary(repoId:number){
        let info = []
        try {
            const serverResponse : Response = await fetch(`http://${this.IP}:${this.PORT}/repos/${repoId}`,{
                method: "GET",
                headers: {
                    "Content-Type": "application/json"
                }
            })
            const data = serverResponse;
            info = await data.json()
            console.log("Success", data)
        } catch (error){
            console.log("Error: ", error)
        }
        return info
    }

    async getAllRepos() {
        const serverResponse : Response = await fetch(`http://${this.IP}:${this.PORT}/repos`,{
            method: "GET",
            headers: {
                "Conent-type": "application/json"
            }
        })
        const data = await serverResponse.json()
        return data
    }

}