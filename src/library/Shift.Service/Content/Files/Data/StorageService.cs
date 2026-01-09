using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

using InSite.Application.Files.Read;

using Shift.Common;
using Shift.Toolbox;

namespace Shift.Service.Content
{
    public partial class StorageService : IStorageServiceAsync
    {
        private static readonly Encoding LatinEncoding = Encoding.GetEncoding("ISO-8859-1");
        private static readonly Regex FileUrlRegex = RelativePathToRetrieveFile();

        private readonly IFileSearchAsync _fileSearch;
        private readonly IFileStoreAsync _fileStore;
        private readonly IFileManagerServiceAsync _fileManagerService;
        private readonly int _tempFileExpirationInMinutes;

        private readonly ConcurrentDictionary<Guid, FileStorageModel> _files;

        public StorageService(
            IFileSearchAsync fileSearch,
            IFileStoreAsync fileStore,
            IFileManagerServiceAsync fileManagerService,
            AppSettings appSettings
            )
        {
            _fileSearch = fileSearch;
            _fileStore = fileStore;
            _fileManagerService = fileManagerService;
            _tempFileExpirationInMinutes = appSettings.Application.TempFileExpirationInMinutes;
            _files = new ConcurrentDictionary<Guid, FileStorageModel>();
        }

        public string GetFileUrl(FileStorageModel model, bool download = false) =>
            GetFileUrl(model.FileIdentifier, model.FileName, download);

        public string GetFileUrl(Guid fileId, string fileName, bool download = false)
        {
            var url = $"content/files/{fileId}/{fileName}".ToLower();

            if (download)
                url += "?download=1";

            return url;
        }

        public (Guid? FileIdentifier, string? FileName) ParseFileUrl(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return (null, null);

            var match = FileUrlRegex.Match(fileUrl);

            var fileIdentifier = Guid.TryParse(match.Groups["fileId"].Value, out var id)
                ? id
                : (Guid?)null;

            var fileName = match.Groups["fileName"].Value;
            if (fileName == string.Empty)
                fileName = null;

            return (fileIdentifier, fileName);
        }

