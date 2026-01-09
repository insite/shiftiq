namespace InSite.Persistence
{
    public class UnexpectedCollectionItem
    {
        public string EntityName { get; set; }
        public string ActualCollectionSlug { get; set; }
        public string ExpectedCollectionSlug { get; set; }
    }
}