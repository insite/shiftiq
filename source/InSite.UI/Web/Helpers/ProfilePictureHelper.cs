using System;
using System.Collections.Generic;
using System.IO;

using Shift.Common.Integration.ImageMagick;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Web.Helpers
{
    public class ProfilePictureProcessInfo
    {
        public AlertType Type { get; set; }
        public string Message { get; set; }
    }

    public static class ProfilePictureHelper
    {
        public const int MaxProfileImageSize = 200;

        public static string GetFilePhysicalPath(string fileName, string guid)
        {
            var folder = ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Library", "Accounts", "Users", guid, "Avatars");
            return Path.Combine(folder, fileName);
        }

        public static bool PreprocessImage(string uploadPath, string storagePath, out ProfilePictureProcessInfo result)
        {
            try
            {
                var messages = new List<string>();

                using (var input = File.Open(uploadPath, FileMode.Open, FileAccess.Read))
                {
                    var imageInfo = ImageHelper.ReadInfo(input);

                    if (imageInfo.Format == ImageFormat.Unknown || imageInfo.Height <= 0 || imageInfo.Width <= 0)
                    {
                        result = new ProfilePictureProcessInfo()
                        {
                            Message = "Unknown image file format.",
                            Type = AlertType.Error
                        };

                        return false;
                    }

                    if (imageInfo.Height < MaxProfileImageSize || imageInfo.Width < MaxProfileImageSize)
                    {
                        result = new ProfilePictureProcessInfo()
                        {
                            Message = "The uploaded image is too small. The width and height of image must be greater or equal to {MaxProfileImageSize} pixels.",
                            Type = AlertType.Error
                        };

                        return false;
                    }

                    using (var output = File.Open(storagePath, FileMode.Create, FileAccess.Write))
                        ImageHelper.AdjustImage(input, output, ImageType.Jpeg, true, messages, MaxProfileImageSize, MaxProfileImageSize);
                }
            }
            catch (Exception)
            {
                result = new ProfilePictureProcessInfo()
                {
                    Message = "An error occurred during reading of uploaded image.",
                    Type = AlertType.Error
                };

                return false;
            }

            result = new ProfilePictureProcessInfo()
            {
                Message = "Your changes are saved.",
                Type = AlertType.Success
            };

            return true;
        }
    }
}