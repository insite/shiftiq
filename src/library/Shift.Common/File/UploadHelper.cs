using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shift.Common.File
{
    public static class UploadHelper
    {
        public const string MetadataFileName = "__Metadata.json";

        public static List<string> GetFiles(UploadMetadata metadata)
        {
            var result = new List<string>();

            if (string.IsNullOrEmpty(metadata?.UploadFolder))
                return result;

            var files = Directory.GetFiles(metadata.UploadFolder);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (!string.Equals(fileName, MetadataFileName, StringComparison.OrdinalIgnoreCase))
                    result.Add(file);
            }

            return result;
        }

        public static void CopyUploadedFiles(UploadMetadata fromTempUploadFolder, string toPhysicalPath, bool excludeMetadata)
        {
            if (!Directory.Exists(toPhysicalPath))
                Directory.CreateDirectory(toPhysicalPath);

            var source = new DirectoryInfo(fromTempUploadFolder.UploadFolder);
            foreach (FileInfo file in source.GetFiles())
            {
                if (excludeMetadata && file.Name == MetadataFileName)
                    continue;

                string target = Path.Combine(toPhysicalPath, file.Name);
                file.CopyTo(target, true);
            }
        }

        public static string CreateTempFolderForUser(FilePaths filePaths, string user)
        {
            var path = GetTempFolderForUser(filePaths, user);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static string GetTempFolderForUser(FilePaths filePaths, string user)
        {
            var today = DateTime.Today;

            var id = Guid.NewGuid().ToString();

            return filePaths.GetPhysicalPathToShareFolder(
                new[] { "Files", "Temp", "Uploads", $"{today.Year}", $"{today.Month:00}", $"{today.Day:00}", user, id });
        }

        public static string Sanitize(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()))
                .Replace("[", "_").Replace("]", "_")
                .Replace("(", "_").Replace(")", "_");
        }

        public static string SaveUploadedFiles(FilePaths filePaths, UploadMetadata upload, string subfolder)
        {
            var subfolderSegments = subfolder.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            var allSegments = new[] { "Files", "Tenants", upload.OrganizationCode }
                .Concat(subfolderSegments)
                .ToArray();

            var physicalPath = filePaths.GetPhysicalPathToShareFolder(allSegments);

            CopyUploadedFiles(upload, physicalPath, true);

            var urlFolder = GetFileFolderUrl(subfolder);

            return urlFolder + upload.FileName;
        }

        public static string GetFileFolderUrl(string subfolder)
        {
            const string rootRelativeUrl = "/in-content/";

            var subfolderSegments = subfolder.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            var relativeUrl = string.Join("/", subfolderSegments);

            return rootRelativeUrl + relativeUrl;
        }
    }
}
