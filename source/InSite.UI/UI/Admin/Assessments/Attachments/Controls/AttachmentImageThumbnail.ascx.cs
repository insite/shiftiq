using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using InSite.Admin.Assessments.Attachments.Utilities;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Attachments.Controls
{
    public partial class AttachmentImageThumbnail : BaseUserControl
    {
        #region Constants

        private const int MaxThumbnailImageSize = 500;

        #endregion

        #region Classes

        public class ThumbnailInfo
        {
            public string FileName { get; internal set; }
            public ImageDimension ImageDimension { get; set; }

            public Func<string> GetImageUrl { get; set; }
            public Action<Action<Stream>> ReadFile { get; set; }
        }

        #endregion

        #region Data binding

        public void TryLoadData(ThumbnailInfo info)
        {
            if (!TryResizeImage(info, out var url))
                url = info.GetImageUrl();

            ThumbnailImage.Src = url;
        }

        private static bool TryResizeImage(ThumbnailInfo info, out string url)
        {
            url = null;

            if (info.ImageDimension.Height < MaxThumbnailImageSize && info.ImageDimension.Width < MaxThumbnailImageSize)
                return false;

            byte[] data = null;

            info.ReadFile(fs =>
            {
                if (fs == Stream.Null)
                    return;

                using (var image = Image.FromStream(fs))
                {
                    int resultWidth;
                    int resultHeight;

                    if (image.Width > image.Height)
                    {
                        resultWidth = MaxThumbnailImageSize;
                        resultHeight = (int)((decimal)MaxThumbnailImageSize / image.Width * image.Height);
                    }
                    else
                    {
                        resultHeight = MaxThumbnailImageSize;
                        resultWidth = (int)((decimal)MaxThumbnailImageSize / image.Height * image.Width);
                    }

                    using (var resizedBmp = AttachmentHelper.ResizeImage(image, resultWidth, resultHeight))
                    {
                        using (var ms = new MemoryStream())
                        {
                            var encoderId = ImageFormat.Jpeg.Guid;
                            var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == encoderId);
                            var encoderParams = new EncoderParameters
                            {
                                Param = new[] { new EncoderParameter(Encoder.Quality, 70L) }
                            };

                            resizedBmp.Save(ms, encoder, encoderParams);

                            data = ms.ToArray();
                        }
                    }
                }
            });

            if (data != null)
            {
                url = $"data:image/jpeg;base64,{Convert.ToBase64String(data)}";
                return true;
            }

            return false;
        }

        #endregion
    }
}