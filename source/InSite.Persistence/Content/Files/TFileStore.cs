using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Files.Read;

using Shift.Common;

using Shift.Toolbox;

namespace InSite.Persistence
{
    public class TFileStore : IFileStore
    {
        private readonly IFileChangeFactory _fileChangeFactory;
        private readonly IJsonSerializer _jsonSerializer;

        public TFileStore(IFileChangeFactory fileChangeFactory, IJsonSerializer jsonSerializer)
        {
            _fileChangeFactory = fileChangeFactory;
            _jsonSerializer = jsonSerializer;
        }

        public FileStorageModel InsertModel(FileStorageModel model)
        {
            TFile file;
            List<TFileClaim> claims;

            using (var db = new InternalDbContext())
            {
                file = CreateFile(model);

                db.TFiles.Add(file);

                claims = CreateClaimList(model.FileIdentifier, model.Claims);
                if (claims != null)
                    db.TFileClaims.AddRange(claims);

                db.SaveChanges();
            }

            var newModel = TFileSearch.CreateModel(file);
            newModel.Claims = TFileSearch.CreateClaims(claims);

            return newModel;
        }

        public void UpdateObject(Guid fileIdentifier, Guid objectIdentifier, FileObjectType objectType)
        {
            using (var db = new InternalDbContext())
            {
                var file = db.TFiles
                    .Where(x => x.FileIdentifier == fileIdentifier)
                    .FirstOrDefault();

                if (file == null)
                    throw new ArgumentException($"File with ID = {fileIdentifier} is not found");

                file.ObjectIdentifier = objectIdentifier;
                file.ObjectType = objectType.ToString();

                db.SaveChanges();
            }
        }

        public void RenameFile(Guid fileIdentifier, string newFileName)
        {
            using (var db = new InternalDbContext())
            {
                var file = db.TFiles
                    .Where(x => x.FileIdentifier == fileIdentifier)
                    .FirstOrDefault();

                if (file == null)
                    throw new ArgumentException($"File with ID = {fileIdentifier} is not found");

                file.FileName = newFileName;

                db.SaveChanges();
            }
        }

        public FileLastActivity UpdateProperties(Guid fileIdentifier, Guid userIdentifier, FileProperties properties, bool updateActivityChanges)
        {
            if (properties == null)
                throw new ArgumentNullException("Properties");

            FileStorageModel oldModel, newModel;

            using (var db = new InternalDbContext())
            {
                var file = db.TFiles
                    .Where(x => x.FileIdentifier == fileIdentifier)
                    .FirstOrDefault();

                if (file == null)
                    throw new ArgumentException($"File with ID = {fileIdentifier} is not found");

                oldModel = TFileSearch.CreateModel(file);

                CopyProperties(file, properties, userIdentifier);

                newModel = TFileSearch.CreateModel(file);

                db.SaveChanges();
            }

            if (updateActivityChanges)
                SaveChangeHistory(oldModel, newModel, userIdentifier);

            return new FileLastActivity
            {
                LastActivityTime = newModel.LastActivityTime,
                LastActivityUserIdentifier = newModel.LastActivityUserIdentifier
            };
        }

        public void UpdateClaims(Guid fileIdentifier, IEnumerable<FileClaim> claims)
        {
            var newClaims = CreateClaimList(fileIdentifier, claims);

            using (var db = new InternalDbContext())
            {
                var existingClaims = db.TFileClaims
                    .Where(x => x.FileIdentifier == fileIdentifier)
                    .ToList();

                foreach (var existingClaim in existingClaims)
                {
                    if (!newClaims.Any(x => x.ObjectIdentifier == existingClaim.ObjectIdentifier && x.ObjectType == existingClaim.ObjectType))
                        db.TFileClaims.Remove(existingClaim);
                }

                foreach (var newClaim in newClaims)
                {
                    if (!existingClaims.Any(x => x.ObjectIdentifier == newClaim.ObjectIdentifier && x.ObjectType == newClaim.ObjectType))
                        db.TFileClaims.Add(newClaim);
                }

                db.SaveChanges();
            }
        }

        public void Delete(Guid fileIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var file = db.TFiles
                    .Where(x => x.FileIdentifier == fileIdentifier)
                    .FirstOrDefault();

                if (file == null)
                    return;

                var claims = db.TFileClaims
                    .Where(x => x.FileIdentifier == fileIdentifier)
                    .ToList();

                var activities = db.TFileActivities
                    .Where(x => x.FileIdentifier == fileIdentifier)
                    .ToList();

                db.TFileClaims.RemoveRange(claims);
                db.TFileActivities.RemoveRange(activities);
                db.TFiles.Remove(file);

                db.SaveChanges();
            }
        }

        public void UpdateFileUploaded(Guid fileIdentifier, DateTimeOffset value)
        {
            using (var db = new InternalDbContext())
            {
                var file = db.TFiles
                    .Where(x => x.FileIdentifier == fileIdentifier)
                    .FirstOrDefault();

                file.FileUploaded = value;

                db.SaveChanges();
            }
        }

        private static TFile CreateFile(FileStorageModel model)
        {
            var file = new TFile
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

        private static void CopyProperties(TFile file, FileProperties properties, Guid userIdentifier)
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

        private void SaveChangeHistory(FileStorageModel oldModel, FileStorageModel newModel, Guid userIdentifier)
        {
            var changes = _fileChangeFactory.CreateChanges(oldModel, newModel);
            if (changes.Length == 0)
                return;

            var jsonChanges = _jsonSerializer.Serialize(changes);

            var activity = new TFileActivity
            {
                FileIdentifier = oldModel.FileIdentifier,
                UserIdentifier = userIdentifier,
                ActivityIdentifier = UniqueIdentifier.Create(),
                ActivityTime = DateTimeOffset.UtcNow,
                ActivityChanges = jsonChanges
            };

            using (var db = new InternalDbContext())
            {
                db.TFileActivities.Add(activity);
                db.SaveChanges();
            }
        }

        private static List<TFileClaim> CreateClaimList(Guid fileIdentifier, IEnumerable<FileClaim> claims)
        {
            if (claims == null)
                return new List<TFileClaim>();

            return claims
                .Select(x => new TFileClaim
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
