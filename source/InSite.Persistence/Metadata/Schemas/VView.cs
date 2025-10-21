namespace InSite.Persistence
{
    public class VView
    {
        public string SchemaName { get; set; }
        public string ViewName { get; set; }

        public int? ColumnCount { get; set; }
        public int SchemaId { get; set; }
        public int ViewId { get; set; }
    }
}
