using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InSite.Application.Files.Read
{
    public interface IFileStore
    {
        FileStorageModel InsertModel(FileStorageModel model);

        void UpdateObject(Guid fileIdentifier, Guid objectIdentifier, FileObjectType objectType);

        void RenameFile(Guid fileIdentifier, string newFileName);

        FileLastActivity UpdateProperties(Guid fileIdentifier, Guid userIdentifier, FileProperties properties, bool updateActivityChanges);

        void UpdateClaims(Guid fileIdentifier, IEnumerable<FileClaim> claims);

        void UpdateFileUploaded(Guid fileIdentifier, DateTimeOffset value);

        void Delete(Guid fileIdentifier);
    }

    public interface IFileStoreAsync
    {
        Task<FileStorageModel> InsertModelAsync(FileStorageModel model);

        Task UpdateObjectAsync(Guid fileIdentifier, Guid objectIdentifier, FileObjectType objectType);

        Task RenameFileAsync(Guid fileIdentifier, string newFileName);

        Task<FileLastActivity> UpdatePropertiesAsync(Guid fileIdentifier, Guid userIdentifier, FileProperties properties, bool updateActivityChanges);

        Task UpdateClaimsAsync(Guid fileIdentifier, IEnumerable<FileClaim> claims);

        Task UpdateFileUploadedAsync(Guid fileIdentifier, DateTimeOffset value);

        Task DeleteAsync(Guid fileIdentifier);
    }
}
