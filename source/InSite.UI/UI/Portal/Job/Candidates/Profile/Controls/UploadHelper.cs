using System;
using System.Collections.Generic;
using System.IO;

using InSite.Application.Files.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public static class UploadHelper
    {
        public static void SaveContactUploads(Guid userId, string type, string filename, IEnumerable<FileUploadV2> uploads)
        {
            var existentEntities = TCandidateUploadSearch.Select(x => x.CandidateUserIdentifier == userId && x.UploadType == type, "UploadName, UploadIdentifier");
            var newEntities = new List<TCandidateUpload>();
            var index = 0;
            var number = 1;

            foreach (var upload in uploads)
            {
                if (upload.HasFile)
                {
                    var model = SaveUploadedFile(userId, string.Format(filename, number++), upload);

                    newEntities.Add(new TCandidateUpload
                    {
                        UploadMime = model.ContentType,
                        UploadSize = model.FileSize,
                        UploadName = model.Properties.DocumentName,
                        FileIdentifier = model.FileIdentifier
                    });
                }
                else if (index < existentEntities.Count)
                {
                    newEntities.Add(existentEntities[index]);
                    number++;
                }

                index++;
            }

            TCandidateUploadStore.Update(userId, type, newEntities);
        }

        public static string GetFileRelativePath(Guid fileId)
        {
            var model = ServiceLocator.StorageService.GetFile(fileId);
            return model != null ? ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName) : null;
        }

        public static void SaveContactUpload(Guid userId, string type, string fileName, FileUploadV2 upload)
        {
            if (!upload.HasFile)
                return;

            var model = SaveUploadedFile(userId, fileName, upload);

            TCandidateUploadStore.Update(userId, type, new TCandidateUpload
            {
                UploadMime = model.ContentType,
                UploadSize = model.FileSize,
                UploadName = model.Properties.DocumentName,
                FileIdentifier = model.FileIdentifier
            });
        }

        public static string GetFilePhysicalPath(Guid candidate, string organizationCode, string fileName)
        {
            var folder = ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Tenants", organizationCode, "Contacts", "People", candidate.ToString(), "Attachments");
            return Path.Combine(folder, fileName);
        }

        private static FileStorageModel SaveUploadedFile(Guid contactId, string fileName, FileUploadV2 upload)
        {
            var fileNameWithExt = fileName + Path.GetExtension(upload.FileName);

            var existingList = ServiceLocator.StorageService.GetGrantedFiles(CurrentSessionState.Identity, contactId, fileNameWithExt);
            foreach (var existing in existingList)
                ServiceLocator.StorageService.Delete(existing.FileIdentifier);

            var model = upload.SaveFile(contactId, FileObjectType.User, fileNameWithExt);

            upload.ClearFiles();

            return model;
        }
    }
}