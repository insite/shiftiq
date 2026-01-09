using System;

using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QPageFilter>
    {
        private string DefaultType => Request["type"];

        public override QPageFilter Filter
        {
            get
            {
                var filter = new QPageFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    WebSiteIdentifier = WebSiteId.ValueAsGuid,
                    PageSlug = PageSlug.Text,
                    Title = Title.Text,
                    Modified =
                    {
                        Since = UtcModifiedSince.Value?.UtcDateTime,
                        Before = UtcModifiedBefore.Value?.UtcDateTime
                    },
                    ContentControl = ContentControl.Value,
                    IsPublished = PublicationStatus.ValueAsBoolean,
                    PermissionGroupIdentifier = GroupIdentifier.Value,

                };

                {
                    var type = !IsPostBack && FindTypeOption(DefaultType) != null
                        ? DefaultType
                        : PageType.Value;

                    if (!string.IsNullOrEmpty(type))
                        filter.Types.Add(type);
                }

                GetCheckedShowColumns(filter);

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                WebSiteId.ValueAsGuid = value.WebSiteIdentifier;
                PageSlug.Text = value.PageSlug;
                Title.Text = value.Title;
                UtcModifiedSince.Value = value.Modified.Since;
                UtcModifiedBefore.Value = value.Modified.Before;
                PublicationStatus.ValueAsBoolean = value.IsPublished;
                if (value.PermissionGroupIdentifier.HasValue)
                    GroupIdentifier.Value = value.PermissionGroupIdentifier;

                {
                    var item = !IsPostBack ? FindTypeOption(DefaultType) : null;

                    if (item == null && value.Types.IsNotEmpty())
                        item = FindTypeOption(value.Types[0]);

                    if (item != null)
                        item.Selected = true;
                }

                ContentControl.Value = value.ContentControl;
            }
        }

        public override void Clear()
        {
            WebSiteId.ValueAsInt = null;
            PageType.Value = FindTypeOption(DefaultType)?.Value;
            PageSlug.Text = null;
            Title.Text = null;
            UtcModifiedSince.Value = null;
            UtcModifiedBefore.Value = null;
            ContentControl.Value = null;
            PublicationStatus.ValueAsBoolean = null;
            GroupIdentifier.Value = null;
        }

        private IComboBoxOption FindTypeOption(string value) => value.IsEmpty() ? null : PageType.FindOptionByValue(value);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
            GroupIdentifier.Filter.MustHavePermissions = true;
        }
    }
}