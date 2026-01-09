using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Files.Read;

using Shift.Common;
using Shift.Toolbox;

namespace InSite.Persistence
{
    public class TFileSearch : IFileSearch
    {
        private readonly IJsonSerializer _jsonSerializer;

        public TFileSearch(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public FileStorageModel GetModel(Guid fileIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var file = db.TFiles
                    .AsNoTracking()
                    .Where(x => x.FileIdentifier == fileIdentifier)
                    .FirstOrDefault();

                if (file == null)
                    return null;

                var claims = db.TFileClaims
                    .AsNoTracking()
                    .Where(x => x.FileIdentifier == fileIdentifier)
                    .ToList();

                var model = CreateModel(file);

                model.Claims = CreateClaims(claims);

                return model;
            }
        }

        public List<FileStorageModel> GetModels(Guid? organizationIdentifier, Guid objectIdentifier, string documentName, bool includeClaims)
        {
            return GetModels(organizationIdentifier, new[] { objectIdentifier }, documentName, includeClaims);
        }

        public List<FileStorageModel> GetModels(Guid? organizationIdentifier, Guid[] objectIdentifiers, string documentName, bool includeClaims)
        {
            List<TFile> files;

            using (var db = new InternalDbContext())
            {
                var query = db.TFiles
                    .AsNoTracking()
                    .Where(x => objectIdentifiers.Any(id => id == x.ObjectIdentifier));

                if (organizationIdentifier.HasValue)
                    query = query.Where(x => x.OrganizationIdentifier == organizationIdentifier);

                if (!string.IsNullOrEmpty(documentName))
                    query = query.Where(x => x.DocumentName == documentName);

                if (includeClaims)
                    query = query.Include(x => x.FileClaims);

                files = query.ToList();
            }

            return EntitiesToModels(files, includeClaims);
        }

        public List<FileStorageModel> GetExpiredModels(DateTimeOffset expiredAt)
        {
            List<TFile> files;

            var objectType = FileObjectType.Temporary.ToString();

            using (var db = new InternalDbContext())
            {
                files = db.TFiles
                    .AsNoTracking()
                    .Where(x => x.ObjectType == objectType && x.FileUploaded <= expiredAt)
                    .ToList();
            }

            return EntitiesToModels(files, false);
        }

        public List<FileActivity> GetFileActivities(Guid fileIdentifier)
        {
            List<TFileActivity> activities;

            using (var db = new InternalDbContext())
            {
                activities = db.TFileActivities
                    .AsNoTracking()
                    .Where(x => x.FileIdentifier == fileIdentifier)
                    .ToList();
            }

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

        public List<OrphanFile> GetOrphanFiles()
        {
            using (var db = new InternalDbContext())
            {
                return db.VOrphanFiles.ToList();
            }
        }

        private static List<FileStorageModel> EntitiesToModels(List<TFile> files, bool includeClaims)
        {
            var result = new List<FileStorageModel>();

            foreach (var file in files)
            {
                var model = CreateModel(file);

                if (includeClaims)
                    model.Claims = CreateClaims(file.FileClaims);

                result.Add(model);
            }

            return result;
        }

        internal static FileStorageModel CreateModel(TFile file)
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
                AllowLearnerToView = file.AllowLearnerToView,
            };

            model.Properties = properties;

            return model;
        }

        internal static List<FileClaim> CreateClaims(IEnumerable<TFileClaim> claims)
        {
            if (claims == null)
                return null;

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
