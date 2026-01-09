using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;

namespace InSite.UI.Portal.Standards
{
    public partial class Search : SearchPage<StandardFilter>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            base.OnLoad(e);

            if (IsPostBack)
                return;

            AutoBindFolderHeader();
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload.GetColumns(typeof(Controls.SearchResults.ExportDataItem)).OrderBy(x => x.Name);
        }
    }
}