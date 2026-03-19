
using System.Diagnostics;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Xabe.FFmpeg;

namespace VideoManager
{
    public class Video
    {
        public string Path;
        protected MediaInfo? _Info;
        protected byte[]? thumbnail; 

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

        public async Task<byte[]> GetThumbnail()
        {
            if (thumbnail != null) return thumbnail;
            var args = $"-ss 15 -i \"{this.Path}\" -frames:v 1 -f image2pipe -vcodec png -";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            using var ms = new MemoryStream();
            await process.StandardOutput.BaseStream.CopyToAsync(ms);
            await process.WaitForExitAsync();
            using var image = Image.Load(ms.ToArray());
            image.Mutate(x => x.Resize(320,180));
            using var output = new MemoryStream();
            image.SaveAsJpeg(output);

            this.thumbnail = output.ToArray();

            return output.ToArray();
        }
    }
}
