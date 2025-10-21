using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Desktops.Design.Users
{
    public partial class Search : SearchPage<PersonFilter>
    {
        public override string EntityName => "User";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.Alert += (s, a) => ScreenStatus.AddMessage(a);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload.GetColumns(typeof(Controls.SearchResults.ExportDataItem)).OrderBy(x => x.Name);
        }
    }
}