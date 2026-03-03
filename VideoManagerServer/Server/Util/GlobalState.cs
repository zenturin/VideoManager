using VideoManager;

public class GlobalState
{
    public string? RepositoryPath
    {
        get
        {
            return Repo.Path;
        }
        set
        {
            this.Repo.Path = value;
        }
    }

    public VideoRepository? Repo;


}