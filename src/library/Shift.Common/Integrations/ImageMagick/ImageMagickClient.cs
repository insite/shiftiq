using System;
using System.IO;
using System.Net.Http;

using Newtonsoft.Json;

using Shift.Constant;

using Shift.Common;

namespace Shift.Common.Integration.ImageMagick
{
    public class ImageMagickClient
    {
        private readonly ApiSettings _settings;

        public ImageMagickClient(ApiSettings settings)
        {
            _settings = settings;
        }

        public ImageInfo GetImageInfo(Stream stream) =>
            GetImageInfo(ReadAllBytes(stream));

        public ImageInfo GetImageInfo(byte[] data)
        {
            if (data.IsEmpty())
                throw new ArgumentNullException(nameof(data));

            var content = new ByteArrayContent(data);
            using (var postResult = TaskRunner.RunSync(StaticHttpClient.Client.PostAsync, GetEndpointUrl(Endpoints.GetImageInfo), content))
            {
                postResult.EnsureSuccessStatusCode();

                var json = TaskRunner.RunSync(postResult.Content.ReadAsStringAsync);

                return JsonConvert.DeserializeObject<ImageInfo>(json);
            }
        }

        public (byte[] ImageData, string[] Messages) AdjustImage(Stream stream, AdjustImageSettings settings) => AdjustImage(ReadAllBytes(stream), settings);

        public (byte[] ImageData, string[] Messages) AdjustImage(byte[] data, AdjustImageSettings settings)
        {
            if (data.IsEmpty())
                throw new ArgumentNullException(nameof(data));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var content = new MultipartFormDataContent
            {
                { new ByteArrayContent(data), "Image", "file.bin" },
                { new StringContent(settings.Crop.ToString()), "Settings.Crop" },
                { new StringContent(settings.MaxHeight.ToString()), "Settings.MaxHeight" },
                { new StringContent(settings.MaxWidth.ToString()), "Settings.MaxWidth" },
                { new StringContent(settings.OutputType.GetName()), "Settings.OutputType" }
            };

            using (var postResult = TaskRunner.RunSync(StaticHttpClient.Client.PostAsync, GetEndpointUrl(Endpoints.AdjustImage), content))
            {
                postResult.EnsureSuccessStatusCode();
                
                byte[] imageData;
                string[] messages;

                using (var fileStream = TaskRunner.RunSync(postResult.Content.ReadAsStreamAsync))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        var id = reader.ReadString();
                        if (id != "MAGICK")
                            throw ApplicationError.Create("Invalid result ID");

                        var count = reader.ReadInt32();
                        messages = new string[count];
                        for (var i = 0; i < count; i++)
                            messages[i] = reader.ReadString();

                        var size = reader.ReadInt32();
                        imageData = reader.ReadBytes(size);
                    }
                }

                return (imageData, messages);
            }
        }

        private string GetEndpointUrl(string endpoint)
        {
            var baseUrl = _settings.BaseUrl;

            if (!baseUrl.EndsWith("/"))
                baseUrl += "/";

            return $"{baseUrl}{endpoint}";
        }

        private static byte[] ReadAllBytes(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                var position = stream.CanSeek ? stream.Position : -1;

                stream.CopyTo(ms);

                if (position >= 0)
                    stream.Seek(position, SeekOrigin.Begin);

                return ms.ToArray();
            }
        }
    }
}
