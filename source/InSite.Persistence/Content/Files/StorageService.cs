using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using InSite.Application.Files.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public class StorageService : IStorageService
    {
        private static readonly Encoding LatinEncoding = Encoding.GetEncoding("ISO-8859-1");

        private readonly IFileSearch _fileSearch;
        private readonly IFileStore _fileStore;
        private readonly IFileManagerService _fileManagerService;
        private readonly int _tempFileExpirationInMinutes;

        private readonly ConcurrentDictionary<Guid, FileStorageModel> _files;

        public StorageService(
            IFileSearch fileSearch,
            IFileStore fileStore,
            IFileManagerService fileManagerService,
            int tempFileExpirationInMinutes
            )
        {
            _fileSearch = fileSearch;
            _fileStore = fileStore;
            _fileManagerService = fileManagerService;
            _tempFileExpirationInMinutes = tempFileExpirationInMinutes;
            _files = new ConcurrentDictionary<Guid, FileStorageModel>();
        }

        public string GetFileUrl(FileStorageModel model, bool download = false) =>
            GetFileUrl(model.FileIdentifier, model.FileName, download);

        public string GetFileUrl(Guid fileIdentifier, string fileName, bool download = false)
        {
            var url = $"/api/assets/files/{fileIdentifier}/{fileName}".ToLower();

            if (download)
                url += "?download=1";

            return url;
        }

        static readonly Regex FileUrlRegex = new Regex("^/api/assets/files/(?<fileIdentifier>[^/]+)/(?<fileName>[^/]+)$");

        public (Guid? FileIdentifier, string FileName) ParseFileUrl(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return (null, null);

            var match = FileUrlRegex.Match(fileUrl);

            var fileIdentifier = Guid.TryParse(match.Groups["fileIdentifier"].Value, out var id)
                ? id
                : (Guid?)null;

            var fileName = match.Groups["fileName"].Value;
            if (fileName == string.Empty)
                fileName = null;

            return (fileIdentifier, fileName);
        }

        public List<(Guid FileIdentifier, string FileName)> ParseSurveyResponseAnswer(string responseAnswerText)
        {
            var result = new List<(Guid, string)>();
            if (string.IsNullOrEmpty(responseAnswerText))
                return result;

            var urls = StringHelper.Split(responseAnswerText);
            foreach (var url in urls)
            {
                var (fileIdentifier, fileName) = ParseFileUrl(url);

                if (fileIdentifier.HasValue)
                    result.Add((fileIdentifier.Value, fileName));
            }

            return result;
        }

        public bool IsRemoteFilePathValid(FileStorageModel file)
        {
            if (file.FileLocation != FileLocation.Remote)
                return true;

            if (string.IsNullOrEmpty(file.FilePath))
                return false;

            return file.FilePath.StartsWith(@"\\")
                || file.FilePath.Contains(@":\")
                || Uri.TryCreate(file.FilePath, UriKind.Absolute, out var uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public FileStorageModel Create(
            Stream file,
            string fileName,
            Guid organizationIdentifier,
            Guid actorUserIdentifier,
            Guid objectIdentifier,
            FileObjectType objectType,
            FileProperties properties,
            IEnumerable<FileClaim> claims,
            FileLocation fileLocation = FileLocation.Local,
            string filePath = null
            )
        {
            fileName = AdjustFileName(fileName);

            var fileIdentifier = UniqueIdentifier.Create();
            var fileSize = _fileManagerService.SaveFile(organizationIdentifier, fileIdentifier, fileName, filePath, file);

            var model = new FileStorageModel
            {
                FileIdentifier = fileIdentifier,
                UserIdentifier = actorUserIdentifier,
                OrganizationIdentifier = organizationIdentifier,
                ObjectIdentifier = objectIdentifier,
                ObjectType = objectType,
                FileLocation = fileLocation,
                FileName = fileName,
                FilePath = filePath,
                FileSize = fileSize,
                ContentType = MimeMapping.GetContentType(fileName),
                Properties = properties,
                Claims = claims
            };

            model = _fileStore.InsertModel(model);

            _files.AddOrUpdate(model.FileIdentifier, model, (x, y) => model);

            return model.Clone();
        }

        public void ChangeObject(Guid fileIdentifier, Guid objectIdentifier, FileObjectType objectType)
        {
            var model = GetFile(fileIdentifier);

            model.ObjectIdentifier = objectIdentifier;
            model.ObjectType = objectType;

            _fileStore.UpdateObject(fileIdentifier, objectIdentifier, objectType);

            _files.AddOrUpdate(fileIdentifier, model, (x, y) => model);
        }

        public void RenameFile(Guid fileIdentifier, Guid userIdentifier, string newFileName)
        {
            var model = GetFile(fileIdentifier);
            var oldFileName = model.FileName;

            model.FileName = AdjustFileName(newFileName);
            model.Properties.DocumentName = newFileName;

            _fileManagerService.RenameFile(model.OrganizationIdentifier, model.FileIdentifier, oldFileName, model.FileName);

            _fileStore.RenameFile(fileIdentifier, newFileName);

            var lastActivity = _fileStore.UpdateProperties(fileIdentifier, userIdentifier, model.Properties, false);

            lastActivity.CopyToModel(model);

            _files.AddOrUpdate(fileIdentifier, model, (x, y) => model);
        }

        public void ChangeProperties(Guid fileIdentifier, Guid userIdentifier, FileProperties properties, bool updateActivityChanges)
        {
            var model = GetFile(fileIdentifier);
            model.Properties = properties.Clone();

            var lastActivity = _fileStore.UpdateProperties(fileIdentifier, userIdentifier, properties, updateActivityChanges);

            lastActivity.CopyToModel(model);

            _files.AddOrUpdate(fileIdentifier, model, (x, y) => model);
        }

        public void ChangeClaims(Guid fileIdentifier, IEnumerable<FileClaim> claims)
        {
            var model = GetFile(fileIdentifier);
            model.Claims = FileClaim.CloneList(claims);

            _fileStore.UpdateClaims(fileIdentifier, claims);

            _files.AddOrUpdate(fileIdentifier, model, (x, y) => model);
        }

        public void Delete(Guid fileIdentifier)
        {
            var model = GetFile(fileIdentifier);
            if (model == null)
                return;

            DeleteFile(model);
        }

        public void DeleteOnlyRecord(Guid fileIdentifier)
        {
            var model = GetFile(fileIdentifier);
            if (model == null)
                return;

            _fileStore.Delete(model.FileIdentifier);
            _files.TryRemove(model.FileIdentifier, out var _);
        }

        public int DeleteExpiredFiles()
        {
            var expiredAt = DateTimeOffset.UtcNow.AddMinutes(-_tempFileExpirationInMinutes);
            var models = _fileSearch.GetExpiredModels(expiredAt);

            foreach (var model in models)
                DeleteFile(model);

            return models.Count;
        }

        public FileStorageModel GetFile(Guid fileIdentifier)
        {
            if (!_files.TryGetValue(fileIdentifier, out var model))
            {
                model = _fileSearch.GetModel(fileIdentifier);
                _files.TryAdd(fileIdentifier, model);
            }

            return model?.Clone();
        }

        public bool IsFileExist(Guid fileIdentifier)
        {
            var model = GetFile(fileIdentifier);
            return _fileManagerService.IsFileExist(model.OrganizationIdentifier, model.FileIdentifier, model.FileName, model.FilePath);
        }

        public (FileStorageModel, Stream) GetFileStream(Guid fileIdentifier)
        {
            var model = GetFile(fileIdentifier);
            var stream = _fileManagerService.ReadFileStream(model.OrganizationIdentifier, model.FileIdentifier, model.FileName, model.FilePath);

            return (model, stream);
        }

        public FileGrantStatus GetGrantStatus(ISimplePrincipal identity, Guid fileIdentifier)
        {
            var model = GetFile(fileIdentifier);
            return Authorize(identity, model);
        }

        public (FileGrantStatus, FileStorageModel) GetFileAndAuthorize(ISimplePrincipal identity, Guid fileIdentifier)
        {
            var model = GetFile(fileIdentifier);
            var status = Authorize(identity, model);
            return status != FileGrantStatus.Granted ? (status, null) : (status, model);
        }

        public (FileGrantStatus, FileStorageModel, Stream) GetFileStreamAndAuthorize(ISimplePrincipal identity, Guid fileIdentifier)
        {
            var model = GetFile(fileIdentifier);
            var status = Authorize(identity, model);
            if (status != FileGrantStatus.Granted)
                return (status, null, null);

            var stream = _fileManagerService.ReadFileStream(model.OrganizationIdentifier, model.FileIdentifier, model.FileName, model.FilePath);

            return (status, model, stream);
        }

        public List<FileStorageModel> GetGrantedFiles(ISimplePrincipal identity, Guid objectIdentifier, string documentName = null)
        {
            return GetGrantedFiles(identity, new[] { objectIdentifier }, documentName);
        }

        public List<FileStorageModel> GetGrantedFiles(ISimplePrincipal identity, Guid[] objectIdentifiers, string documentName = null)
        {
            var result = new List<FileStorageModel>();

            if (identity?.OrganizationId == null)
                return result;

            var organizationIdentifier = identity.OrganizationId;

            var groups = identity.RoleIds;

            var models = _fileSearch.GetModels(organizationIdentifier, objectIdentifiers, documentName, true);

            foreach (var model in models)
            {
                if (Authorize(identity, model) == FileGrantStatus.Granted)
                    result.Add(model);
            }

            return result;
        }

        public void ClearCache()
        {
            _files.Clear();
        }

        private void DeleteFile(FileStorageModel model)
        {
            _fileManagerService.DeleteFile(model.OrganizationIdentifier, model.FileIdentifier, model.FileName);
            _fileStore.Delete(model.FileIdentifier);
            _files.TryRemove(model.FileIdentifier, out var _);
        }

        public string AdjustFileName(string fileName)
        {
            fileName = RemoveNonLatinLetters(fileName).ToLower();

            var (fileNameWithoutExt, ext) = GetFileNameParts(fileName);
            var maxLength = _fileManagerService.GetMaxFileNameLength() - ext.Length;

            var result = new StringBuilder();
            var prev = '\0';

            foreach (char c in fileNameWithoutExt)
            {
                if (char.IsLetterOrDigit(c) || c == '-' || c == '.')
                {
                    if (c != '-' || prev != c)
                    {
                        result.Append(c);
                        prev = c;
                    }
                }
                else if (char.IsWhiteSpace(c) || c == '_')
                {
                    if (prev != '-')
                    {
                        result.Append('-');
                        prev = '-';
                    }
                }

                if (result.Length == maxLength)
                    break;
            }

            if (ext.Length > 0)
                result.Append(ext);

            return result.ToString();
        }

        private (string fileNameWithoutExt, string ext) GetFileNameParts(string fileName)
        {
            var separatorIndex = fileName.LastIndexOf('.');
            if (separatorIndex <= 0 || _fileManagerService.GetMaxFileNameLength() - 2 <= separatorIndex)
                return (fileName, "");

            var fileNameWithoutExt = fileName.Substring(0, separatorIndex);
            var ext = fileName.Substring(separatorIndex);

            return (fileNameWithoutExt, ext);
        }

        private static string RemoveNonLatinLetters(string text)
        {
            var originalBytes = Encoding.UTF8.GetBytes(text);
            var resultBytes = Encoding.Convert(Encoding.UTF8, LatinEncoding, originalBytes);

            return LatinEncoding.GetString(resultBytes);
        }

        private static FileGrantStatus Authorize(ISimplePrincipal identity, FileStorageModel model)
        {
            if (model == null
                || model.OrganizationIdentifier != identity?.OrganizationId
                )
            {
                return FileGrantStatus.NoFile;
            }

            if (identity.IsOperator)
                return FileGrantStatus.Granted;

            var userIdentifier = identity.UserId;

            if (model.Claims == null
                || !model.Claims.Any()
                || model.Claims.Any(x => x.ObjectIdentifier == userIdentifier)
                )
            {
                return FileGrantStatus.Granted;
            }

            var groups = identity.RoleIds;

            if (userIdentifier == null || groups == null || !groups.Any())
                return FileGrantStatus.Denied;

            foreach (var claim in model.Claims)
            {
                if (claim.ObjectType == FileClaimObjectType.Group && groups.Contains(claim.ObjectIdentifier))
                {
                    return FileGrantStatus.Granted;
                }
            }

            return FileGrantStatus.Denied;
        }
    }
}
