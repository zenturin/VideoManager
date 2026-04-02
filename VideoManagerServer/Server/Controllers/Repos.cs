using System.Runtime.ExceptionServices;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using VideoManager;

[ApiController]
[Route("/[controller]")]
public class ReposController : ControllerBase
{
    private readonly Repositories Repos;

    public ReposController(Repositories state)
    {
        this.Repos = state;
    }

    [HttpGet]
    public async Task<string> GetRepos()
    {
        var dirs = Repos.repos.ConvertAll((x) => {return x.Path;});
        return JsonSerializer.Serialize(dirs);
    }

    [HttpGet("{repoIdString}")]
    public async Task<string> GetRepo(string repoIdString)
    {
        int repoId = -1;
        if (!int.TryParse(repoIdString,out repoId)) return "";

        var summary = await Repos.repos[repoId].Summary();
        return JsonSerializer.Serialize(summary);
    }

    [HttpGet("{repoIdString}/{fileId}/info")]
    public async Task<string> GetFileInfo(string repoIdString, string fileId)
    {
        int repoId = -1;
        if (!int.TryParse(repoIdString,out repoId)) return "";
        var repo = Repos.repos[repoId];
        System.Diagnostics.Debug.WriteLine("RepoID: " + repoId + " | " + "FileId: " + fileId);
        return JsonSerializer.Serialize(await repo.GetVideoInfo(fileId));
    }
    [HttpGet("{repoIdString}/thumbnail")]
    public async Task<FileContentResult> GetFilethumbnail(string repoIdString, [FromQuery] string fileId)
    {
        Console.WriteLine("getting thumbnail at Repo: " + repoIdString + "file : " + fileId);
        int repoId = -1;
        if (!int.TryParse(repoIdString,out repoId)); // ! This needs a resulting action
        var repo = Repos.repos[repoId];
        Video video = await repo.GetVideo(fileId);
        var img = await video.GetThumbnail();
        return File(img  ,"image/png");
    }
    [HttpGet("{repoIdString}/tree")]
    public async Task<string> GetTree(string repoIdString)
    {
        int repoId;
        if (!int.TryParse(repoIdString,out repoId))
        {
            repoId = 0;
        }
        VideoRepository repo = Repos.repos[repoId];
        RepoFolder tree = await repo.GetDirectoryTree(repo.Path);
        return JsonSerializer.Serialize(tree, new JsonSerializerOptions{WriteIndented = true});

    }
}