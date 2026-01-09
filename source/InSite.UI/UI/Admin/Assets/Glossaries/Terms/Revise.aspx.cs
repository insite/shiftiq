using System;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Glossaries.Read;
using InSite.Application.Glossaries.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Assets.Glossaries.Terms.Forms
{
    public partial class Revise : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/assets/glossaries/search";

        private Guid? TermIdentifier => Guid.TryParse(Request["term"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Save())
                RedirectToParent();
        }

        private void Open()
        {
            if (!CanEdit)
                RedirectToParent();

            var term = TermIdentifier.HasValue ? ServiceLocator.GlossarySearch.GetTerm(TermIdentifier.Value) : null;
            if (term == null)
            {
                RedirectToParent();
                return;
            }

            PageHelper.AutoBindHeader(this);

            CancelButton.NavigateUrl = SearchUrl;

            SetInputValues(term);
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            var term = new QGlossaryTerm();
            var content = ServiceLocator.ContentSearch.GetBlock(TermIdentifier.Value);

            Detail.GetInputValues(term, content);

            ServiceLocator.SendCommand(new ReviseGlossaryTerm(GlossaryHelper.GlossaryIdentifier, TermIdentifier.Value, term.TermName, content));

            return true;
        }

        private void SetInputValues(QGlossaryTerm term)
        {
            var content = ServiceLocator.ContentSearch.GetBlock(TermIdentifier.Value);

            Detail.SetInputValues(term, content);
        }

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"/ui/admin/assets/glossaries/search");
    }
}
