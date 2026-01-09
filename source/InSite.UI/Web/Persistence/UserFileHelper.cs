using System;
using System.IO;

using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.Setup;

using Shift.Constant;

namespace InSite.Common
{
    internal static class UserFileHelper
    {
        #region Properties

        internal static FileProvider Provider => FileHelper.Provider;
        internal static UserUploadManager UserUpload => FileHelper.UserUpload;

        #endregion

        #region Public methods

        public static string SaveFile(Guid organizationId, string user, string name, Stream stream)
        {
            var path = GetFilePath(user, name);

            Provider.Save(organizationId, path, stream);

            return FileHelper.GetUrl(path);
        }

        public static string SaveFile(Guid organizationId, string user, string name, byte[] data)
        {
            var path = GetFilePath(user, name);

            Provider.Save(organizationId, path, data);

            return FileHelper.GetUrl(path);
        }

        public static string SaveFile(Guid organizationId, string user, string directory, string name, Stream stream)
        {
            var path = GetFilePath(user, directory, name);

            Provider.Save(organizationId, path, stream);

            return FileHelper.GetUrl(path);
        }

        public static string ReplaceNonAlphanumericCharacters(string value, char separator, bool makeLowercase = true)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                if (makeLowercase)
                {
                    foreach (char ch in value.ToLower())
                        result += ch >= 'a' && ch <= 'z' || ch >= '0' && ch <= '9' ? ch : separator;
                }
                else
                {
                    foreach (char ch in value)
                        result += ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z' || ch >= '0' && ch <= '9' ? ch : separator;
                }

                string strDoubleSeparator = string.Concat(separator, separator);
                string strSeparator = string.Empty + separator;

                while (result.Contains(strDoubleSeparator))
                    result = result.Replace(strDoubleSeparator, strSeparator);

                if (result.StartsWith(strSeparator) && result.EndsWith(strSeparator))
                    result = result.Length == 1 ? null : result.Substring(1, result.Length - 2);
                else if (result.StartsWith(strSeparator))
                    result = result.Substring(1);
                else if (result.EndsWith(strSeparator))
                    result = result.Substring(0, result.Length - 1);
            }

            return result;
        }

        #endregion

        #region Helpers

        private static string GetFileName(string input) =>
            ReplaceNonAlphanumericCharacters(Path.GetFileNameWithoutExtension(input), '_', false) + Path.GetExtension(input);

        private static string GetFilePath(string user, string name) =>
            string.Format(OrganizationRelativePath.UserPathTemplate, user) + GetFileName(name);

        private static string GetFilePath(string user, string directory, string name) =>
            string.Format(OrganizationRelativePath.UserPathTemplate, user) + directory + "/" + GetFileName(name);

        #endregion
    }
}