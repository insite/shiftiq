namespace InSite.Persistence
{
    public class VForeignKey
    {
        public string ForeignColumnName { get; set; }
        public string ForeignSchemaName { get; set; }
        public string ForeignTableName { get; set; }
        public string PrimaryColumnName { get; set; }
        public string PrimarySchemaName { get; set; }
        public string PrimaryTableName { get; set; }
        public string UniqueName { get; set; }

        public bool? ForeignColumnRequired { get; set; }
        public bool IsEnforced { get; set; }

        public int ForeignSchemaId { get; set; }
        public int ForeignTableId { get; set; }
        public int PrimarySchemaId { get; set; }
        public int PrimaryTableId { get; set; }

        public long? RowNumber { get; set; }
    }
}
