using System;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Glossaries.Read;
using InSite.Application.Glossaries.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assets.Glossaries.Terms.Forms
{
    public partial class Propose : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/assets/glossaries/search";

        private static readonly string TermIdViewStateKey = typeof(Propose).FullName + "." + nameof(TermIdSessionKey);

        private string TermIdSessionKey
        {
            get => (string)ViewState[TermIdViewStateKey];
            set => ViewState[TermIdViewStateKey] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddButton.Click += AddButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (Add())
                RedirectToParent();
        }

        private void Open()
        {
            if (!CanEdit)
                RedirectToParent();

            PageHelper.AutoBindHeader(this);

            CancelButton.NavigateUrl = SearchUrl;

            SetInputValues();
        }

        private bool Add()
        {
            if (!IsValid)
                return false;

            var term = new QGlossaryTerm();
            var content = new Shift.Common.ContentContainer();

            Detail.GetInputValues(term, content);

            if (ServiceLocator.GlossarySearch.GetTerm(GlossaryHelper.GlossaryIdentifier, term.TermName) != null)
            {
                Status.AddMessage(AlertType.Error, $"The glossary already contains this term: '{term.TermName}'.");

                return false;
            }

            var termId = (TermIdSessionKey.IsNotEmpty() ? (Guid?)Session[TermIdSessionKey] : null)
                ?? UniqueIdentifier.Create();

            ServiceLocator.SendCommand(new ProposeGlossaryTerm(GlossaryHelper.GlossaryIdentifier, termId, term.TermName, content));

            return true;
        }

        private void SetInputValues()
        {
            var termId = UniqueIdentifier.Create();
            TermIdSessionKey = Guid.NewGuid().ToString();
            Session[TermIdSessionKey] = termId;

            Detail.SetDefaultInputValues(GlossaryHelper.GlossaryIdentifier, termId);
        }

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect(SearchUrl);
    }
}