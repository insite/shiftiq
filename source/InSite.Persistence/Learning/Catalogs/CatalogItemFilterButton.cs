using System;

namespace InSite.Persistence
{
    [Serializable]
    public class CatalogItemFilterButton
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string CssClass { get; set; }
    }
}
