using System.Collections.Generic;
using System.Linq;

namespace InSite.Persistence
{
    public class VSchemaSearch
    {
        public static IReadOnlyList<VSchema> Select()
        {
            using (var db = new ReportDbContext())
            {
                return db.VSchemas.ToList();
            }
        }
    }
}
