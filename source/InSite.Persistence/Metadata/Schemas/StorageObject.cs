using System;

namespace InSite.Persistence
{
    public class StorageObject
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string TableType { get; set; }

        public int ColumnCount { get; set; }
        public long RowCount { get; set; }

        public DateTime Created { get; set; }
    }
}
