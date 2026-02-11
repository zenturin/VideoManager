using VideoManager;

public class GlobalState
{
    public string? RepositoryPath
    {
        get
        {
            if (Repo == null) return null;
            return Repo.Path;
        }
        set
        {
            if (value == null) return;
            this.Repo = new VideoRepository(value);
        }
    }

    public VideoRepository? Repo;


}