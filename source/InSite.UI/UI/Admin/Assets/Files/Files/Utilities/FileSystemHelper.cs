using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Web;

using InSite.UI;

using Shift.Common;

namespace InSite.Admin.Assets.Files.Files.Utilities
{
    public static class FileSystemHelper
    {
        public static string MoveDirectory(string pSource, string pDest, string vSource, string vDest)
        {
            try
            {
                if ((PathHelper.IsSharedPath(pSource) && PathHelper.IsSharedPath(pDest)) || (PathHelper.IsPhysicalPath(pSource) && PathHelper.IsPhysicalPath(pDest)))
                {
                    Directory.Move(pSource, pDest);
                }
                else
                {
                    var vDest2 = VirtualPathUtility.GetDirectory(PathHelper.AddStartingSlash(vDest, '/'));

                    vDest2 = PathHelper.RemoveStartingSlash(vDest2, '/');
                    vDest2 = PathHelper.RemoveEndingSlash(vDest2, '/');

                    CopyDirectory(
                        pSource,
                        GetDirectoryName(pDest),
                        vSource,
                        vDest2);
                    DeleteDirectory(pSource, vSource);
                }
            }
            catch (DirectoryNotFoundException)
            {
                return $"One of the directories: '{vSource}' or '{vDest}' does not exist!";
            }
            catch (UnauthorizedAccessException)
            {
                return "You do not have enough permissions for this operation!";
            }
            catch (Exception)
            {
                return "The operation cannot be compleated";
            }

            return string.Empty;
        }

        public static string MoveFile(string pSource, string pDest, string vSource, string vDest)
        {
            try
            {
                File.Move(pSource, pDest);
            }
            catch (FileNotFoundException)
            {
                return $"File: '{vSource}' does not exist!";
            }
            catch (UnauthorizedAccessException)
            {
                return "FileSystem's restriction: You do not have enough permissions for this operation!";
            }
            catch (IOException)
            {
                return "The operation cannot be compleated";
            }

            return string.Empty;
        }

        public static string DeleteDirectory(string pPath, string vPath)
        {
            try
            {
                Directory.Delete(pPath, true);
            }
            catch (DirectoryNotFoundException)
            {
                return $"FileSystem restriction: Directory '{vPath}' is not found!";
            }
            catch (UnauthorizedAccessException)
            {
                return $"FileSystem's restriction: You do not have enough permissions for this operation!";
            }
            catch (IOException)
            {
                return $"FileSystem restriction: The directory '{vPath}' cannot be deleted!";
            }

            return string.Empty;
        }

        public static string DeleteFile(string pPath, string vPath)
        {
            try
            {
                File.Delete(pPath);
            }
            catch (FileNotFoundException)
            {
                return $"File: '{vPath}' does not exist!";
            }
            catch (UnauthorizedAccessException)
            {
                return $"FileSystem restriction: You do not have enough permissions for this operation!";
            }
            catch (IOException)
            {
                return "The operation cannot be compleated";
            }

            return string.Empty;
        }

        public static string CopyFile(string pSource, string pDest, string vSource, string vDest)
        {
            try
            {
                File.Copy(pSource, pDest, true);
            }

            catch (FileNotFoundException)
            {
                return $"File: '{vSource}' does not exist!";
            }
            catch (UnauthorizedAccessException)
            {
                return $"FileSystem's restriction: You do not have enough permissions for this operation!";
            }
            catch (IOException)
            {
                return $"The operation cannot be compleated";
            }

            return string.Empty;
        }

        public static string CopyDirectory(string pSource, string pDest, string vSource, string vDest)
        {
            DirectoryInfo dirSourceInfo;
            string dirSourcePath;

            try
            {
                dirSourceInfo = new DirectoryInfo(pSource);
                dirSourcePath = PathHelper.AddEndingSlash(pDest, '\\') + dirSourceInfo.Name + '\\';

                Directory.CreateDirectory(dirSourcePath, dirSourceInfo.GetAccessControl());
            }
            catch (UnauthorizedAccessException)
            {
                return "FileSystem's restriction: You do not have enough permissions for this operation!";
            }

            foreach (var fPath in Directory.GetFiles(pSource))
            {
                var fInfo = new FileInfo(fPath);

                try
                {
                    File.Copy(fPath, dirSourcePath + fInfo.Name);
                }
                catch (FileNotFoundException)
                {
                    return $"File: '{vSource}' does not exist!";
                }
                catch (UnauthorizedAccessException)
                {
                    return "You do not have enough permissions for this operation!";
                }
                catch (IOException)
                {
                    return "The operation cannot be compleated";
                }
            }

            foreach (var dPath in Directory.GetDirectories(pSource))
            {
                var dInfo = new DirectoryInfo(dPath);
                var error = CopyDirectory(
                    dPath,
                    dPath,
                    PathHelper.AddEndingSlash(vSource, '/') + dInfo.Name + "/",
                    PathHelper.AddEndingSlash(vDest, '/') + dirSourceInfo.Name + "/");

                if (error.IsNotEmpty())
                    return error;
            }

            return string.Empty;
        }

        public static string CreateDirectory(string pPath, string dirName, string vPath)
        {
            try
            {
                //var dirInfo = new DirectoryInfo(pPath);

                Directory.CreateDirectory(
                    PathHelper.AddEndingSlash(pPath, '\\') + dirName/*,
                    dirInfo.GetAccessControl()*/);
            }
            catch (DirectoryNotFoundException)
            {
                return $"FileSystem restriction: Directory with name '{vPath}' is not found!";
            }
            catch (UnauthorizedAccessException)
            {
                return "FileSystem's restriction: You do not have enough permissions for this operation!";
            }
            catch (IOException)
            {
                return $"FileSystem restriction: The directory '{dirName}' cannot be created!";
            }

            return string.Empty;
        }

        public static byte[] GetFileContent(string pPath, string vPath)
        {
            using (var fileStream = new FileStream(pPath, FileMode.Open, FileAccess.Read))
            {
                var content = new byte[fileStream.Length];
                fileStream.Read(content, 0, (int)fileStream.Length);
                fileStream.Close();
                return content;
            }
        }

        private static string GetDirectoryName(string path)
        {
            if (path != null)
            {
                if (path.EndsWith("\\"))
                {
                    var lastIndex = path.Length - 1;
                    if (lastIndex > 0)
                    {
                        var startIndex = path.LastIndexOf('\\', lastIndex - 1) + 1;
                        return path.Substring(startIndex, lastIndex - startIndex);
                    }
                }
                else if (path.Length > 0)
                {
                    var slashIndex = path.LastIndexOf("\\");
                    return path.Substring(slashIndex + 1);
                }
            }

            return string.Empty;
        }

        public static bool CheckWritePermission(string pPath, string vPath)
        {
            var f = new FileIOPermission(FileIOPermissionAccess.Write, pPath);

            try
            {
                f.Demand();

                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }
    }
}