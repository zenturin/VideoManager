
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
                video = this.Videos.First((video) => video.Path.Replace("\\","/") == path);
                rawInfo = await video.Info();
            }catch (System.InvalidOperationException e)
            {
                return null;
            }

            VideoInfo info = new VideoInfo(
                path.Split("/").Last(),
                rawInfo.Size.ToString(),
                rawInfo.Duration.ToString(),
                rawInfo.CreationTime.ToString()
            );
            return info;
        }

        public async Task<Video> GetVideo(string path)
        {
            Video? video;
            try
            {
                video = this.Videos.First((video) => video.Path.Replace("\\","/") == path);
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
}

