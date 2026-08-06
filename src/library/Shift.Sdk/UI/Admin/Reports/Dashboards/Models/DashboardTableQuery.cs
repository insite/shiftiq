using System;
using System.Linq;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardTableQuery : DashboardQuery
    {
        public DashboardTableQueryColumn[] Columns { get; set; }

        public DashboardTableQueryColumn FindColumn(string name)
        {
            if (Columns == null)
                return null;
            return Columns.FirstOrDefault(c => c.Name == name);
        }
    }
}
