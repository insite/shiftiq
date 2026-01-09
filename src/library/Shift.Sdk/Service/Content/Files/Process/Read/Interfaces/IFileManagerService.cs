using System;
using System.IO;
using System.Threading.Tasks;

namespace InSite.Application.Files.Read
{
    public interface IFileManagerService
    {
        bool IsFileExist(Guid organizationIdentifier, Guid fileIdentifier, string fileName, string filePath);

        int SaveFile(Guid organizationIdentifier, Guid fileIdentifier, string fileName, string filePath, Stream file);

        void DeleteFile(Guid organizationIdentifier, Guid fileIdentifier, string fileName);

        void RenameFile(Guid organizationIdentifier, Guid fileIdentifier, string oldFileName, string newFileName);

        Stream ReadFileStream(Guid organizationIdentifier, Guid fileIdentifier, string fileName, string filePath);

        int GetMaxFileNameLength();
    }

    public interface IFileManagerServiceAsync
    {
        bool IsFileExist(Guid organizationIdentifier, Guid fileIdentifier, string fileName, string filePath);

        Task<int> SaveFileAsync(Guid organizationIdentifier, Guid fileIdentifier, string fileName, string filePath, Stream file);

        void DeleteFile(Guid organizationIdentifier, Guid fileIdentifier, string fileName);

        void RenameFile(Guid organizationIdentifier, Guid fileIdentifier, string oldFileName, string newFileName);

        Task<Stream> ReadFileStreamAsync(Guid organizationIdentifier, Guid fileIdentifier, string fileName, string filePath);

        int GetMaxFileNameLength();
    }
}
