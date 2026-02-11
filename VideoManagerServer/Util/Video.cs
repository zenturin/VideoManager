
using Xabe.FFmpeg;

namespace VideoManager
{
    public class Video
    {
        public string Path;
        protected MediaInfo? _Info;

        public Video(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("Could not find video at path: " + path);
            this.Path = path;
        }

        public async Task<MediaInfo> Info()
        {
            if (_Info == null) _Info = (MediaInfo)await FFmpeg.GetMediaInfo(this.Path);
            return _Info;
        }

        public async Task<decimal> Space()
        {
            var I = await Info();
            return I.Size;
        }

        public async Task<TimeSpan> Duration()
        {
            var I = await Info();
            return I.Duration;
        } 
    }
}
