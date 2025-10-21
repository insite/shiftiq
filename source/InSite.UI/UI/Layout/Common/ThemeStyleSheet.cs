using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Web;

using Shift.Common;

namespace InSite.UI.Layout.Common
{
    public static class ThemeStyleSheet
    {
        private const string CssBundlesRoot = "/UI/Layout/Common/Styles";
        private const string DefaultThemeStyle = CssBundlesRoot + "/Shift.css";

        private static readonly ReadOnlyDictionary<string, string> OrganizationThemeStyles;

        static ThemeStyleSheet()
        {
            var tData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            LoadFiles(tData, CssBundlesRoot + "/tenants", "*.css");
            OrganizationThemeStyles = new ReadOnlyDictionary<string, string>(tData);
        }

        private static void LoadFiles(Dictionary<string, string> data, string relativePath, string searchPattern)
        {
            var physicalPath = HttpContext.Current.Server.MapPath(relativePath);

            var dir = new DirectoryInfo(physicalPath);
            if (!dir.Exists)
                return;

            foreach (var file in dir.EnumerateFiles(searchPattern))
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                var path = PathHelper.PhysicalToRelativePath(file.FullName);

                data.Add(name, path);
            }
        }

        public static string Get()
        {
            var code = CurrentSessionState.Identity?.Organization?.Code
                ?? CookieTokenModule.Current.OrganizationCode;
            if (code.IsEmpty())
                return null;

            if (OrganizationThemeStyles.TryGetValue(code, out var styleUrl))
                return styleUrl;

            return DefaultThemeStyle;
        }
    }
}