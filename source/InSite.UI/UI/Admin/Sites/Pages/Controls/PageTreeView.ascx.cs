using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Sites.Pages.Controls
{
    public partial class PageTreeView : BaseUserControl
    {
        private class DataItem
        {
            public Guid Id { get; set; }
            public Guid? ParentId { get; set; }
            public string Slug { get; set; }
            public string Hook { get; set; }
            public string Title { get; set; }
            public string Type { get; set; }
            public bool IsUnpublished { get; set; }
            public bool IsPrivate { get; set; }
            public bool? IsParentFound { get; set; }

            public bool IsActive { get; set; }

            public DataItem Parent { get; set; }
            public List<DataItem> Children { get; } = new List<DataItem>();

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemDataBound += Repeater_ItemDataBound;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (DataItem)e.Item.DataItem;

            if (dataItem.Children.Count > 0)
            {
                var container = (DynamicControl)e.Item.FindControl("Container");
                var treeView = (PageTreeView)container.LoadControl("~/UI/Admin/Sites/Pages/Controls/PageTreeView.ascx");
                treeView.LoadData(dataItem.Children);
            }
        }

        public void LoadDataBySiteId(Guid siteId, Guid selectedId)
        {
            var site = ServiceLocator.SiteSearch.Select(siteId);
            var root = new DataItem
            {
                Id = site.SiteIdentifier,
                Type = "Site",
                Title = site.SiteTitle,
                IsActive = site.SiteIdentifier == selectedId
            };
            var pages = ServiceLocator.PageSearch.GetSitePages(siteId);

            LoadData(root, pages, selectedId);
        }

        public void LoadDataByPageId(Guid pageId, Guid selectedId)
        {
            var pages = ServiceLocator.PageSearch.GetTreePages(pageId);

            LoadData(null, pages, selectedId);
        }

        private void LoadData(DataItem root, IEnumerable<QPage> pages, Guid selectedId)
        {
            var hasRoot = root != null;

            if (!hasRoot)
                root = new DataItem();

            var items = pages.Select(x => new DataItem
            {
                Hook = x.Hook,
                Id = x.PageIdentifier,
                Slug = x.PageSlug,
                ParentId = x.ParentPageIdentifier,
                Title = x.Title,
                Type = x.PageType,
                IsUnpublished = x.IsHidden,
                IsActive = x.PageIdentifier == selectedId
            }).ToArray();
            var index = items.ToDictionary(x => x.Id, x => x);
            var privatePages = TGroupPermissionSearch.Distinct(x => x.ObjectIdentifier, x => index.Keys.Contains(x.ObjectIdentifier)).ToHashSet();

            foreach (var item in items)
            {
                var parent = item.ParentId.HasValue && (item.IsParentFound = index.ContainsKey(item.ParentId.Value)).Value
                    ? index[item.ParentId.Value]
                    : root;

                item.Parent = parent;
                parent.Children.Add(item);
                item.IsPrivate = privatePages.Contains(item.Id);
            }

            if (hasRoot)
                LoadData(new[] { root });
            else
                LoadData(root.Children);
        }

        private void LoadData(IEnumerable<DataItem> items)
        {
            Repeater.DataSource = items;
            Repeater.DataBind();
        }

        protected string GetItemIcon()
        {
            var item = (DataItem)Page.GetDataItem();

            switch (item.Type)
            {
                case "Site": return "far fa-cloud";
                case "Folder": return "far fa-folder";
                case "Page": return "far fa-file";
                case "Block": return "far fa-layer-group";
                default: return "far fa-question-square";
            }
        }

        protected string GetItemEditUrl()
        {
            var item = (DataItem)Page.GetDataItem();

            if (item.IsActive)
                return "javascript:void(0);";

            if (item.Type == "Site")
                return $"/ui/admin/sites/outline?id={item.Id}";

            if (item.Type == "Block" && item.ParentId.HasValue)
                return $"/ui/admin/sites/pages/outline?panel=content&id={item.ParentId.Value}&tab=PageBlocks&nav={item.Title}";

            return $"/ui/admin/sites/pages/outline?panel=content&id={item.Id}";
        }

        protected string GetItemDescription()
        {
            var item = (DataItem)Page.GetDataItem();

            if (item.Type == "Site" || item.Type == "Block")
                return $"<strong>{item.Type}</strong>";

            var result = $"<strong>{item.Type}</strong> &bull; {HttpUtility.HtmlEncode(item.Slug)}";

            if (item.IsUnpublished)
                result += "<span class='badge bg-danger ms-2'>Unpublished</span>";

            if (item.IsPrivate)
                result += "<span class='badge bg-info ms-2'>Private</span>";

            if (item.IsParentFound == false)
                result += "<span class='badge bg-danger ms-2'>PARENT NOT FOUND</span>";

            if (item.Hook.IsNotEmpty())
                result += $"<span class='text-warning ms-2'>{HttpUtility.HtmlEncode(item.Hook)}</span>";

            return result;
        }
    }
}