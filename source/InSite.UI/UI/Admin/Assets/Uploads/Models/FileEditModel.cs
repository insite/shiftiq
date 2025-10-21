using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Assets.Uploads.Models
{
    public class FileEditModel
    {
        #region Constants

        private static readonly ICollection<char> InvalidNameCharacters = new HashSet<char>(System.IO.Path.GetInvalidFileNameChars());
        private static readonly ICollection<char> InvalidPathCharacters = new HashSet<char>(System.IO.Path.GetInvalidPathChars());
        private static readonly ICollection<string> ReservedNames = new List<string>
        {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        #endregion

        #region Properties

        public string Name { get; private set; }
        public string Path { get; private set; }
        public string Location { get; private set; }

        #endregion

        #region Initalization

        public static FileEditModel Create(HttpRequest request, out string validationError)
        {
            validationError = null;

            var path = request["path"];
            if (string.IsNullOrEmpty(path))
                throw new ApplicationError("The path is undefined");

            var extension = FileModel.GetExtension(path);
            var name = (request["name"] ?? string.Empty).Trim(' ').TrimEnd('.');
            var location = (request["location"] ?? string.Empty).Trim(' ').TrimEnd('.').Replace("\\", "/");

            if (!location.EndsWith("/"))
                location = location + "/";

            if (!location.StartsWith("/"))
                location = "/" + location;

            if (string.IsNullOrEmpty(name))
                validationError = "The file name is required field";
            else if (name.Length > 128)
                validationError = "The file name must be less than 129 characters";
            else if (location.Length + name.Length + extension.Length > 256)
                validationError = "The fully qualified file name must be less than 257 characters";
            else if (name.Any(x => InvalidNameCharacters.Contains(x)))
                validationError = $"'{name}' is not valid filename. A filename can't contain any of the following characters: \" < > | : & ? \\ /";
            else if (ReservedNames.Any(x => string.Compare(x, name, true) == 0))
                validationError = $"'{name}' is reserved name and it can't be used as a filename.";

            else if (string.IsNullOrEmpty(location))
                validationError = "The location is required field";
            else if (location.Length > 128)
                validationError = "The location must be less than 129 characters";
            else if (location.Any(x => InvalidPathCharacters.Contains(x)))
                validationError = $"'{location}' is not valid location. A location can't contain any of the following characters: \" < > | : & ?";

            return validationError == null
                ? new FileEditModel
                {
                    Name = name + extension,
                    Path = location + name + extension,
                    Location = location
                }
                : null;
        }

        #endregion
    }
}