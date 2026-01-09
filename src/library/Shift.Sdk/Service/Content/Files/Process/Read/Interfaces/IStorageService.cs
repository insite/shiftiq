using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Shift.Common;

namespace InSite.Application.Files.Read
{
    public interface IStorageService
    {
        string GetFileUrl(FileStorageModel model, bool download = false);
        string GetFileUrl(Guid fileIdentifier, string fileName, bool download = false);

        (Guid? FileIdentifier, string FileName) ParseFileUrl(string fileUrl);

        List<(Guid FileIdentifier, string FileName)> ParseSurveyResponseAnswer(string responseAnswerText);

        bool IsRemoteFilePathValid(FileStorageModel file);

        FileStorageModel Create(
            Stream file,
            string fileName,
            Guid organizationIdentifier,
            Guid actorUserIdentifier,
            Guid objectIdentifier,
            FileObjectType objectType,
            FileProperties props,
            IEnumerable<FileClaim> claims,
            FileLocation fileLocation = FileLocation.Local,
            string filePath = null
        );

        void ChangeObject(Guid fileIdentifier, Guid objectIdentifier, FileObjectType objectType);

        void RenameFile(Guid fileIdentifier, Guid userIdentifier, string newFileName);

        void ChangeProperties(Guid fileIdentifier, Guid changedByUserIdentifier, FileProperties properties, bool updateActivityChanges);

        void ChangeClaims(Guid fileIdentifier, IEnumerable<FileClaim> claims);

        void Delete(Guid fileIdentifier);

        void DeleteOnlyRecord(Guid fileIdentifier);

        int DeleteExpiredFiles();

        FileStorageModel GetFile(Guid fileIdentifier);

        bool IsFileExist(Guid fileIdentifier);

        (FileStorageModel, Stream) GetFileStream(Guid fileIdentifier);

        FileGrantStatus GetGrantStatus(ISimplePrincipal identity, Guid fileIdentifier);

        (FileGrantStatus, FileStorageModel) GetFileAndAuthorize(ISimplePrincipal identity, Guid fileIdentifier);

        (FileGrantStatus, FileStorageModel, Stream) GetFileStreamAndAuthorize(ISimplePrincipal identity, Guid fileIdentifier);

        List<FileStorageModel> GetGrantedFiles(ISimplePrincipal identity, Guid objectIdentifier, string documentName = null);

        List<FileStorageModel> GetGrantedFiles(ISimplePrincipal identity, Guid[] objectIdentifiers, string documentName = null);

        void ClearCache();

        string AdjustFileName(string fileName);
    }

    public interface IStorageServiceAsync
    {
        Task ChangeClaimsAsync(Guid fileIdentifier, IEnumerable<FileClaim> claims);

        Task ChangeObjectAsync(Guid fileIdentifier, Guid objectIdentifier, FileObjectType objectType);

        Task ChangePropertiesAsync(Guid fileIdentifier, Guid userIdentifier, FileProperties properties, bool updateActivityChanges);

        void ClearCache();

        Task<FileStorageModel> CreateAsync(
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
            );

        Task DeleteAsync(Guid fileIdentifier);

        Task DeleteOnlyRecordAsync(Guid fileIdentifier);

        Task<int> DeleteExpiredFilesAsync();

        Task<FileStorageModel> GetFileAsync(Guid fileIdentifier);

        Task<(FileGrantStatus, FileStorageModel)> GetFileAndAuthorizeAsync(ISimplePrincipal identity, Guid fileIdentifier);

        Task<(FileStorageModel, Stream)> GetFileStreamAsync(Guid fileIdentifier);

        Task<(FileGrantStatus, FileStorageModel, Stream)> GetFileStreamAndAuthorizeAsync(ISimplePrincipal identity, Guid fileIdentifier);

        string GetFileUrl(FileStorageModel model, bool download = false);

        string GetFileUrl(Guid fileIdentifier, string fileName, bool download = false);

        Task<List<FileStorageModel>> GetGrantedFilesAsync(ISimplePrincipal identity, Guid objectIdentifier, string documentName = null);

        Task<List<FileStorageModel>> GetGrantedFilesAsync(ISimplePrincipal identity, Guid[] objectIdentifiers, string documentName = null);

        Task<FileGrantStatus> GetGrantStatusAsync(ISimplePrincipal identity, Guid fileIdentifier);

        bool IsRemoteFilePathValid(FileStorageModel file);

        (Guid? FileIdentifier, string FileName) ParseFileUrl(string fileUrl);

        Task RenameFileAsync(Guid fileIdentifier, Guid userIdentifier, string newFileName);
    }
}
