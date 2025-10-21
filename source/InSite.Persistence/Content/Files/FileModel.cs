using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public interface IFile
    {
        string Path { get; }
    }

    public class FileModel : IFile
    {
        #region Constants

        private static readonly IReadOnlyCollection<char> InvalidCharacters = new HashSet<char>(new[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*' });

        #endregion

        #region Properties

        public Guid Guid => _storage.FileId;

        public Guid OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string Path
        {
            get => _path;
            set
            {
                if (!IsPathValid(value))
                    throw ApplicationError.Create("Invalid file path: {0}", value);

                _path = value;
            }
        }

        public string Name
        {
            get => GetName(_path);
            set => Path = GetLocation(_path) + value;
        }

        public string Type
        {
            get => GetExtension(_path);
            set => Name = GetNameWithoutExtension(_path) + value;
        }

        public int ContentSize => _storage.ContentSize;
        public string DataFingerprint => _storage.DataFingerprint;

        public Guid? Uploader => _storage.Uploader;
        public DateTimeOffset Uploaded => _storage.Uploaded;

        public bool IsNew => _storage.IsNew;

        public List<string> ActionMessages { get; }

        #endregion

        #region Fields

        private string _path;
        private IFileStorageItem _storage;

        #endregion

        #region Construction

        public FileModel(IFileStorageItem storage)
        {
            _storage = storage;

            OrganizationIdentifier = storage.OrganizationIdentifier;
            _path = storage.Path;

            ActionMessages = new List<string>();
        }

        #endregion

        #region Methods

        public Stream Read() => _storage.Read();

        public void Write(Stream stream, bool isCheckFileSizeLimits = true) => _storage.Write(stream, isCheckFileSizeLimits);

        public void Write(Action<Stream> write) => _storage.Write(write);

        public void Save() => _storage.Save();

        public void Delete() => _storage.Delete();

        #endregion

        #region Helper methods

        public static string GetName(string path)
        {
            if (path == null)
                return null;

            var index = path.LastIndexOf('/');
            if (index < 0)
                return null;

            var length = path.Length - ++index;
            if (length <= 0)
                return null;

            return path.Substring(index, length);
        }

        public static string GetExtension(string path)
        {
            if (path == null)
                return null;

            var index = path.LastIndexOf('.');
            if (index < 0)
                return null;

            var length = path.Length - index;
            if (length <= 1)
                return null;

            return path.Substring(index, length);
        }

        public static string GetNameWithoutExtension(string path)
        {
            if (path == null)
                return null;

            var startIndex = path.LastIndexOf('/');
            if (startIndex < 0) startIndex = 0;
            else startIndex++;

            var endIndex = path.LastIndexOf('.');
            if (endIndex < 0)
                return null;

            var length = endIndex - startIndex;
            if (length <= 0)
                return null;

            return path.Substring(startIndex, length);
        }

        public static string GetLocation(string path)
        {
            if (path == null)
                return null;

            var index = path.LastIndexOf('/');

            return index == -1
                ? null
                : path.Substring(0, index + 1);
        }

        public static bool IsPathValid(string path)
        {
            var isValid = !string.IsNullOrEmpty(path);

            if (isValid)
            {
                var parts = path.Split('/');

                isValid = parts.Length > 1 && string.IsNullOrEmpty(parts[0])
                    && !parts.Skip(1).Any(p => p.Any(ch => InvalidCharacters.Contains(ch)) || string.IsNullOrWhiteSpace(p));

                if (isValid)
                {
                    var name = parts[parts.Length - 1];
                    if (name.Length > 128 || GetExtension(name) == null)
                        isValid = false;
                }
            }

            return isValid;
        }

        #endregion
    }
}
