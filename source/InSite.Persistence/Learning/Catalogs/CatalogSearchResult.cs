using System;

namespace InSite.Persistence
{
    public class CatalogSearchResult
    {
        public Guid CatalogIdentifier { get; set; }
        public string CatalogName { get; set; }
        public int CourseCount { get; set; }
        public int ProgramCount { get; set; }
        public bool IsHidden { get; set; }

        public string GetStatusHtml()
        {
            var html = string.Empty;

            html += IsHidden
                ? "<span class='badge bg-danger fs-sm'>Hidden</span>"
                : "<span class='badge bg-success fs-sm'>Published</span>"
                ;

            return html;
        }
    }
}
