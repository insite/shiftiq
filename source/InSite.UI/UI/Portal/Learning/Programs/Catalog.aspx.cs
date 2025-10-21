using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.Web.Helpers;

using Shift.Common;

namespace InSite.UI.Portal.Learning.Programs
{
    public partial class Catalog : PortalBasePage
    {
        private CatalogItemFilter Filter
        {
            get
            {
                if (ViewState[nameof(CatalogItemFilter)] == null)
                    ViewState[nameof(CatalogItemFilter)] = new CatalogItemFilter();

                return (CatalogItemFilter)ViewState[nameof(CatalogItemFilter)];
            }
        }

        private string RequestedCatalog
        {
            get
            {
                var catalog = Request.QueryString["catalog"];

                if (!catalog.HasValue())
                    return null;

                return HttpUtility.UrlDecode(catalog);
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

        private bool ViewEntireCatalog => string.Equals(Request.QueryString["view"], "1");

        private VCatalogProgramSearch _programSearch;
        private VCatalogProgramSearch ProgramSearch
        {
            get
            {
                if (_programSearch == null)
                {
                    var groupIds = Identity.Groups.Select(x => x.Identifier).ToArray();
                    _programSearch = VCatalogProgramSearch.Create(Identity.Organization.Identifier, RequestedCatalog, groupIds, ViewEntireCatalog);
                }

                return _programSearch;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CatalogRepeater.DataBinding += CatalogRepeater_DataBinding;
            CatalogRepeater.ItemCreated += CatalogRepeater_ItemCreated;
            CatalogRepeater.ItemDataBound += CatalogRepeater_ItemDataBound;

            ProgramSearchControl.Searched += ProgramSearchControl_Searched;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ProgramSearchControl.SetSearch(ProgramSearch, Filter);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            ProgramSearchControl.LoadData(RequestedCatalog, RequestCategory, ViewEntireCatalog);

            CatalogRepeater.DataBind();
        }

        private void CatalogRepeater_DataBinding(object sender, EventArgs e)
        {
            foreach (var catalog in ProgramSearch.Catalogs)
                catalog.CatalogCollapsed = !StringHelper.Equals(catalog.CatalogSlug, Filter?.CatalogSlug) ? "collapsed" : string.Empty;

            CatalogRepeater.DataSource = ProgramSearch.Catalogs;

            CategoryPanel.Visible = ProgramSearch.Catalogs.Count > 0;
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

            var item = ProgramSearch.Catalogs
                .FirstOrDefault(catalog => catalog.Categories.Any(category => category.CategorySlug == argument));

            if (item != null)
            {
                Filter.AddCategory(item.CatalogSlug, argument);

                CatalogRepeater.DataBind();

                ProgramSearchControl.RefreshData();
            }
        }

        private void ProgramSearchControl_Searched()
        {
            CatalogRepeater.DataBind();
        }
    }
}