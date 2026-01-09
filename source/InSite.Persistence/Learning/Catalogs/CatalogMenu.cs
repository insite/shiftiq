using System;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Persistence
{
    public class CatalogMenu
    {
        public CatalogMenu()
        {
            Categories = new List<CatalogMenuItem>();
        }

        public Guid CatalogId { get; set; }
        public string CatalogName { get; set; }
        public string CatalogCollapsed { get; set; } = "collapsed";
        public string CatalogSlug => "catalog_" + StringHelper.Sanitize(CatalogName, '-');
        public int CatalogSize { get; set; }

        public List<CatalogMenuItem> Categories { get; set; }
        public string CategoriesShow => CatalogCollapsed != "collapsed" ? "show" : null;
    }
}