export default RepoBreakdown;

type Props = {
    repoInfo: Record<string,unknown> | null;
}

function RepoBreakdown ({repoInfo} : Props) {
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
                    size = Math.round(size * 100) /100
                    return (<li key={key}>{key + " : " + size + units[count] }</li>)
                }
            }
            return (<li key={key}>{key + " : " + value}</li>)
        }
    }
        
    )
    console.log("Info")
    console.log(info)
    return (
        <ul>{info}</ul>
    )
}