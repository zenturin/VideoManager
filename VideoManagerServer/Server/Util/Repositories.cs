using System.Data;
using System.Net;
using System.Security.AccessControl;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using VideoManager;

public class Repositories
{
    public class State
    {
        public List<string> directories {get;set;}
        public State(List<string> directories)
        {
            this.directories = directories;
        }
    }
    public List<string> dirs {get; protected set;}
    public List<VideoRepository> repos;

    public Repositories()
    {
        dirs = [];
        LoadState();
        LoadRepos();
    }

    public void SaveState()
    {
        State state = new State(dirs);
        var json = JsonSerializer.Serialize(state);
        File.WriteAllText("state.json", json);
    }

    public void LoadState()
    {
        if (!File.Exists("state.json"))
        {
            File.Create("state.json");
            this.dirs = [];
            return;
        }
        string json = File.ReadAllText("state.json");
        State data = JsonSerializer.Deserialize<State>(json) ?? new State([]);
        this.dirs = data.directories;
    }

    private void LoadRepos()
    {
        this.repos = dirs.ConvertAll((dir) => { return new VideoRepository(dir); });
    }
}