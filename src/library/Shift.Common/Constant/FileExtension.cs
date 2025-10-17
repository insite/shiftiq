using System;
using System.Collections.Generic;

using Shift.Common.Integration.ImageMagick;
using Shift.Constant;

namespace Shift.Common
{
    public static class FileExtension
    {
        #region Constants

        public static readonly string[] EmbeddedResources = { ".axd" };

        public static readonly string[] StaticAssets = { ".jpg", ".jpeg", ".png", ".gif", ".css", ".js", ".ico", ".woff", ".woff2", ".ttf", ".eot", ".svg", ".map", ".txt" };

        private static HashSet<string> _imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".bmp", ".gif", ".jpeg", ".jpg", ".png", ".tif", ".tiff" };

        private static HashSet<string> _documentExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".doc", ".docx", ".htm", ".html", ".md", ".odm", ".ods", ".odt", ".pdf", ".ppt", ".pptx", ".rtf", ".txt", ".xls", ".xlsx" };

        private static HashSet<string> _archiveExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".7z", ".rar", ".zip" };

        #endregion

        #region Methods

        public static bool IsImage(string extension)
        {
            extension = GetExtension(extension);

            return !string.IsNullOrEmpty(extension) && _imageExtensions.Contains(extension);
        }

        public static bool IsDocument(string extension)
        {
            extension = GetExtension(extension);

            return !string.IsNullOrEmpty(extension) && _documentExtensions.Contains(extension);
        }

        public static bool IsArchive(string extension)
        {
            extension = GetExtension(extension);

            return !string.IsNullOrEmpty(extension) && _archiveExtensions.Contains(extension);
        }

        public static string GetExtension(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var dotIndex = name.LastIndexOf('.');
            if (dotIndex == -1)
                return name;

            return name.Substring(dotIndex);
        }


        public static string GetImageExtension(ImageType imageType)
        {
            switch (imageType)
            {
                case ImageType.Bmp: return ".bmp";
                case ImageType.Gif: return ".gif";
                case ImageType.Jpeg: return ".jpg";
                case ImageType.Png: return ".png";
                case ImageType.Tiff: return ".tif";
            }

            return null;
        }

        public static ImageType GetImageType(string extension)
        {
            if (string.Equals(extension, ".bmp", StringComparison.OrdinalIgnoreCase))
                return ImageType.Bmp;

            if (string.Equals(extension, ".gif", StringComparison.OrdinalIgnoreCase))
                return ImageType.Gif;

            if (string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase))
                return ImageType.Jpeg;

            if (string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase))
                return ImageType.Png;

            if (string.Equals(extension, ".tif", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".tiff", StringComparison.OrdinalIgnoreCase))
                return ImageType.Tiff;

            return ImageType.Null;
        }

        #endregion
    }
}
