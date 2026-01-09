using InSite.Application.Files.Read;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Toolbox;

namespace Shift.Service.Content
{
    public class FileSearch : IFileSearchAsync
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IDbContextFactory<TableDbContext> _tables;
        private readonly IDbContextFactory<ViewDbContext> _views;

        public FileSearch(
            IDbContextFactory<TableDbContext> tables,
            IDbContextFactory<ViewDbContext> views,
            IJsonSerializer serializer)
        {
            _tables = tables;
            _views = views;
            _jsonSerializer = serializer;
        }

        public async Task<FileStorageModel?> GetModelAsync(Guid fileIdentifier)
        {
            using var db = _tables.CreateDbContext();

            var file = await db.TFile
                .AsNoTracking()
                .Where(x => x.FileIdentifier == fileIdentifier)
                .FirstOrDefaultAsync();

            if (file == null)
                return null;

            var claims = await db.TFileClaim
                .AsNoTracking()
                .Where(x => x.FileIdentifier == fileIdentifier)
                .ToListAsync();

            var model = CreateModel(file);

            model.Claims = CreateClaims(claims);

            return model;
        }

        public async Task<List<FileStorageModel>> GetModelsAsync(Guid? organizationIdentifier, Guid objectIdentifier, string documentName, bool includeClaims)
        {
            return await GetModelsAsync(organizationIdentifier, [objectIdentifier], documentName, includeClaims);
        }

        public async Task<List<FileStorageModel>> GetModelsAsync(Guid? organizationIdentifier, Guid[] objectIdentifiers, string documentName, bool includeClaims)
        {
            using var db = _tables.CreateDbContext();

            List<FileEntity> files;

            var query = db.TFile
                .AsNoTracking()
                .Where(x => objectIdentifiers.Any(id => id == x.ObjectIdentifier));

            if (organizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == organizationIdentifier);

            if (!string.IsNullOrEmpty(documentName))
                query = query.Where(x => x.DocumentName == documentName);

            if (includeClaims)
                query = query.Include(x => x.Claims);

            files = await query.ToListAsync();

            return EntitiesToModels(files, includeClaims);
        }

        public async Task<List<FileStorageModel>> GetExpiredModelsAsync(DateTimeOffset expiredAt)
        {
            using var db = _tables.CreateDbContext();

            List<FileEntity> files;

            var objectType = FileObjectType.Temporary.ToString();

            files = await db.TFile
                .AsNoTracking()
                .Where(x => x.ObjectType == objectType && x.FileUploaded <= expiredAt)
                .ToListAsync();

            return EntitiesToModels(files, false);
        }

        public async Task<List<FileActivity>> GetFileActivitiesAsync(Guid fileIdentifier)
        {
            using var db = _tables.CreateDbContext();

            List<FileActivityEntity> activities;

            activities = await db.TFileActivity
                .AsNoTracking()
                .Where(x => x.FileIdentifier == fileIdentifier)
                .ToListAsync();

            return activities
                .Select(x => new FileActivity
                {
                    FileIdentifier = x.FileIdentifier,
                    UserIdentifier = x.UserIdentifier,
                    ActivityTime = x.ActivityTime,
                    ActivityChanges = _jsonSerializer.Deserialize<FileChange[]>(x.ActivityChanges)
                })
                .OrderByDescending(x => x.ActivityTime)
                .ToList();
        }

        public async Task<List<OrphanFile>> GetOrphanFilesAsync()
        {
            using var db = _views.CreateDbContext();

            return await db.VOrphanFile.ToListAsync();
        }

        private static List<FileStorageModel> EntitiesToModels(List<FileEntity> files, bool includeClaims)
        {
            var result = new List<FileStorageModel>();

            foreach (var file in files)
            {
                var model = CreateModel(file);

                if (includeClaims)
                    model.Claims = CreateClaims(file.Claims);

                result.Add(model);
            }

            return result;
        }

        internal static FileStorageModel CreateModel(FileEntity file)
        {
            var model = new FileStorageModel
            {
                FileIdentifier = file.FileIdentifier,
                UserIdentifier = file.UserIdentifier,
                OrganizationIdentifier = file.OrganizationIdentifier,
                ObjectIdentifier = file.ObjectIdentifier,
                ObjectType = file.ObjectType.ToEnum(FileObjectType.Temporary),
                FileName = file.FileName,
                FileSize = file.FileSize,
                FileLocation = file.FileLocation.ToEnum(FileLocation.Local),
                FilePath = file.FilePath,
                ContentType = file.FileContentType,
                Uploaded = file.FileUploaded,
                LastActivityTime = file.LastActivityTime,
                LastActivityUserIdentifier = file.LastActivityUserIdentifier
            };

            var properties = new FileProperties
            {
                DocumentName = file.DocumentName,
                Description = file.FileDescription,
                Category = file.FileCategory,
                Subcategory = file.FileSubcategory,
                Status = file.FileStatus,
                Expiry = file.FileExpiry,
                Received = file.FileReceived,
                Alternated = file.FileAlternated,
                ReviewedTime = file.ReviewedTime,
                ReviewedUserIdentifier = file.ReviewedUserIdentifier,
                ApprovedTime = file.ApprovedTime,
                ApprovedUserIdentifier = file.ApprovedUserIdentifier,
                AllowLearnerToView = file.AllowLearnerToView
            };

            model.Properties = properties;

            return model;
        }

        internal static List<FileClaim> CreateClaims(IEnumerable<FileClaimEntity> claims)
        {
            if (claims == null)
                return [];

            return claims
                .Select(x => new FileClaim
                {
                    ObjectIdentifier = x.ObjectIdentifier,
                    ObjectType = x.ObjectType.ToEnum(FileClaimObjectType.Person)
                })
                .ToList();
        }
    }
}
