using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardQuery
    {
        public string File { get; set; }
        public string FileRaw { get; set; }
        public string Sql { get; set; }
        public string SqlRaw { get; set; }
        public DashboardQueryColumn[] Columns { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public DashboardQuery()
        {
            Parameters = new Dictionary<string, string>();
        }

        public DashboardQueryColumn FindColumn(string name)
        {
            if (Columns == null)
                return null;
            return Columns.FirstOrDefault(c => c.Name == name);
        }
    }
}