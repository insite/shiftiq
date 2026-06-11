namespace InSite.Persistence
{
    public class VViewColumn
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string SchemaName { get; set; }
        public string ViewName { get; set; }

        public bool? IsIdentity { get; set; }
        public bool? IsRequired { get; set; }

        public int? MaximumLength { get; set; }
        public int? OrdinalPosition { get; set; }
        public int SchemaId { get; set; }
        public int ViewId { get; set; }
    }
}
