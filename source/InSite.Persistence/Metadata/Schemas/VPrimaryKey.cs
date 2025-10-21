namespace InSite.Persistence
{
    public class VPrimaryKey
    {
        public string ColumnName { get; set; }
        public string ConstraintName { get; set; }
        public string DataType { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }

        public bool IsIdentity { get; set; }
    }
}
