using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Content;
using InSite.UI.Portal.Learning.Programs.Models;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Learning.Programs.Controls
{
    public partial class ProgramSearchControl : BaseUserControl
    {
        public delegate void SearchedHandler();

        public event SearchedHandler Searched;

        private VCatalogProgramSearch _programSearch;
        private CatalogItemFilter _filter;

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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButtonRepeater.DataBinding += (x, y) =>
            {
                var buttons = _filter.GetButtons(_programSearch.Catalogs);
                FilterButtonRepeater.DataSource = buttons;
                FilterPanel.Visible = buttons.Count > 0;
            };
            FilterButtonRepeater.ItemCreated += FilterButtonRepeater_ItemCreated;
            FilterButtonRepeater.ItemDataBound += FilterButtonRepeater_ItemDataBound;

            SearchText.ClearClick += (x, y) => Search(1);
            SearchText.FilterClick += (x, y) => Search(1);

            SortBySelect.AutoPostBack = true;
            SortBySelect.ValueChanged += (x, y) => Search(PageNumber);

            PageRepeater.DataBinding += (x, y) => PageRepeater.DataSource = _programSearch.CreatePages(PageNumber, PageCount);
            PageRepeater.ItemCreated += PageRepeater_ItemCreated;
            PageRepeater.ItemDataBound += PageRepeater_ItemDataBound;

            CardRepeater.ProgramsSubmitted += CardRepeater_ProgramsSubmitted;
        }

        public void SetSearch(VCatalogProgramSearch search, CatalogItemFilter filter)
        {
            _programSearch = search;
            _filter = filter;
        }

        public void LoadData(string requestedCatalog, string requestCategory, bool viewEntireCatalog)
        {
            FilterCatalog(requestedCatalog, requestCategory, viewEntireCatalog);

            _filter.AddOrganization(Organization.Identifier);

            if (Organization.ParentOrganizationIdentifier.HasValue)
                _filter.AddOrganization(Organization.ParentOrganizationIdentifier.Value);

            FilterButtonRepeater.DataBind();

            Search(1);
        }

        public void RefreshData()
        {
            FilterButtonRepeater.DataBind();

            Search(1);
        }

        private void FilterCatalog(string requestedCatalog, string requestCategory, bool viewEntireCatalog)
        {
            var cat = _programSearch.FindCatalog(requestedCatalog, requestCategory, viewEntireCatalog);
            if (cat != null)
                _filter.AddCategory(cat.CatalogSlug, requestCategory);
        }

        private void CardRepeater_ProgramsSubmitted(List<Guid> ids)
        {
            var result = ProgramRequestHelper.Submit(ids, CurrentLanguage);

            if (!string.IsNullOrEmpty(result.Error))
            {
                CatalogAlert.AddMessage(AlertType.Error, result.Error);
                return;
            }

            var programNames = string.Join("", result.ProgramNames.Select(x => $"<li>{x}</li>"));

            CatalogAlert.AddMessage(AlertType.Success, $"The request is submitted for the program(s):<ul>{programNames}</ul>");
        }

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

            _filter.RemoveCatalog(slug);
            _filter.RemoveCategory(slug);

            FilterButtonRepeater.DataBind();
            Search(1);

            Searched?.Invoke();
        }

        private void Search(int page)
        {
            var sortBy = SortBySelect.Value == "newest" ? VCatalogProgramSearch.SortBy.Newest : VCatalogProgramSearch.SortBy.Title;

            var items = _programSearch.SearchPrograms(SearchText.Text, _filter, sortBy, page);

            var cards = items.Select(card => new LaunchCard
            {
                Identifier = card.ItemIdentifier,
                Image = card.ThumbnailImageUrl,
                Title = card.ItemTitle,
                Summary = card.ItemDescription,
            })
            .ToList();

            CardRepeater.BindModelToControls(cards, Identity);

            PageNumber = page;
            PageCount = _programSearch.PageCount;

            var x = items.Count > 0 ? (1 + (PageNumber - 1) * CourseCatalogSearch.PageSize) : 0;
            var y = items.Count > 0 ? (x + items.Count - 1) : 0;
            ItemCount.Text = $"{x} to {y} of {_programSearch.ItemCount}";

            PageRepeater.Visible = PageCount > 1;

            PageRepeater.DataBind();
        }

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
    }
}