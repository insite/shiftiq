namespace InSite.Persistence
{
    public class UnexpectedEntityItem
    {
        public string StorageTable { get; set; }
        public string ActualEntityName { get; set; }
        public string ExpectedEntityName { get; set; }
    }
}