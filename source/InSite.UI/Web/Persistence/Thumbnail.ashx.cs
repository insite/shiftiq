using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

using InSite.Common.Web;

using Shift.Common;

namespace InSite.Content.Handlers
{
    public class Thumbnail : IHttpHandler
    {
        public bool IsReusable => true;

        private static readonly string CacheFolder = ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Library", "Images", "Temp");

        private static DateTime _nextUpdate = DateTime.MinValue;
        private static readonly ConcurrentDictionary<string, object> _fileLocks = new ConcurrentDictionary<string, object>();

        static Thumbnail()
        {
            if (!Directory.Exists(CacheFolder))
                Directory.CreateDirectory(CacheFolder);
        }

        public void ProcessRequest(HttpContext context)
        {
            SetNextUpdateDatewTime();

            var fill = context.Request.QueryString["fill"] != "0";
            var imageUrl = SetImageURL(context);

            var imagePath = context.Server.MapPath(imageUrl);
            var fileInfo = new FileInfo(imagePath);

            if (!fileInfo.Exists
                || !int.TryParse(context.Request.QueryString["width"], out var width)
                || !int.TryParse(context.Request.QueryString["height"], out var height)
                )
            {
                HttpResponseHelper.SendHttp404(context.Response, null);
                return;
            }

            string cachePath = GetCachePath(fill, imageUrl, imagePath, width, height);

            PrepareResponse(context, fileInfo, cachePath);
        }

        private static string GetCachePath(bool fill, string imageUrl, string imagePath, int width, int height)
        {

            var cachePath = Path.Combine(CacheFolder, StringHelper.Sanitize(imageUrl, '-') + $".{width}x{height}");
            if (File.Exists(cachePath))
                return cachePath;

            var cacheLock = _fileLocks.GetOrAdd(cachePath, x => new object());

            lock (cacheLock)
            {
                if (!File.Exists(cachePath))
                    CreateThumbnail(fill, imagePath, width, height, cachePath);

                _fileLocks.TryRemove(cachePath, out _);
            }

            return cachePath;
        }

        private static void CreateThumbnail(bool fill, string imagePath, int width, int height, string cachePath)
        {
            using (var image = new Bitmap(imagePath))
            {
                var scale = Math.Min((double)width / image.Width, (double)height / image.Height);
                var scaleWidth = (int)(image.Width * scale);
                var scaleHeight = (int)(image.Height * scale);

                if (!fill)
                {
                    width = scaleWidth;
                    height = scaleHeight;
                }

                using (var thumbnail = new Bitmap(width, height))
                {
                    using (var g = Graphics.FromImage(thumbnail))
                    {
                        g.Clear(Color.Transparent);
                        g.DrawImage(image, (width - scaleWidth) / 2, (height - scaleHeight) / 2, scaleWidth, scaleHeight);
                    }

                    thumbnail.Save(cachePath, ImageFormat.Jpeg);
                }
            }
        }

        private static string SetImageURL(HttpContext context)
        {
            var imageUrl = context.Request.QueryString["imageUrl"];

            if (!string.IsNullOrEmpty(imageUrl))
                return imageUrl;

            var folder = context.Request.QueryString["folder"];
            var fileName = context.Request.QueryString["fileName"];
            imageUrl = "/Library/Images/Certificates/" + folder + "/" + fileName;

            return imageUrl;
        }

        private static void SetNextUpdateDatewTime()
        {
            if (DateTime.UtcNow <= _nextUpdate)
                return;

            lock (_fileLocks)
            {
                if (DateTime.UtcNow <= _nextUpdate)
                    return;

                var dir = new DirectoryInfo(CacheFolder);
                foreach (FileInfo file in dir.GetFiles())
                    file.Delete();

                _nextUpdate = DateTime.UtcNow.AddHours(1);
            }
        }

        private static void PrepareResponse(HttpContext context, FileInfo fileInfo, string cachePath)
        {
            var response = context.Response;

            response.Clear();
            response.ClearHeaders();
            response.ClearContent();

            response.AddHeader("Last-Modified", $"{fileInfo.LastWriteTime:ddd, dd MMM yyyy HH:mm:ss} GMT");
            response.AddHeader("Content-Disposition", $"attachment;filename={fileInfo.Name}");

            response.Cache.SetCacheability(HttpCacheability.Private);

            response.WriteFile(cachePath);
        }
    }
}