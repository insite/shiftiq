using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Content;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.Web.Helpers;

using Shift.Common;

namespace InSite.UI.Portal.Learning
{
    public partial class Catalog : PortalBasePage
    {
        private CourseCatalogSearch _search;

        private CatalogItemFilter Filter
        {
            get
            {
                if (ViewState[nameof(CatalogItemFilter)] == null)
                    ViewState[nameof(CatalogItemFilter)] = new CatalogItemFilter();

                return (CatalogItemFilter)ViewState[nameof(CatalogItemFilter)];
            }
            set { ViewState[nameof(CatalogItemFilter)] = value; }
        }

        private int PageCount
        {
            get { return (int?)ViewState[nameof(PageCount)] ?? 1; }
            set { ViewState[nameof(PageCount)] = value; }
        }

        private int PageNumber
        {
            get { return (int?)ViewState[nameof(PageNumber)] ?? 1; }
            set { ViewState[nameof(PageNumber)] = value; }
        }

        private Guid? RequestedCatalogId
        {
            get
            {
                var catalog = Request.QueryString["catalog"];

                if (!catalog.HasValue())
                    return null;

                var decodedCatalog = HttpUtility.UrlDecode(catalog);

                if (Guid.TryParse(decodedCatalog, out var id))
                    return id;

                return _search.GetCatalogId(decodedCatalog);
            }
        }

        private string RequestCategory
        {
            get
            {
                if (Request.QueryString["category"].HasValue())
                    return HttpUtility.UrlDecode(Request.QueryString["category"]);
                return null;
            }
        }

        private bool ViewEntireCatalog
        {
            get
            {
                return string.Equals(Request.QueryString["view"], "1");
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var m = Page.Master as PortalMaster;
            if (m != null)
                if (ServiceLocator.Partition.IsE03())
                    m.OverrideHomeLink("/ui/portal/learning/catalog");

            var groups = Identity.Groups.Select(x => x.Identifier).ToArray();
            _search = new CourseCatalogSearch(Identity.Organization.Identifier, RequestedCatalogId, groups, ViewEntireCatalog);

            CatalogRepeater.DataBinding += CatalogRepeater_DataBinding;
            CatalogRepeater.ItemCreated += CatalogRepeater_ItemCreated;
            CatalogRepeater.ItemDataBound += CatalogRepeater_ItemDataBound;

            FilterButtonRepeater.DataBinding += (x, y) =>
            {
                var buttons = Filter.GetButtons(_search.Catalogs);
                FilterButtonRepeater.DataSource = buttons;
                FilterPanel.Visible = buttons.Count > 0;
            };
            FilterButtonRepeater.ItemCreated += FilterButtonRepeater_ItemCreated;
            FilterButtonRepeater.ItemDataBound += FilterButtonRepeater_ItemDataBound;

            SearchText.ClearClick += (x, y) => Search(1);
            SearchText.FilterClick += (x, y) => Search(1);

            SortBySelect.AutoPostBack = true;
            SortBySelect.ValueChanged += (x, y) => Search(PageNumber);

            PageRepeater.DataBinding += (x, y) => PageRepeater.DataSource = _search.CreatePages(PageNumber, PageCount);
            PageRepeater.ItemCreated += PageRepeater_ItemCreated;
            PageRepeater.ItemDataBound += PageRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Page.Request.Url.AbsolutePath != "/ui/portal/learning/catalog")
            {
                PortalMaster.ShowAvatar();
                CategoryPanel.Style["display"] = "none";
            }

            PageHelper.AutoBindHeader(this);

            FilterCatalog();

            Filter.AddOrganization(Organization.Identifier);

            if (Organization.ParentOrganizationIdentifier.HasValue)
                Filter.AddOrganization(Organization.ParentOrganizationIdentifier.Value);

            FilterButtonRepeater.DataBind();

            Search(1);

            PageRepeater.DataBind();
            CatalogRepeater.DataBind();
        }

        private void FilterCatalog()
        {
            var cat = _search.FindCatalog(RequestedCatalogId, RequestCategory, ViewEntireCatalog);
            if (cat != null)
                Filter.AddCategory(cat.CatalogSlug, RequestCategory);
        }

        #region Filter by Category

        private void OrganizationRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var item = (CourseCatalogOrganization)e.Item.DataItem;

            var check = (ICheckBox)e.Item.FindControl("OrganizationCheck");
            check.Text = item.OrganizationName + $"<span class='fs-xs text-body-secondary ms-2'>{item.OrganizationSize}</span>";
            check.Value = item.OrganizationIdentifier.ToString();
            check.Checked = Filter.HasOrganization(item.OrganizationIdentifier);
        }

