using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Web;

using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Layout.Portal
{
    public static class StyleHelper
    {
        private const string StylesRoot = "/UI/Layout/Common/Styles";
        private const string DefaultThemeStyle = StylesRoot + "/Shift.css";

        private static readonly ReadOnlyDictionary<string, string> OrganizationThemeStyles;

        static StyleHelper()
        {
            var tData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            LoadFiles(tData, StylesRoot + "/Organizations", "*.css");
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
            var code = CurrentSessionState.Identity?.Organization?.Code ?? CookieTokenModule.Current.OrganizationCode;
            if (code.IsEmpty())
                return null;

            var organization = OrganizationSearch.Select(code);

            var styleUrl = Get(OrganizationThemeStyles, code);
            if (styleUrl != null)
                return styleUrl;

            return DefaultThemeStyle;
        }

        private static string Get(ReadOnlyDictionary<string, string> dict, string name)
        {
            if (ServiceLocator.AppSettings.Application.ResourceBundleEnabled
                && dict.TryGetValue(name + ".min", out var a)
                )
            {
                return a;
            }

            if (dict.TryGetValue(name, out var b))
                return b;

            return null;
        }
    }
}