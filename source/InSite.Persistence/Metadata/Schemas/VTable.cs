using System;

namespace InSite.Persistence
{
    public class VTable
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }

        public bool? HasClusteredIndex { get; set; }

        public int ColumnCount { get; set; }
        public int SchemaId { get; set; }
        public int TableId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public long? RowCount { get; set; }
    }
}
