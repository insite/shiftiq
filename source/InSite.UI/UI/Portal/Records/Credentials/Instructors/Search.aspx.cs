using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Portal.Records.Credentials.Instructors
{
    public partial class Search : SearchPage<VCredentialFilter>
    {
        private VCredentialFilter Filter
        {
            get => ViewState[nameof(Filter)] as VCredentialFilter;
            set => ViewState[nameof(Filter)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

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

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsQueryStringValid(Request.QueryString, null, new[] { "auto-search", "achievement-label" }, DummyAlert))
            {
                DisableForm();

                // Copy the error messages from the hidden dummy/legacy alert to the Bootstrap 5 alert.
                foreach (var message in DummyAlert.GetMessages())
                    SearchAlert.AddMessage(AlertType.Error, "fas fa-hand-paper", message.Item2);
            }
            else if (!IsPostBack)
            {
                LoadSearchedResults();
            }
        }
    }
}