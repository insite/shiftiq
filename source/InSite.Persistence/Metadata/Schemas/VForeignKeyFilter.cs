using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    [Serializable]
    public class VForeignKeyFilter : Filter
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string PrimarySchemaName { get; set; }
        public string PrimaryTableName { get; set; }
        public InclusionType? EnforcedInclusion { get; set; }
        public bool IsExactComparison { get; set; }
        public string UniqueName { get; set; }
    }
}
