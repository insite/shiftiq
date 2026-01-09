using System.IO;

namespace Shift.Common
{
    public class FilePaths
    {
        private readonly string _dataFolderShare;
        private readonly string _dataFolderEnterprise;

        public FilePaths(string dataFolderShare, string dataFolderEnterprise)
        {
            _dataFolderShare = dataFolderShare;
            _dataFolderEnterprise = dataFolderEnterprise;
        }


        public string GetPhysicalPathToShareFolder(params string[] args)
        {
            var path = GetPhysicalPathToShareFile(args);
            if (!Directory.Exists(path) && !System.IO.File.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public string GetPhysicalPathToEnterpriseFolder(params string[] args)
        {
            var path = GetPhysicalPathToEnterpriseFile(args);
            if (!Directory.Exists(path) && !System.IO.File.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        private string GetPhysicalPathToShareFile(params string[] args)
        {
            return Path.Combine(_dataFolderShare, Path.Combine(args));
        }

        public string GetPhysicalPathToEnterpriseFile(params string[] args)
        {
            return Path.Combine(_dataFolderEnterprise, Path.Combine(args));
        }

        public string GetPhysicalPathToTempFile(params string[] args)
        {
            return Path.Combine(TempFolderPath, Path.Combine(args));
        }

        public string CreateLogFilePath(string folder, string file)
        {
            var path = GetPhysicalPathToEnterpriseFolder("Logs", folder);
            return Path.Combine(path, file);
        }

        public string ApplicationPasswordResetsPendingPath => GetPhysicalPathToShareFolder("Files", "Temp", "Passwords", "PendingReset");
        public string CmdsFilePath => GetPhysicalPathToShareFolder("Files", "Uploads");
        public string FileStoragePath => GetPhysicalPathToShareFolder("Files");
        public string TempFolderPath => GetPhysicalPathToShareFolder("Files", "Temp");
    }
}
