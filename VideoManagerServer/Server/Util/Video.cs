
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

        public async Task<MediaInfo?> Info()
        {
            try
            {
                if (_Info == null) _Info = (MediaInfo)await FFmpeg.GetMediaInfo(this.Path);
            } catch (System.ArgumentException)
            {
                Console.WriteLine("Couldn't load" + Path);
            }
            return _Info;
        }

        public async Task<decimal> Space()
        {
            var I = await Info();
            if (I == null) return 0;
            return I.Size;
        }

        public async Task<TimeSpan> Duration()
        {
            var I = await Info();
            if (I == null) return TimeSpan.MinValue;
            return I.Duration;
        } 
    }
}
