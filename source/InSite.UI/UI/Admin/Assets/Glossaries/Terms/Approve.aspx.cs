using System;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Glossaries.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assets.Glossaries.Terms.Forms
{
    public partial class Approve : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/assets/glossaries/search";

        private Guid? TermIdentifier => Guid.TryParse(Request["term"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ApproveButton.Click += ApproveButton_Click;

            CancelButton.NavigateUrl = SearchUrl;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void ApproveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new ApproveGlossaryTerm(GlossaryHelper.GlossaryIdentifier, (Guid)TermIdentifier));

            HttpResponseHelper.Redirect(SearchUrl);
        }

        private void Open()
        {
            var term = TermIdentifier.HasValue ? ServiceLocator.GlossarySearch.GetTerm(TermIdentifier.Value) : null;
            if (term == null)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            PageHelper.AutoBindHeader(this);

            Detail.SetInputValues(term);
        }
    }
}