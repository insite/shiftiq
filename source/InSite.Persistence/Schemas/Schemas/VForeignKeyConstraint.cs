namespace InSite.Persistence
{
    public class VForeignKeyConstraint
    {
        public string ConstraintName { get; set; }
        public string ForeignColumnName { get; set; }
        public string ForeignSchemaName { get; set; }
        public string ForeignTableName { get; set; }
        public string PrimaryColumnName { get; set; }
        public string PrimarySchemaName { get; set; }
        public string PrimaryTableName { get; set; }
    }
}
