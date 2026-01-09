using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

namespace InSite.Domain.Reports
{
    public static class ReportDataSourceReader
    {
        private const string ResourceFolder = "Shift.Sdk.Service.Reporting.State.DataSource.Templates.";

        private static Assembly Assembly => typeof(ReportDataSourceReader).Assembly;

        public static List<string> GetDataSourceNames()
        {
            return Assembly
                .GetManifestResourceNames()
                .Where(x => x.StartsWith(ResourceFolder) && x.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Substring(ResourceFolder.Length, x.Length - ResourceFolder.Length - ".json".Length))
                .OrderBy(x => x)
                .ToList();
        }

        public static ReportDataSource ReadDataSource(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var fullName = ResourceFolder + name + ".json";
            string json;

            try
            {
                using (var stream = Assembly.GetManifestResourceStream(fullName))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ReportDataSource>(json);
        }
    }
}
