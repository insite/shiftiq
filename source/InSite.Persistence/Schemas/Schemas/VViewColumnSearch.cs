using System.Collections.Generic;
using System.Linq;

namespace InSite.Persistence
{
    public static class VViewColumnSearch
    {
        public static List<VViewColumn> Select(string schema, string view)
        {
            using (var db = new ReportDbContext())
            {
                return db.VViewColumns.AsNoTracking()
                    .Where(x => x.SchemaName == schema && x.ViewName == view)
                    .ToList();
            }
        }

        public static VViewColumn Select(string schema, string view, string column)
        {
            using (var db = new ReportDbContext())
            {
                return db.VViewColumns.AsNoTracking()
                    .Single(x => x.SchemaName == schema && x.ViewName == view && x.ColumnName == column);
            }
        }
    }
}
