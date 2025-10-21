using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class CatalogItemFilter
    {
        public string CatalogSlug { get; set; }

        public List<string> CategorySlugs { get; set; }

        public List<Guid> Organizations { get; set; }

        public List<CatalogItemFilterButton> GetButtons(List<CatalogMenu> menus)
        {
            var buttons = new List<CatalogItemFilterButton>();

            if (CatalogSlug.HasValue())
            {
                var menu = menus.Single(m => string.Equals(m.CatalogSlug, CatalogSlug, StringComparison.OrdinalIgnoreCase));

                var catalogButton = new CatalogItemFilterButton
                {
                    Type = "Catalog",
                    Text = menu.CatalogName,
                    Value = menu.CatalogSlug,
                    CssClass = "active-filter me-2 my-2"
                };
                buttons.Add(catalogButton);

                foreach (var category in CategorySlugs)
                {
                    var item = menu.Categories.Single(i => i.CategorySlug == category);

                    var categoryButton = new CatalogItemFilterButton
                    {
                        Type = "Category",
                        Text = item.CategoryName,
                        Value = item.CategorySlug,
                        CssClass = "active-filter me-2 my-2"
                    };
                    buttons.Add(categoryButton);
                }
            }

            return buttons;
        }

        public List<string> CatalogSlugs
        {
            get
            {
                if (CatalogSlug.HasValue())
                    return new List<string> { CatalogSlug };
                return null;
            }
        }

        public CatalogItemFilter()
        {
            CategorySlugs = new List<string>();
            Organizations = new List<Guid>();
        }

        public void AddCategory(string catalog, string category)
        {
            if (!StringHelper.Equals(catalog, CatalogSlug))
                CategorySlugs.Clear();

            CatalogSlug = catalog;

            if (category == null)
                return;

            if (!CategorySlugs.Contains(category))
                CategorySlugs.Add(category);
        }

        public void AddOrganization(Guid organization)
        {
            if (!HasOrganization(organization))
                Organizations.Add(organization);
        }

        public void RemoveOrganization(Guid organization)
        {
            if (HasOrganization(organization))
                Organizations.Remove(organization);
        }

        public bool HasOrganization(Guid organization)
        {
            return Organizations.Contains(organization);
        }

        public bool HasCatalog(string catalog)
        {
            return CatalogSlugs.Contains(catalog);
        }

        public bool HasCategory(string category)
        {
            return CategorySlugs.Contains(category);
        }

        public void RemoveCatalog(string catalog)
        {
            if (HasCatalog(catalog))
            {
                CatalogSlug = null;
                CategorySlugs.Clear();
            }
        }

        public void RemoveCategory(string category)
        {
            if (HasCategory(category))
                CategorySlugs.Remove(category);
        }
    }
}
