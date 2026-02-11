using VideoManager;

namespace Server.Tests;

public class VideoManagerTests
{
    [Fact]
    public async Task CreateAsyncReturnRepo()
    {
        var repo = await VideoRepository.CreateAsync();
        Assert.IsType<VideoRepository>(repo);
    }

    [Fact]
    public async Task PathCanBeSet()
    {
        var repo = await VideoRepository.CreateAsync("D:/Videos");
        Assert.Equal("D:/Videos",repo.Path);
    }
}