        public List<(Guid FileIdentifier, string? FileName)> ParseSurveyResponseAnswer(string responseAnswerText)
        {
            var result = new List<(Guid, string?)>();
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

        public async Task<FileStorageModel> CreateAsync(
            Stream file,
            string fileName,
            Guid organizationIdentifier,
            Guid actorUserIdentifier,
            Guid objectIdentifier,
            FileObjectType objectType,
            FileProperties properties,
            IEnumerable<FileClaim> claims,
            FileLocation fileLocation = FileLocation.Local,
            string? filePath = null
            )
        {
            fileName = AdjustFileName(fileName);

            var fileIdentifier = UniqueIdentifier.Create();
            var fileSize = await _fileManagerService.SaveFileAsync(organizationIdentifier, fileIdentifier, fileName, filePath, file);

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

            model = await _fileStore.InsertModelAsync(model);

            _files.AddOrUpdate(model.FileIdentifier, model, (x, y) => model);

            return model.Clone();
        }

        public async Task ChangeObjectAsync(Guid fileIdentifier, Guid objectIdentifier, FileObjectType objectType)
        {
            var model = await GetFileAsync(fileIdentifier)
                ?? throw new Exception($"File not found: {fileIdentifier}");

            model.ObjectIdentifier = objectIdentifier;

            model.ObjectType = objectType;

            await _fileStore.UpdateObjectAsync(fileIdentifier, objectIdentifier, objectType);

            _files.AddOrUpdate(fileIdentifier, model, (x, y) => model);
        }

        public async Task RenameFileAsync(Guid fileIdentifier, Guid userIdentifier, string newFileName)
        {
            var model = await GetFileAsync(fileIdentifier)
                ?? throw new Exception($"File not found: {fileIdentifier}");

            var oldFileName = model.FileName;

            model.FileName = AdjustFileName(newFileName);

            model.Properties.DocumentName = newFileName;

            _fileManagerService.RenameFile(model.OrganizationIdentifier, model.FileIdentifier, oldFileName, model.FileName);

            await _fileStore.RenameFileAsync(fileIdentifier, newFileName);

            var lastActivity = await _fileStore.UpdatePropertiesAsync(fileIdentifier, userIdentifier, model.Properties, false);

            lastActivity.CopyToModel(model);

            _files.AddOrUpdate(fileIdentifier, model, (x, y) => model);
        }

        public async Task ChangePropertiesAsync(Guid fileIdentifier, Guid userIdentifier, FileProperties properties, bool updateActivityChanges)
        {
            var model = await GetFileAsync(fileIdentifier)
                ?? throw new Exception($"File not found: {fileIdentifier}");

            model.Properties = properties.Clone();

            var lastActivity = await _fileStore.UpdatePropertiesAsync(fileIdentifier, userIdentifier, properties, updateActivityChanges);

            lastActivity.CopyToModel(model);

            _files.AddOrUpdate(fileIdentifier, model, (x, y) => model);
        }

        public async Task ChangeClaimsAsync(Guid fileIdentifier, IEnumerable<FileClaim> claims)
        {
            var model = await GetFileAsync(fileIdentifier)
                ?? throw new Exception($"File not found: {fileIdentifier}");

            model.Claims = FileClaim.CloneList(claims);

            await _fileStore.UpdateClaimsAsync(fileIdentifier, claims);

            _files.AddOrUpdate(fileIdentifier, model, (x, y) => model);
        }

        public async Task DeleteAsync(Guid fileIdentifier)
        {
            var model = await GetFileAsync(fileIdentifier);

            if (model == null)
                return;

            await DeleteFileAsync(model);
        }

        public async Task DeleteOnlyRecordAsync(Guid fileIdentifier)
        {
            var model = await GetFileAsync(fileIdentifier);

            if (model == null)
                return;

            await _fileStore.DeleteAsync(model.FileIdentifier);

            _files.TryRemove(model.FileIdentifier, out var _);
        }

        public async Task<int> DeleteExpiredFilesAsync()
        {
            var expiredAt = DateTimeOffset.UtcNow.AddMinutes(-_tempFileExpirationInMinutes);

            var models = await _fileSearch.GetExpiredModelsAsync(expiredAt);

            foreach (var model in models)
                await DeleteFileAsync(model);

            return models.Count;
        }

        public async Task<FileStorageModel?> GetFileAsync(Guid fileIdentifier)
        {
            if (!_files.TryGetValue(fileIdentifier, out var model))
            {
                model = await _fileSearch.GetModelAsync(fileIdentifier);
                _files.TryAdd(fileIdentifier, model);
            }

            return model?.Clone();
        }

        public async Task<(FileStorageModel, Stream)> GetFileStreamAsync(Guid fileIdentifier)
        {
            var model = await GetFileAsync(fileIdentifier)
                ?? throw new Exception($"File not found: {fileIdentifier}");

            var stream = await _fileManagerService.ReadFileStreamAsync(model.OrganizationIdentifier, model.FileIdentifier, model.FileName, model.FilePath);

            return (model, stream);
        }

        public async Task<FileGrantStatus> GetGrantStatusAsync(ISimplePrincipal identity, Guid fileIdentifier)
        {
            var model = await GetFileAsync(fileIdentifier);

            return Authorize(identity, model);
        }

        public async Task<(FileGrantStatus, FileStorageModel?)> GetFileAndAuthorizeAsync(ISimplePrincipal identity, Guid fileIdentifier)
        {
            var model = await GetFileAsync(fileIdentifier);
            var status = Authorize(identity, model);
            return status != FileGrantStatus.Granted ? (status, null) : (status, model);
        }

        public async Task<(FileGrantStatus, FileStorageModel?, Stream?)> GetFileStreamAndAuthorizeAsync(ISimplePrincipal identity, Guid fileIdentifier)
        {
            var model = await GetFileAsync(fileIdentifier)
                ?? throw new Exception($"File not found: {fileIdentifier}");

            var status = Authorize(identity, model);

            if (status != FileGrantStatus.Granted)
                return (status, null, null);

            var stream = await _fileManagerService.ReadFileStreamAsync(model.OrganizationIdentifier, model.FileIdentifier, model.FileName, model.FilePath);

            return (status, model, stream);
        }

        public async Task<List<FileStorageModel>> GetGrantedFilesAsync(ISimplePrincipal identity, Guid objectIdentifier, string? documentName = null)
        {
            return await GetGrantedFilesAsync(identity, [objectIdentifier], documentName);
        }

        public async Task<List<FileStorageModel>> GetGrantedFilesAsync(ISimplePrincipal identity, Guid[] objectIdentifiers, string? documentName = null)
        {
            var result = new List<FileStorageModel>();

            if (identity?.OrganizationId == null)
                return result;

            var organizationIdentifier = identity.OrganizationId;

            var models = await _fileSearch.GetModelsAsync(organizationIdentifier, objectIdentifiers, documentName, true);

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

        private async Task DeleteFileAsync(FileStorageModel model)
        {
            _fileManagerService.DeleteFile(model.OrganizationIdentifier, model.FileIdentifier, model.FileName);

            await _fileStore.DeleteAsync(model.FileIdentifier);

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

        private static FileGrantStatus Authorize(ISimplePrincipal identity, FileStorageModel? model)
        {
            if (model == null
                || !identity.IsOperator && identity.OrganizationId != model.OrganizationIdentifier
                )
            {
                return FileGrantStatus.NoFile;
            }

            var claims = model.Claims ?? [];

            if (identity.IsOperator
                || !claims.Any()
                || claims.Any(x => x.ObjectIdentifier == identity.UserId)
                )
            {
                return FileGrantStatus.Granted;
            }

            var groups = identity.RoleIds ?? [];

            if (identity.UserId == null || groups.Length == 0)
            {
                return FileGrantStatus.Denied;
            }

            foreach (var claim in claims)
            {
                if (claim.ObjectType == FileClaimObjectType.Group && groups.Contains(claim.ObjectIdentifier))
                {
                    return FileGrantStatus.Granted;
                }
            }

            return FileGrantStatus.Denied;
        }

        [GeneratedRegex("^content/files/(?<fileId>[^/]+)/(?<fileName>[^/]+)$")]
        public static partial Regex RelativePathToRetrieveFile();
    }
}