        private void CatalogRepeater_DataBinding(object sender, EventArgs e)
        {
            foreach (var catalog in _search.Catalogs)
                catalog.CatalogCollapsed = !StringHelper.Equals(catalog.CatalogSlug, Filter?.CatalogSlug) ? "collapsed" : string.Empty;

            CatalogRepeater.DataSource = _search.Catalogs;

            CategoryPanel.Visible = _search.Catalogs.Count > 0;
        }

        private void CatalogRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var repeater = (Repeater)e.Item.FindControl("CategoryRepeater");
            repeater.ItemCreated += CategoryRepeater_ItemCreated;
            repeater.ItemDataBound += CategoryRepeater_ItemDataBound;
        }

        private void CategoryRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var button = (LinkButton)e.Item.FindControl("CategoryButton");
            button.Click += CategoryButton_Click;
        }

        private void CatalogRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var menu = (CatalogMenu)e.Item.DataItem;

            foreach (var category in menu.Categories)
            {
                category.IsSelected = Filter.HasCategory(category.CategoryName);
            }

            var repeater = (Repeater)e.Item.FindControl("CategoryRepeater");
            repeater.DataSource = menu.Categories;
            repeater.DataBind();
        }

        private void CategoryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var item = (CatalogMenuItem)e.Item.DataItem;

            var button = (LinkButton)e.Item.FindControl("CategoryButton");
            button.CommandArgument = item.CategorySlug;
        }

        private void CategoryButton_Click(object sender, EventArgs e)
        {
            var button = (LinkButton)sender;

            var argument = button.CommandArgument;

            var item = _search.Catalogs
                .FirstOrDefault(catalog => catalog.Categories.Any(category => category.CategorySlug == argument));

            if (item != null)
            {
                Filter.AddCategory(item.CatalogSlug, argument);

                FilterButtonRepeater.DataBind();

                Search(1);

                CatalogRepeater.DataBind();
            }
        }

        #endregion

        private void FilterButtonRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var item = (CatalogItemFilterButton)e.Item.DataItem;

            var button = (LinkButton)e.Item.FindControl("FilterButton");
            button.Command += FilterButton_Command;
        }

        private void FilterButtonRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var item = (CatalogItemFilterButton)e.Item.DataItem;

            var button = (LinkButton)e.Item.FindControl("FilterButton");

            button.CommandArgument = item.Value;

            var html = $"<span class='badge bg-secondary fs-sm'>{item.Text}<i class='fas fa-times ms-1'></i></span>";

            button.Text = html;
        }

        private void FilterButton_Command(object sender, EventArgs e)
        {
            var button = (LinkButton)sender;

            var slug = button.CommandArgument;

            Filter.RemoveCatalog(slug);
            Filter.RemoveCategory(slug);

            CatalogRepeater.DataBind();
            FilterButtonRepeater.DataBind();
            Search(1);
        }

        private void Search(int page)
        {
            List<CourseCatalogItem> items = _search.ApplyFilter(SearchText.Text, Filter, SortBySelect.Value, page);

            var cards = items.Select(card => new LaunchCard
            {
                Category = card.ItemSubcategories,
                Flag = card.ItemFlag,
                Image = card.ThumbnailImageUrl,
                Title = card.ItemTitle,
                Url = card.ItemStartUrl
            })
                .ToList();

            CardRepeater.BindModelToControls(cards, Identity);

            PageNumber = page;
            PageCount = _search.PageCount;

            var x = items.Count > 0 ? (1 + (PageNumber - 1) * CourseCatalogSearch.PageSize) : 0;
            var y = items.Count > 0 ? (x + items.Count - 1) : 0;
            ItemCount.Text = $"{x} to {y} of {_search.ItemCount}";

            PageRepeater.DataBind();
        }

        #region Pagination

        private void PageRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var button = (LinkButton)e.Item.FindControl("PageButton");
            button.Command += PageButton_Command;
        }

        private void PageButton_Command(object sender, CommandEventArgs e)
        {
            PageNumber = int.Parse((string)e.CommandArgument);
            PageRepeater.DataBind();
            Search(PageNumber);
        }

        private void PageRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var item = (PaginationItem)e.Item.DataItem;

            var button = (LinkButton)e.Item.FindControl("PageButton");
            button.CommandArgument = item.PageNumber.ToString();
            button.Visible = !item.IsCurrent;

            var label = (Label)e.Item.FindControl("PageLabel");
            label.Visible = item.IsCurrent;
        }

        #endregion
    }
}