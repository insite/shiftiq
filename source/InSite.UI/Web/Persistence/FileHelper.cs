using System;
using System.Text;

using InSite.Common.Web.Setup;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.Infrastructure
{
    public static class FileHelper
    {
        #region Properties

        public static FileProvider Provider { get; }

        public static UserUploadManager UserUpload { get; }

        #endregion

        #region Construction

        static FileHelper()
        {
            Provider = new FileProvider(
                System.IO.Path.Combine(ServiceLocator.FilePaths.FileStoragePath, "Uploads"),
                () => CurrentSessionState.Identity?.User?.FullName ?? UserNames.Someone,
                () => CurrentSessionState.Identity?.User?.UserIdentifier ?? Guid.Empty
            );
            UserUpload = new UserUploadManager(Provider);
        }

        #endregion

        public static string AdjustFileName(string fileName, bool toLower = true, bool allowDot = true)
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName;

            if (toLower)
                fileName = fileName.ToLower();

            var result = new StringBuilder();

            foreach (char c in fileName)
            {
                if (char.IsLetterOrDigit(c) || c == '-')
                    result.Append(c);
                else if (c == '.' && allowDot)
                    result.Append(c);
                else if (char.IsWhiteSpace(c) || c == '_')
                    result.Append('-');
            }

            return result.ToString();
        }

        public static bool IsStandardFileName(string fileName)
        {
            foreach (char c in fileName)
            {
                if (!char.IsLetterOrDigit(c) && c != '.' && c != '-')
                    return false;
            }

            return true;
        }

        public static bool IsFileExists(string url)
        {
            return HttpRequestHelper.IsUrlFilePathValid(url);
        }

        public static string GetUrl(string path) => HttpRequestHelper.CurrentRootUrlFiles + path;

        public static FileDescriptor GetDescriptor(string url)
        {
            var uri = new Uri(url);
            if (!uri.PathAndQuery.StartsWith(RelativeUrl.FileAppSubfolder, StringComparison.OrdinalIgnoreCase))
                return null;

            var organizationCode = UrlHelper.GetOrganizationCode(uri).EmptyIfNull();
            var organization = OrganizationSearch.Select(organizationCode);

            if (organization == null)
                return null;

            var path = uri.LocalPath.Substring(RelativeUrl.FileAppSubfolder.Length);

            return Provider.GetDescriptor(organization.Identifier, path);
        }

        public static string GetSurveyUploadPath(int key, string subtype)
        {
            string p1;

            if (string.Equals(subtype, "Survey Form", StringComparison.OrdinalIgnoreCase))
                p1 = "forms";
            else if (string.Equals(subtype, "Survey Question", StringComparison.OrdinalIgnoreCase))
                p1 = "questions";
            else
                p1 = "surveys";

            return string.Format(OrganizationRelativePath.SurveyPathTemplate, p1, key);
        }

        public static string GetSurveyUploadPath(Guid identifier, string subtype)
        {
            string p1;

            if (string.Equals(subtype, "Survey Form", StringComparison.OrdinalIgnoreCase))
                p1 = "forms";
            else if (string.Equals(subtype, "Survey Question", StringComparison.OrdinalIgnoreCase))
                p1 = "questions";
            else
                p1 = "surveys";

            return string.Format(OrganizationRelativePath.SurveyPathTemplate, p1, identifier);
        }
    }
}