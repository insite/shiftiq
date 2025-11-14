using System;

namespace InSite.Admin.Jobs.Candidates
{
    public class UploadHelper
    {
        public static string GetFileRelativePath(Guid fileId)
        {
            var model = ServiceLocator.StorageService.GetFile(fileId);
            return model != null ? ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName) : null;
        }
    }
}