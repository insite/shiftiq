using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InSite.Application.Files.Read
{
    public interface IFileSearch
    {
        FileStorageModel GetModel(Guid fileIdentifier);

        List<FileStorageModel> GetModels(Guid? organizationIdentifier, Guid objectIdentifier, string documentName, bool includeClaims);

        List<FileStorageModel> GetModels(Guid? organizationIdentifier, Guid[] objectIdentifiers, string documentName, bool includeClaims);

        List<FileStorageModel> GetExpiredModels(DateTimeOffset expiredAt);

        List<FileActivity> GetFileActivities(Guid fileIdentifier);

        List<OrphanFile> GetOrphanFiles();
    }

    public interface IFileSearchAsync
    {
        Task<FileStorageModel> GetModelAsync(Guid fileIdentifier);

        Task<List<FileStorageModel>> GetModelsAsync(Guid? organizationIdentifier, Guid objectIdentifier, string documentName, bool includeClaims);

        Task<List<FileStorageModel>> GetModelsAsync(Guid? organizationIdentifier, Guid[] objectIdentifiers, string documentName, bool includeClaims);

        Task<List<FileStorageModel>> GetExpiredModelsAsync(DateTimeOffset expiredAt);

        Task<List<FileActivity>> GetFileActivitiesAsync(Guid fileIdentifier);

        Task<List<OrphanFile>> GetOrphanFilesAsync();
    }
}
