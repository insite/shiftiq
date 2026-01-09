using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;

using Shift.Constant;

namespace InSite.UI.Portal.Records.Credentials.Learners
{
    public partial class Search : SearchPage<VCredentialFilter>
    {
        private VCredentialFilter Filter
        {
            get => ViewState[nameof(Filter)] as VCredentialFilter;
            set => ViewState[nameof(Filter)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddCertificate.Alert += (s, a) => ScreenStatus.AddMessage(a);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            base.OnLoad(e);

            if (IsPostBack)
                return;

            Layout.Admin.PageHelper.AutoBindHeader(this, qualifier: "My Achievements");

            AddNewTab.Visible = AddCertificate.Visible;
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload.GetColumns(typeof(Controls.SearchResults.ExportDataItem)).OrderBy(x => x.Name);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsQueryStringValid(Request.QueryString, null, new[] { "auto-search" }, DummyAlert))
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

        protected override void SetDownloadsVisiblity(bool visible)
        {
            base.SetDownloadsVisiblity(false);
        }
    }
}