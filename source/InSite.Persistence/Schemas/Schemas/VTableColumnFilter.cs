using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class VTableColumnFilter : Filter
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public bool IsExactComparison { get; set; }
        public bool? IsRequired { get; set; }
    }
}