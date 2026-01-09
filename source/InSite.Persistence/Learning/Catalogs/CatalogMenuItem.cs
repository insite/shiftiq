using Shift.Common;

namespace InSite.Persistence
{
    public class CatalogMenuItem
    {
        public void SetMenu(CatalogMenu menu)
        {
            _menu = menu;
        }

        private CatalogMenu _menu { get; set; }

        public string CategoryName { get; set; }

        public string CategorySlug => _menu.CatalogSlug + "__category_" + StringHelper.Sanitize(CategoryName, '-');

        public bool IsSelected { get; set; }
    }
}
