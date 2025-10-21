namespace InSite.Persistence
{
    public class VTableColumn
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string DefaultValue { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }

        public bool? IsIdentity { get; set; }
        public bool? IsRequired { get; set; }

        public int? MaximumLength { get; set; }
        public int? OrdinalPosition { get; set; }
        public int SchemaId { get; set; }
        public int TableId { get; set; }
    }
}
