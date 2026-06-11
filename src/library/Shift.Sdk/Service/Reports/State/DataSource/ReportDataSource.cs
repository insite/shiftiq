using System;
using System.Linq;

namespace InSite.Domain.Reports
{
    [Serializable]
    public class ReportDataSource
    {
        public string Name { get; set; }
        public View View { get; set; }
        public Join[] Joins { get; set; }

        public string GetViewNameByAlias(string alias)
        {
            if (string.Equals(View.Alias, alias, StringComparison.OrdinalIgnoreCase))
                return View.Name;

            return Joins
                .Where(x => string.Equals(x.Alias, alias, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault()?.Name;
        }
    }
}
