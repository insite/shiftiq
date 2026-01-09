using InSite.Application.Files.Read;

using Shift.Common;

using ioDirectory = System.IO.Directory;

namespace Shift.Service.Content
{
    public class FileManagerService : IFileManagerServiceAsync
    {
        private const int MaxPathLength = 259;
        private const int DeleteTryCount = 3;
        private const int DeleteTryDelayInMs = 100;

        private readonly FilePaths _filePaths;

        public FileManagerService(FilePaths filePaths)
        {
            _filePaths = filePaths;
        }

        public bool IsFileExist(Guid organizationIdentifier, Guid fileIdentifier, string fileName, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                filePath = GetFilePath(organizationIdentifier, fileIdentifier, fileName);

            return File.Exists(filePath);
        }

        public async Task<int> SaveFileAsync(Guid organizationIdentifier, Guid fileIdentifier, string fileName, string? filePath, Stream file)
        {
            var (isSavedPreviously, existingFileSize) = CheckSavedPreviously(filePath, file);

            if (isSavedPreviously)
                return existingFileSize;

            var newFilePath = GetFilePath(organizationIdentifier, fileIdentifier, fileName);

            var folder = Path.GetDirectoryName(newFilePath)
                ?? throw new ApplicationError($"Failed to determine a valid directory for file path: {newFilePath}");

            int newFileSize;

            try
            {
                if (!ioDirectory.Exists(folder))
                    ioDirectory.CreateDirectory(folder);

                if (File.Exists(newFilePath))
                    File.Delete(newFilePath);

                using (var stream = new FileStream(newFilePath, FileMode.CreateNew))
                    await file.CopyToAsync(stream);

                newFileSize = (int)new FileInfo(newFilePath).Length;
            }
            catch (Exception ex)
            {
                throw new ApplicationError($"The error for the file path: '{newFilePath}'", ex);
            }

            if (newFileSize > 0)
                return newFileSize;

            File.Delete(newFilePath);

            throw new ApplicationError("File cannot be empty");
        }

        private (bool, int) CheckSavedPreviously(string? filePath, Stream file)
        {
            // If there is no input stream then this means the file must remain in its current location, and the system
            // is expected to store enough information to locate it when it is later requested.

            if (file == null && !string.IsNullOrEmpty(filePath))
            {
                var path = new FilePath(filePath);
                if (path.PathType == FilePathType.LocalFile || path.PathType == FilePathType.RemoteFile)
                    return (true, (int)new FileInfo(filePath).Length);

                // If the path is not a local file or a remote file then its size is more difficult to determine. For
                // now, return -1 as a sentinel value to represent an unknown file size.

                return (true, -1);
            }

            if (file == null)
                throw new ArgumentException("File stream is null but file path is not specified");

            if (!string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Both file stream and file path are specified");

            return (false, 0);
        }

        public void DeleteFile(Guid organizationIdentifier, Guid fileIdentifier, string fileName)
        {
            var filePath = GetFilePath(organizationIdentifier, fileIdentifier, fileName);

            var folder = Path.GetDirectoryName(filePath);

            if (!ioDirectory.Exists(folder))
                return;

            var tryCount = DeleteTryCount;

            for (; ; )
            {
                try
                {
                    ioDirectory.Delete(folder, true);
                    break;
                }
                catch (IOException)
                {
                    tryCount--;

                    if (tryCount <= 0)
                        throw;

                    Thread.Sleep(DeleteTryDelayInMs);
                }
            }
        }

        public void RenameFile(Guid organizationIdentifier, Guid fileIdentifier, string oldFileName, string newFileName)
        {
            var oldFilePath = GetFilePath(organizationIdentifier, fileIdentifier, oldFileName);

            var newFilePath = GetFilePath(organizationIdentifier, fileIdentifier, newFileName);

            File.Move(oldFilePath, newFilePath);
        }

        /// <summary>
        /// Retrieve an IO stream to the requested file.
        /// </summary>
        /// <remarks>
        /// Determine what type of file path this is and open a stream to it. If the path refers to a local file or a 
        /// remote (UNC) file then proceed normally. If the path is an absolute URL then use an HTTP client to obtain a 
        /// stream to the file. For now, we won't validate the URL or implement support for relative URL paths.
        /// </remarks>>
        public async Task<Stream> ReadFileStreamAsync(Guid organizationIdentifier, Guid fileIdentifier, string fileName, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                filePath = GetFilePath(organizationIdentifier, fileIdentifier, fileName);

            try
            {
                var path = new FilePath(filePath);

                if (path.PathType == FilePathType.AbsoluteUrl)
                    return await StaticHttpClient.Client.GetStreamAsync(filePath);

                // Here the code assumes PathType == LocalFile || RemoteFile. I don't know if this is a safe 
                // assumption, but I am leaving it "as is" to ensure existing functionality does not change.
                return new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                throw new ReadFileStreamFailedException(filePath, ex);
            }
        }

        public int GetMaxFileNameLength()
        {
            var folderPath = GetFilePath(Guid.Empty, Guid.Empty, "");
            return MaxPathLength - folderPath.Length - 1;
        }

        public string GetFilePath(Guid organizationIdentifier, Guid fileIdentifier, string fileName)
        {
            return _filePaths.GetPhysicalPathToEnterpriseFile(
                "Assets",
                "Files",
                "Organizations",
                organizationIdentifier.ToString(),
                fileIdentifier.ToString(),
                fileName
            );
        }
    }
}
