using System.Diagnostics;
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
            var selectedFile = new RepoFile(file);
            if (!selectedFile.IsVideo()) continue;
            this.Files.Add(selectedFile);
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

    public bool IsVideo()
    {
        return HasMoov();
    }

    protected bool HasMoov()
    {
        var head = ReadHeader(512*1024);
        var tail = ReadTail(512*1024);

        var headText = System.Text.Encoding.ASCII.GetString(head);
        var tailText = System.Text.Encoding.ASCII.GetString(tail);

        return headText.Contains("moov") || tailText.Contains("moov");
    }

    protected byte[] ReadHeader(int size = 4096)
    {
        using var fs = new FileStream(path,FileMode.Open,FileAccess.Read);

        byte[] buffer = new byte[size];
        int bytesRead = fs.Read(buffer,0,size);

        if (bytesRead < size)
        {
            Array.Resize(ref buffer, bytesRead);
        }

        return buffer;
    }

    protected byte[] ReadTail(int size = 4096)
    {
        using var fs = new FileStream(path, FileMode.Open,FileAccess.Read);

        if (fs.Length < size)
        {
            size = (int)fs.Length;
        }

        fs.Seek(-size, SeekOrigin.End);

        byte[] buffer = new byte[size];
        fs.Read(buffer,0,size);

        return buffer;
    }
}