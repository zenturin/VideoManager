using System.Runtime.ExceptionServices;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

[ApiController]
[Route("/[controller]")]
public class RepositoryController : ControllerBase
{
    private readonly GlobalState _state;

    public RepositoryController(GlobalState state)
    {
        this._state = state;
    }

    [HttpGet]
    public async Task<string> Get()
    {
        if (_state.Repo == null || _state.Repo.Path == null) return "Select a repository first";
        return JsonSerializer.Serialize(await _state.Repo.Summary());
        // return (await _state.Repo.GetTotalNumberOfVideos()).ToString();
    }
    [HttpPost]
    public IActionResult Set([FromBody] PathRequest request)
    {
        Console.WriteLine(request.Path);
        _state.RepositoryPath = request.Path;
        _state.Repo.Load();
        return Ok();
    }
    [HttpGet("tree")]
    public async Task<string> GetTree()
    {
        if (_state.Repo == null || _state.Repo.Path == null) return "Select a repository first";
        RepoFolder tree = await _state.Repo.GetDirectoryTree(_state.Repo.Path);
        return JsonSerializer.Serialize(tree, new JsonSerializerOptions{WriteIndented = true});

    }

    [HttpGet("video/{*filePath}")]
    public async Task<string> GetVideoFile(string filePath)
    {
        Console.WriteLine(filePath);
        if (_state.Repo == null || _state.Repo.Path == null) return "";
        Dictionary<string,string>? videoInfo = await _state.Repo.GetVideoInfo(filePath);
        if (videoInfo == null) return "";
        return JsonSerializer.Serialize( await _state.Repo.GetVideoInfo(filePath),new JsonSerializerOptions{WriteIndented = true});
    }
    
}


public class PathRequest
{
    public string Path {get;set;}
}