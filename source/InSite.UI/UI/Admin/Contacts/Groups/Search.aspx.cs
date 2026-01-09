using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Contacts.Groups.Controls;
using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Contacts.Groups
{
    public partial class Search : SearchPage<QGroupFilter>
    {
        public string DefaultGroupType
            => Request.QueryString["type"];

        public string DefaultGroupLabel
            => Request.QueryString["label"];

        public override string EntityName
            => DefaultGroupType ?? "Group";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload
                .GetColumns(typeof(InSite.Admin.Contacts.Groups.Controls.SearchResults.ExportDataItem))
                .OrderBy(x => x.Name);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Group", "/ui/admin/contacts/groups/create", null, null));
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack && !SearchResults.HasRows && (DefaultGroupType != null || DefaultGroupLabel != null))
                SearchResults.Search(SearchCriteria.Filter);
        }
    }
}