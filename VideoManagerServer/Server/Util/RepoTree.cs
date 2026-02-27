using System.Runtime.ExceptionServices;

public class RepoItem
{
    
}

public class RepoFolder : RepoItem
{
    public List<RepoFolder> Folders {get;set;}
    public List<RepoFile> Files {get;set;}
    public string name {get;set;}
    public string path {get;set;}
    public RepoFolder(string path)
    {
        this.path = path;
        this.name = path.Split(['/','\\']).Last();
        this.Folders = [];
        this.Files = [];
        getChildren();
    }
    protected void getChildren(string? path = null)
    {
        path ??= this.path;
        foreach (string dir in Directory.GetDirectories(path))
        {
            string name = dir.Split(['/','\\']).Last();
            this.Folders.Add(new RepoFolder(dir));
        }

        foreach (string file in Directory.GetFiles(path))
        {
            string name = file.Split(['/','\\']).Last();
            this.Files.Add(new RepoFile(file));
        }
    }
}

public class RepoFile : RepoItem
{
    public string path {get;set;}
    public string name {get;set;}
    public RepoFile(string path)
    {
        this.path = path;
        this.name = path.Split(['/','\\']).Last();
    }
}