using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.Web.Infrastructure;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Integration.ImageMagick;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Admin.Assessments.Attachments.Utilities
{
    public static class AttachmentHelper
    {
        #region Constants

        private static readonly HashSet<string> AttachmentChangeTypeFilter;

        #endregion

        #region Fields

        private static readonly object _syncRoot = new object();
        private static DateTime _nextClearTime = DateTime.MinValue;

        #endregion

        #region Construction

        static AttachmentHelper()
        {
            var changeType = typeof(Change);
            var targetType = typeof(AttachmentAdded);

            AttachmentChangeTypeFilter = System.Reflection.Assembly.GetAssembly(targetType).GetTypes()
                .Where(t => t.IsPublic && t.IsClass
                         && t.IsSubclassOf(changeType)
                         && t.Namespace == targetType.Namespace
                         && t.Name.StartsWith("Attachment"))
                .Select(x => x.Name)
                .ToHashSet();
        }

        #endregion

        #region Methods (temp files: save)

        public static Guid SaveTempFile(FileInfo fileInfo)
        {
            Guid storageId;

            var fileId = UniqueIdentifier.Create();
            var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var user = CurrentSessionState.Identity.User;

            var attachment = new AttachmentInfo
            {
                Author = user.UserIdentifier,
                Title = fileName,
                Uploaded = DateTimeOffset.UtcNow,

                File = new AttachmentFileInfo
                {
                    Name = fileName,
                    Extension = Path.GetExtension(fileInfo.Name),
                    ContentLength = (int)fileInfo.Length,
                }
            };

            using (var stream = fileInfo.OpenRead())
            {
                attachment.Type = Attachment.GetAttachmentType(attachment.File.Extension);
                if (attachment.Type == AttachmentType.Image)
                    attachment.Image = ReadImageProps(stream);

                storageId = TempFileStorage.Create();

                TempFileStorage.Open(storageId, dir =>
                {
                    {
                        var path = GetAttachmentDataPath(dir);
                        using (var fs = File.Open(path, FileMode.Create, FileAccess.Write))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.CopyTo(fs);
                        }
                    }

                    {
                        var json = JsonConvert.SerializeObject(attachment);
                        var path = GetAttachmentInfoPath(dir);

                        File.WriteAllText(path, json);
                    }
                });
            }

            return storageId;
        }

        #endregion

        #region Methods (temp files: load)

        public static AttachmentInfo LoadAttachmentInfo(Guid storageId)
        {
            AttachmentInfo result = null;

            TempFileStorage.Open(storageId, dir =>
            {
                var path = GetAttachmentInfoPath(dir);
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);

                    result = JsonConvert.DeserializeObject<AttachmentInfo>(json);
                }
            });

            return result;
        }

        public static byte[] LoadAttachmentData(Guid storageId)
        {
            byte[] result = null;

            TempFileStorage.Open(storageId, dir =>
            {
                var path = GetAttachmentDataPath(dir);
                if (File.Exists(path))
                    result = File.ReadAllBytes(path);
            });

            return result;
        }

        public static byte[] LoadAndResizeImageData(Guid storageId, AttachmentInfo attachment)
        {
            byte[] data = null;

            ReadAttachmentData(storageId, stream =>
            {
                using (var image = Image.FromStream(stream))
                {
                    using (var resizedBmp = ResizeImage(image, attachment.Image.Actual.Width, attachment.Image.Actual.Height))
                    {
                        using (var ms = new MemoryStream())
                        {
                            var imgType = FileExtension.GetImageType(attachment.File.Extension);

                            if (imgType == ImageType.Png || imgType == ImageType.Gif)
                            {
                                resizedBmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                                attachment.File.Extension = ".png";
                            }
                            else
                            {
                                var encoderId = System.Drawing.Imaging.ImageFormat.Jpeg.Guid;
                                var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == encoderId);
                                var encoderParams = new EncoderParameters
                                {
                                    Param = new[] { new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L) }
                                };

                                resizedBmp.Save(ms, encoder, encoderParams);

                                attachment.File.Extension = ".jpg";
                            }

                            data = ms.ToArray();
                        }
                    }
                }
            });

            return data;
        }

        public static void ReadAttachmentData(Guid storageId, Action<Stream> read)
        {
            TempFileStorage.Open(storageId, dir =>
            {
                var path = GetAttachmentDataPath(dir);
                using (var fs = File.Open(path, FileMode.Open, FileAccess.Read))
                    read(fs);
            });
        }

        #endregion

        #region Methods (temp files: delete)

        public static void DeleteStorage(Guid storageId)
        {
            if (storageId == Guid.Empty)
                return;

            TempFileStorage.Remove(storageId);
        }

        #endregion

        #region Methods (temp files: other)

        public static bool ExistAttachmentData(Guid storageId)
        {
            var exists = false;

            TempFileStorage.Open(storageId, dir =>
            {
                var path = GetAttachmentDataPath(dir);

                exists = File.Exists(path);
            });

            return exists;
        }

        private static string GetAttachmentInfoPath(DirectoryInfo dir) =>
            Path.Combine(dir.FullName, $"info.json");

        private static string GetAttachmentDataPath(DirectoryInfo dir) =>
            Path.Combine(dir.FullName, $"data.bin");

        #endregion

        #region Methods (question attachments)

        public static bool IsAttachmentChange(IChange e) =>
            AttachmentChangeTypeFilter.Contains(e.GetType().Name);

        public static IEnumerable<IChange> GetChanges(Guid bankId) =>
            ServiceLocator.ChangeStore.GetChanges("Bank", bankId, AttachmentChangeTypeFilter);

        public static IEnumerable<IChange> GetChanges(Guid bankId, Guid attachmentId) =>
            GetChanges(bankId).Where(e => GetAttachmentIdentifier(e).Contains(attachmentId));

        public static IEnumerable<Guid> GetAttachmentIdentifier(IChange e)
        {
            if (e is AttachmentAdded e1)
            {
                yield return e1.Attachment;
            }
            else if (e is AttachmentAddedToQuestion e2)
            {
                yield return e2.Attachment;
            }
            else if (e is AttachmentChanged e3)
            {
                yield return e3.Attachment;
            }
            else if (e is BankAttachmentDeleted e4)
            {
                yield return e4.Attachment;
            }
            else if (e is AttachmentDeletedFromQuestion e5)
            {
                yield return e5.Attachment;
            }
            else if (e is AttachmentUpgraded e6)
            {
                yield return e6.CurrentAttachment;
                yield return e6.UpgradedAttachment;
            }
            else if (e is AttachmentImageChanged e7)
            {
                yield return e7.Attachment;
            }
            else
                throw ApplicationError.Create("Unexpected change type: " + e.GetType().FullName);
        }

        #endregion

        #region Methods (path)

        public static string GetFilePath(int number, string filename) =>
            $"/Assessments/{number}/Attachments/{filename}";

        public static string GetUniqueFilePath(int number, string filename) =>
            GetUniqueFilePath(number, Path.GetFileNameWithoutExtension(filename), Path.GetExtension(filename));

        public static string GetUniqueFilePath(int assetNumber, string fileName, string fileExtension)
        {
            var path = $"/Assessments/{assetNumber}/Attachments/";

            var resultFileName = $"{fileName}{fileExtension}";

            for (var j = 1; ; j++)
            {
                if (!UploadSearch.ExistsByOrganizationIdentifier(CurrentSessionState.Identity.Organization.OrganizationIdentifier, path + resultFileName))
                    break;

                resultFileName = $"{fileName}_{j}{fileExtension}";
            }

            return path + resultFileName;
        }

        #endregion

        #region Methods (helpers)

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static AttachmentImage ReadImageProps(Stream stream)
        {
            try
            {
                var imgInfo = ImageHelper.ReadInfo(stream);

                return new AttachmentImage
                {
                    Actual = new ImageDimension
                    {
                        Height = (int)imgInfo.Height,
                        Width = (int)imgInfo.Width,
                    },
                    IsColor = imgInfo.ColorSpace != ColorSpace.Gray,
                    Resolution = (int)Math.Round(imgInfo.PixelsPerInch, MidpointRounding.AwayFromZero)
                };
            }
            catch (Exception ex)
            {
                throw ApplicationError.Create(ex, "The image is corrupted.");
            }
        }

        #endregion
    }
}