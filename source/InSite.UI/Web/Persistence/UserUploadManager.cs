using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;

using InSite.Domain.Organizations;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

using Stream = System.IO.Stream;

namespace InSite.Common.Web.Setup
{
    public sealed class UserUploadManager
    {
        #region Constants

        private const int MaxPhotoCount = 2;
        private const string PhotoFileNameTemplate = "photo-id-{0}.jpg";

        #endregion

        #region Classes

        public class UserFile
        {
            public string Name { get; private set; }
            public string Url { get; private set; }
            public DateTimeOffset LastWriteTime { get; private set; }
            public long FileSize { get; private set; }

            public UserFile(Guid userId, string name, DateTimeOffset posted, long length)
            {
                Name = name;
                LastWriteTime = Clock.Trim(posted);
                Url = GetFileUrl(userId, Name, LastWriteTime);
                FileSize = length;
            }
        }

        #endregion

        #region Fields

        private readonly FileProvider _provider;
        private static readonly Regex _photoFileIndexRegex = new Regex("photo-id-(\\d+)\\.jpg", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Construction

        public UserUploadManager(FileProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        #endregion

        #region Methods (photo)

        public void SavePhoto(Guid personId, Stream inputStream, OrganizationState organization)
        {
            var path = GetUserUploadPath(organization, personId);
            var index = GetAvailablePhotoIndex(organization, path);
            if (!index.HasValue)
                throw ApplicationError.Create($"You have exceeded the maximum number of uploaded photos: {MaxPhotoCount:n0}");

            var filePath = $"{path}/{GetPhotoFileName(index.Value)}";

            _provider.Save(organization.OrganizationIdentifier, filePath, inputStream);
        }

        public IReadOnlyList<string> GetPhotoUrls(Guid personId, OrganizationState organization)
        {
            return GetFiles(organization, personId, x => new { x.Name, x.Uploaded })
                .Where(x => _photoFileIndexRegex.IsMatch(x.Name))
                .OrderBy(x => x.Name)
                .Select(x => GetFileUrl(personId, x.Name, x.Uploaded))
                .ToArray();
        }

        public void DeletePhoto(Guid personId, int index, OrganizationState organization)
        {
            var dirPath = GetUserUploadPath(organization, personId);
            var filePath = $"{dirPath}/{GetPhotoFileName(index)}";

            _provider.Delete(organization.OrganizationIdentifier, filePath);

            ResequencePhotos(organization, dirPath);
        }

        #endregion

        #region Methods (files)

        public FileDescriptor GetFileDescriptor(Guid personId, string fileName, OrganizationState organization)
        {
            if (organization == null)
                return null;

            var path = GetUserUploadPath(organization, personId);

            return _provider.GetDescriptor(organization.OrganizationIdentifier, $"{path}/{fileName}");
        }

        public IReadOnlyList<UserFile> GetAllFiles(Guid personId, OrganizationState organization)
        {
            if (organization == null)
                return new UserFile[0];

            var path = GetUserUploadPath(organization, personId) + "/";
            return UploadSearch
                .Bind(x => new
                {
                    x.Name,
                    x.Uploaded,
                    Length = x.ContentSize
                }, x => x.OrganizationIdentifier == organization.OrganizationIdentifier && x.UploadType == UploadType.InSiteFile && x.NavigateUrl.StartsWith(path))
                .OrderBy(x => x.Name)
                .Select(x => new UserFile(personId, x.Name, x.Uploaded, x.Length ?? 0))
                .ToArray();
        }

        #endregion

        #region Helpers

        private int? GetAvailablePhotoIndex(OrganizationState organization, string path)
        {
            var indexes = new HashSet<int>();
            var files = GetFiles(organization, path, x => x.Name);

            foreach (var name in files)
            {
                var m = _photoFileIndexRegex.Match(name);
                if (m.Success)
                    indexes.Add(int.Parse(m.Groups[1].Value));
            }

            for (var i = 1; i <= MaxPhotoCount; i++)
            {
                if (!indexes.Contains(i))
                    return i;
            }

            return null;
        }

        private void ResequencePhotos(OrganizationState organization, string path)
        {
            var index = 1;
            var files = GetFiles(organization, path, x => new { x.Name, x.NavigateUrl });

            foreach (var file in files.OrderBy(x => x.Name))
            {
                if (_photoFileIndexRegex.IsMatch(file.Name))
                    _provider.Move(organization.OrganizationIdentifier, file.NavigateUrl, $"{path}/{GetPhotoFileName(index++)}");
            }
        }

        private static string GetUserUploadPath(OrganizationState organization, Guid personId) =>
            $"/Users/{organization.OrganizationCode}/{personId}";

        private static string GetPhotoFileName(int index) => string.Format(PhotoFileNameTemplate, index);

        private static string GetFileUrl(Guid personId, string fileName, DateTimeOffset fileDate) =>
            $"~/Web/Persistence/userfile.ashx?u={personId}&f={HttpUtility.UrlEncode(fileName)}&d={Clock.Trim(fileDate).UtcDateTime.Ticks}";

        private IReadOnlyList<T> GetFiles<T>(OrganizationState organization, Guid personId, Expression<Func<Upload, T>> binder)
        {
            var path = GetUserUploadPath(organization, personId);

            return GetFiles(organization, path, binder);
        }

        private IReadOnlyList<T> GetFiles<T>(OrganizationState organization, string path, Expression<Func<Upload, T>> binder)
        {
            var pattern = string.Format(PhotoFileNameTemplate, "%");

            return UploadSearch.Bind(
                binder,
                x => x.OrganizationIdentifier == organization.OrganizationIdentifier
                  && x.UploadType == UploadType.InSiteFile
                  && x.NavigateUrl.StartsWith(path + "/")
                  && SqlFunctions.PatIndex(pattern, x.Name) > 0
            );
        }

        #endregion
    }
}
