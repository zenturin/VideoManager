
using System.Drawing;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.VisualBasic;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace VideoManager
{

    public class VideoRepository
    {
        public string? Path {
            get
            {
                return this._path;
            }
            set
            {
                this._path = value;
                this.Videos = [];
            }

        }
        private string? _path;
        public Video[] Videos;
        public VideoRepository(string? path = null)
        {
            _path = path;
            this.Videos = [];
            if (_path == null) return;
            Load();
        }

        public static async Task<VideoRepository> CreateAsync(string? path = null)
        {
            var repo = new VideoRepository(path);
            FileSystem.ChDir(AppContext.BaseDirectory);
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);
            FFmpeg.SetExecutablesPath(AppContext.BaseDirectory + "");
            return repo;
        }

        public void Load()
        {
            if (Videos.Length != 0) return;
            string[] folders = Directory.GetDirectories(this.Path);
            string[] files = [];
            foreach (string folder in folders)
            {
                files = files.Concat(Directory.GetFiles(folder)).ToArray(); 
            }
            foreach (string path in files)
            {
                Videos = Videos.Append(
                    new Video(path)
                ).ToArray();
            }

        }

        public async Task<int> GetTotalNumberOfVideos()
        {
            string[] folders = Directory.GetDirectories(this.Path);
            string[] files = [];
            foreach (string folder in folders)
            {
                files = files.Concat(Directory.GetFiles(folder)).ToArray(); 
            }
            return files.Length;
        }

        public async Task<decimal> GetTotalSizeOfRepository()
        {
            string[] files = [];
            files = Directory.GetFiles(this.Path);
            
            foreach (string dir in Directory.GetDirectories(this.Path))
            {
                files = files.Concat(Directory.GetFiles(dir)).ToArray();
            }
            return files.Sum(file =>
            {
                if ( !File.Exists(file) ) return 0;
                FileInfo fileInfo = new FileInfo(file);
                return fileInfo.Length;
            });
        }

        public async Task<RepoFolder> GetDirectoryTree(string path)
        {
            
            RepoFolder Tree = new RepoFolder(path);
            return Tree;
        }

        public async Task<Dictionary<string,string>> Summary()
        {
            Dictionary<string,string> summary = [];
            summary.Add(
                "Size",
                (await GetTotalSizeOfRepository()).ToString()
                );
            summary.Add(
                "Total Videos",
                (await GetTotalNumberOfVideos()).ToString()
                );
            summary.Add(
                "Path",
                this.Path ??= "None"
            );
            return summary;
        }

        public async Task<VideoInfo> GetVideoInfo(string path)
        {
            MediaInfo? rawInfo;
            Video? video;
            try
            {
                video = this.Videos.First((video) => video.Path == path);
                rawInfo = await video.Info();
            }catch (System.InvalidOperationException e)
            {
                return null;
            }

            VideoInfo info = new VideoInfo(
                path.Split("\\").Last(),
                rawInfo.Size.ToString(),
                rawInfo.Duration.ToString(),
                rawInfo.CreationTime.ToString()
            );
            return info;
        }

        public async Task<RepoGraphData> GetGraphData()
        {
            var semaphore = new SemaphoreSlim(4);
            var tasks = Videos.Select(async v => {
                await semaphore.WaitAsync();
                try { return await v.Info(); }
                finally { semaphore.Release(); }
            });
            var infos = (await Task.WhenAll(tasks)).Where(v => v != null).ToArray();
            

            // Duration buckets (in minutes)
            var durationBucketDefs = new (string Label, double MinSec, double MaxSec)[]
            {
                ("0-2 min",   0,    120),
                ("2-5 min",   120,  300),
                ("5-10 min",  300,  600),
                ("10-20 min", 600,  1200),
                ("20-30 min", 1200, 1800),
                ("30+ min",   1800, double.MaxValue),
            };
            var durationHistogram = durationBucketDefs.Select(b => new HistogramBucket(
                b.Label,
                infos.Count(i => i != null && i.Duration.TotalSeconds >= b.MinSec && i.Duration.TotalSeconds < b.MaxSec)
            )).ToArray();

            // Size buckets (in bytes)
            var sizeBucketDefs = new (string Label, long MinBytes, long MaxBytes)[]
            {
                ("0-100 MB",    0,                    100L * 1024 * 1024),
                ("100-500 MB",  100L * 1024 * 1024,   500L * 1024 * 1024),
                ("500MB-1 GB",  500L * 1024 * 1024,   1024L * 1024 * 1024),
                ("1-2 GB",      1024L * 1024 * 1024,  2048L * 1024 * 1024),
                ("2+ GB",       2048L * 1024 * 1024,  long.MaxValue),
            };
            var sizeHistogram = sizeBucketDefs.Select(b => new HistogramBucket(
                b.Label,
                infos.Count(i => i != null && i.Size >= b.MinBytes && i.Size < b.MaxBytes)
            )).ToArray();

            // Year buckets
            var yearHistogram = infos
                .Where(i => i != null && i.CreationTime.HasValue)
                .GroupBy(i => i!.CreationTime!.Value.Year)
                .OrderBy(g => g.Key)
                .Select(g => new HistogramBucket(g.Key.ToString(), g.Count()))
                .ToArray();

            return new RepoGraphData(durationHistogram, sizeHistogram, yearHistogram);
        }

        public async Task<Video> GetVideo(string path)
        {
            Video? video;
            try
            {
                video = this.Videos.First((video) => video.Path == path);
            }catch (System.InvalidOperationException e)
            {
                return null;
            }
            return video;
        }
    }

    public class VideoInfo
    {
        public string name {get;set;}
        public string size {get;set;}
        public string duration {get;set;}
        public string creationDate {get;set;}

        public VideoInfo (string name,string size,string duration,string creationDate)
        {
            this.name = name;
            this.size = size;
            this.duration = duration;
            this.creationDate = creationDate;
        }
    }

    public record HistogramBucket(string Bucket, int Count);
    public record RepoGraphData(
        HistogramBucket[] DurationHistogram,
        HistogramBucket[] SizeHistogram,
        HistogramBucket[] YearHistogram
    );
}

