using System;
using System.Collections.Generic;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Standards.Documents
{
    public partial class Search : SearchPage<StandardDocumentFilter>
    {
        public override string EntityName => "Document";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
                LoadSearchedResults();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Document", "/ui/admin/standards/documents/create", null, null));
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload
                .GetColumns(typeof(InSite.Admin.Standards.Documents.Controls.SearchResults.ExportDataItem));
        }
    }
}