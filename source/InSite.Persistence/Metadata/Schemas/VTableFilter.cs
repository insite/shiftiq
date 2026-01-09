using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class VTableFilter : Filter
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
    }
}
