using System.Runtime.ExceptionServices;
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
        if (_state.Repo == null) return "Select a repository first";
        return (await _state.Repo.GetTotalNumberOfVideos()).ToString();
    }

    [HttpGet("{id}")]
    public string GetById(int id)
    {
        return $"You sent {id}";
    }
    [HttpPost]
    public IActionResult Set([FromBody] PathRequest request)
    {
        Console.WriteLine(request.Path);
        _state.RepositoryPath = request.Path;
        return Ok();
    }
}


public class PathRequest
{
    public string Path {get;set;}
}