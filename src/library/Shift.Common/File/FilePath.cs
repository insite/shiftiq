using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Shift.Common
{
    public class FilePath
    {
        public FilePathType PathType { get; set; } = FilePathType.Unknown;

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public FilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                PathType = FilePathType.Unknown;
            }

            else if (path.StartsWith(@"\\"))
            {
                PathType = FilePathType.RemoteFile;

                FileName = Path.GetFileName(path);
            }

            else if (Path.IsPathRooted(path) && Regex.IsMatch(path, @"^[a-zA-Z]:\\"))
            {
                PathType = FilePathType.LocalFile;

                FileName = Path.GetFileName(path);
            }

            else if (Uri.TryCreate(path, UriKind.Absolute, out Uri absolute) &&
                (absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps))
            {
                PathType = FilePathType.AbsoluteUrl;

                var uri = new Uri(path);

                FileName = Path.GetFileName(uri.LocalPath);
            }

            else if (Uri.TryCreate(path, UriKind.Relative, out Uri relative))
            {
                PathType = FilePathType.RelativeUrl;

                FileName = Path.GetFileName(path);
            }

            if (!string.IsNullOrWhiteSpace(FileName))
            {
                FileExtension = Path.GetExtension(FileName);
            }
            else
            {
                FileName = null;

                FileExtension = null;
            }
        }
    }
}
