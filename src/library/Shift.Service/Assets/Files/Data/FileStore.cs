using InSite.Application.Files.Read;

using Microsoft.EntityFrameworkCore;

using Shift.Common;

using Shift.Toolbox;

namespace Shift.Service.Content
{
    public class FileStore : IFileStoreAsync
    {
        private readonly IDbContextFactory<TableDbContext> _tables;
        private readonly IFileChangeFactory _fileChangeFactory;
        private readonly IJsonSerializer _jsonSerializer;

        public FileStore(IDbContextFactory<TableDbContext> tables, IFileChangeFactory fileChangeFactory, IJsonSerializer jsonSerializer)
        {
            _tables = tables;
            _fileChangeFactory = fileChangeFactory;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<FileStorageModel> InsertModelAsync(FileStorageModel model)
        {
            using var db = await _tables.CreateDbContextAsync();

            FileEntity file;

            List<FileClaimEntity> claims = [];

            file = CreateFile(model);

            await db.TFile.AddAsync(file);

            claims = CreateClaimList(model.FileIdentifier, model.Claims);

            if (claims.Any())
                db.TFileClaim.AddRange(claims);

            await db.SaveChangesAsync();

            var newModel = FileSearch.CreateModel(file);

            newModel.Claims = FileSearch.CreateClaims(claims);

            return newModel;
        }

        public async Task UpdateObjectAsync(Guid fileIdentifier, Guid objectIdentifier, FileObjectType objectType)
        {
            using var db = await _tables.CreateDbContextAsync();

            var file = await db.TFile
                .Where(x => x.FileIdentifier == fileIdentifier)
                .FirstOrDefaultAsync();

            if (file == null)
                throw new ArgumentException($"File with ID = {fileIdentifier} is not found");

            file.ObjectIdentifier = objectIdentifier;

            file.ObjectType = objectType.ToString();

            await db.SaveChangesAsync();
        }

        public async Task RenameFileAsync(Guid fileIdentifier, string newFileName)
        {
            using var db = await _tables.CreateDbContextAsync();

            var file = await db.TFile
                .Where(x => x.FileIdentifier == fileIdentifier)
                .FirstOrDefaultAsync();

            if (file == null)
                throw new ArgumentException($"File with ID = {fileIdentifier} is not found");

            file.FileName = newFileName;

            await db.SaveChangesAsync();
        }

        public async Task<FileLastActivity> UpdatePropertiesAsync(Guid fileIdentifier, Guid userIdentifier, FileProperties properties, bool updateActivityChanges)
        {
            if (properties == null)
                throw new ArgumentNullException("Properties");

            FileStorageModel oldModel, newModel;

            using var db = await _tables.CreateDbContextAsync();

            var file = await db.TFile
                .Where(x => x.FileIdentifier == fileIdentifier)
                .FirstOrDefaultAsync();

            if (file == null)
                throw new ArgumentException($"File with ID = {fileIdentifier} is not found");

            oldModel = FileSearch.CreateModel(file);

            CopyProperties(file, properties, userIdentifier);

            newModel = FileSearch.CreateModel(file);

            await db.SaveChangesAsync();

            if (updateActivityChanges)
                await SaveChangeHistoryAsync(oldModel, newModel, userIdentifier);

            return new FileLastActivity
            {
                LastActivityTime = newModel.LastActivityTime,
                LastActivityUserIdentifier = newModel.LastActivityUserIdentifier
            };
        }

        public async Task UpdateClaimsAsync(Guid fileIdentifier, IEnumerable<FileClaim> claims)
        {
            using var db = await _tables.CreateDbContextAsync();

            var newClaims = CreateClaimList(fileIdentifier, claims);

            var existingClaims = await db.TFileClaim
                .Where(x => x.FileIdentifier == fileIdentifier)
                .ToListAsync();

            foreach (var existingClaim in existingClaims)
            {
                if (!newClaims.Any(x => x.ObjectIdentifier == existingClaim.ObjectIdentifier && x.ObjectType == existingClaim.ObjectType))
                    db.TFileClaim.Remove(existingClaim);
            }

            foreach (var newClaim in newClaims)
            {
                if (!existingClaims.Any(x => x.ObjectIdentifier == newClaim.ObjectIdentifier && x.ObjectType == newClaim.ObjectType))
                    await db.TFileClaim.AddAsync(newClaim);
            }

            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid fileIdentifier)
        {
            using var db = await _tables.CreateDbContextAsync();

            var file = await db.TFile
                .Where(x => x.FileIdentifier == fileIdentifier)
                .FirstOrDefaultAsync();

            if (file == null)
                return;

            var claims = await db.TFileClaim
                .Where(x => x.FileIdentifier == fileIdentifier)
                .ToListAsync();

            var activities = await db.TFileActivity
                .Where(x => x.FileIdentifier == fileIdentifier)
                .ToListAsync();

            db.TFileClaim.RemoveRange(claims);
            db.TFileActivity.RemoveRange(activities);
            db.TFile.Remove(file);

            await db.SaveChangesAsync();
        }

        public async Task UpdateFileUploadedAsync(Guid fileIdentifier, DateTimeOffset value)
        {
            using var db = await _tables.CreateDbContextAsync();

            var file = await db.TFile
                .Where(x => x.FileIdentifier == fileIdentifier)
                .FirstOrDefaultAsync();

            if (file == null)
                throw new ArgumentException($"File not found: {fileIdentifier}");

            file.FileUploaded = value;

            await db.SaveChangesAsync();
        }

        private static FileEntity CreateFile(FileStorageModel model)
        {
            var file = new FileEntity
            {
                UserIdentifier = model.UserIdentifier,
                OrganizationIdentifier = model.OrganizationIdentifier,
                ObjectType = model.ObjectType.ToString(),
                ObjectIdentifier = model.ObjectIdentifier,
                FileIdentifier = model.FileIdentifier,
                FileName = model.FileName,
                FilePath = model.FilePath,
                FileSize = model.FileSize,
                FileLocation = model.FileLocation.ToString(),
                FileContentType = model.ContentType,
                FileUploaded = DateTimeOffset.UtcNow
            };

            model.Uploaded = file.FileUploaded;

            CopyProperties(file, model.Properties, model.UserIdentifier);

            return file;
        }

        private static void CopyProperties(FileEntity file, FileProperties properties, Guid userIdentifier)
        {
            file.DocumentName = properties?.DocumentName ?? file.FileName;
            file.FileDescription = properties?.Description;
            file.FileCategory = properties?.Category;
            file.FileSubcategory = properties?.Subcategory;
            file.FileStatus = properties?.Status ?? "Uploaded";
            file.FileExpiry = properties?.Expiry;
            file.FileReceived = properties?.Received;
            file.FileAlternated = properties?.Alternated;
            file.ReviewedTime = properties?.ReviewedTime;
            file.ReviewedUserIdentifier = properties?.ReviewedUserIdentifier;
            file.ApprovedTime = properties?.ApprovedTime;
            file.ApprovedUserIdentifier = properties?.ApprovedUserIdentifier;
            file.AllowLearnerToView = properties?.AllowLearnerToView ?? true;

            file.LastActivityTime = DateTimeOffset.UtcNow;
            file.LastActivityUserIdentifier = userIdentifier;
        }

        private async Task SaveChangeHistoryAsync(FileStorageModel oldModel, FileStorageModel newModel, Guid userIdentifier)
        {
            var changes = _fileChangeFactory.CreateChanges(oldModel, newModel);
            if (changes.Length == 0)
                return;

            var jsonChanges = _jsonSerializer.Serialize(changes);

            var activity = new FileActivityEntity
            {
                FileIdentifier = oldModel.FileIdentifier,
                UserIdentifier = userIdentifier,
                ActivityIdentifier = UniqueIdentifier.Create(),
                ActivityTime = DateTimeOffset.UtcNow,
                ActivityChanges = jsonChanges
            };

            using var db = await _tables.CreateDbContextAsync();

            await db.TFileActivity.AddAsync(activity);

            await db.SaveChangesAsync();
        }

        private static List<FileClaimEntity> CreateClaimList(Guid fileIdentifier, IEnumerable<FileClaim> claims)
        {
            if (claims == null)
                return [];

            return claims
                .Select(x => new FileClaimEntity
                {
                    FileIdentifier = fileIdentifier,
                    ClaimIdentifier = UniqueIdentifier.Create(),
                    ClaimGranted = DateTimeOffset.UtcNow,
                    ObjectType = x.ObjectType.ToString(),
                    ObjectIdentifier = x.ObjectIdentifier
                })
                .ToList();
        }
    }
}
