
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
        protected VideoRepository(string? path = null)
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
            decimal totalSpace = 0;
            foreach (Video video in this.Videos)
            {
                totalSpace += await video.Space();
                Console.WriteLine(await video.Space());
            }
            return totalSpace;
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
    }
}

