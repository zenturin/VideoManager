using System.Runtime.ExceptionServices;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using VideoManager;

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
        Dictionary<string,string> summary;
        try
        {
            summary = await _state.Repo.Summary();
        }catch (System.ArgumentException e)
        {
            return "";
        }
        return JsonSerializer.Serialize(summary);
        // return (await _state.Repo.GetTotalNumberOfVideos()).ToString();
    }
    [HttpPost]
    public IActionResult Set([FromBody] PathRequest request)
    {
        Console.WriteLine(request.Path);
        _state.RepositoryPath = request.Path;
        try
        {
            _state.Repo.Load();
        }catch (System.ArgumentException e)
        {
            _state.RepositoryPath = null;
            return NoContent();
        }catch (System.IO.DirectoryNotFoundException e)
        {
            _state.RepositoryPath = null;
            return NoContent();
        }catch (System.IO.IOException e)
        {
            _state.RepositoryPath = null;
            return NoContent();
        }
        
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
        VideoInfo videoInfo;
        try
        {
            videoInfo = await _state.Repo.GetVideoInfo(filePath);
        }catch (System.ArgumentException e)
        {
            return "";
        }
        
        if (videoInfo == null) return "";
        return JsonSerializer.Serialize( videoInfo,new JsonSerializerOptions{WriteIndented = true});
    }

    [HttpGet("thumbnail/{*filePath}")]
    public async Task<FileContentResult> GetThumbnail(string filePath)
    {
        Console.WriteLine(filePath);
        if (_state.Repo == null || _state.Repo.Path == null) return null;
        Video video = await _state.Repo.GetVideo(filePath);
        byte[] img = await video.GetThumbnail();
        return File(img,"image/png");
    }
}


public class PathRequest
{
    public string Path {get;set;}
}