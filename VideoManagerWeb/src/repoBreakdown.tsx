export default RepoBreakdown;

type Props = {
    repoInfo: Record<string,unknown> | null;
}

function RepoBreakdown ({repoInfo} : Props) {
    if (repoInfo == null) return <p>No repo selected</p>

    const info = Object.entries(repoInfo).map(([key,value]) => {
        if (typeof value == "string") {
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