using System;

using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Banks.Forms
{
    public partial class Content : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid? BankID => Guid.TryParse(Request["bank"], out var value) ? value : (Guid?)null;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search");

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && Save())
                RedirectToReader();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToReader();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var summary = BankID.HasValue ? ServiceLocator.BankSearch.GetBank(BankID.Value) : null;
            if (summary == null)
                RedirectToFinder();

            SetInputValues(summary);
        }

        private bool Save()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID.Value);
            var content = bank.Content != null ? bank.Content.Clone() : new ContentExamBank();

            GetInputValues(content);

            ServiceLocator.SendCommand(new ChangeBankContent(BankID.Value, content));

            return true;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(QBank summary)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID.Value);

            var title = summary.BankTitle ?? summary.BankName;
            PageHelper.AutoBindHeader(this, null, title);

            if (ContentEditor.IsEmpty)
            {
                var content = bank.Content ?? new ContentExamBank();

                ContentEditor.Add(ContentSectionDefault.Title, content);
                ContentEditor.Add(ContentSectionDefault.Summary, content);
                ContentEditor.Add(ContentSectionDefault.Materials, content);
                ContentEditor.Add(ContentSectionDefault.Instructions, content);

                ContentEditor.SetLanguage(CurrentSessionState.Identity.Language);
                ContentEditor.OpenTab(Request["tab"]);
            }
        }

        private void GetInputValues(ContentExamBank content)
        {
            content.Title = ContentEditor.GetValue(ContentSectionDefault.Title);
            content.Summary = ContentEditor.GetValue(ContentSectionDefault.Summary);
            content.MaterialsForDistribution = ContentEditor.GetValue(ContentSectionDefault.Materials, ContentSectionDefault.MaterialsForDistribution);
            content.MaterialsForParticipation = ContentEditor.GetValue(ContentSectionDefault.Materials, ContentSectionDefault.MaterialsForParticipation);
            content.InstructionsForOnline = ContentEditor.GetValue(ContentSectionDefault.Instructions, ContentSectionDefault.InstructionsForOnline);
            content.InstructionsForPaper = ContentEditor.GetValue(ContentSectionDefault.Instructions, ContentSectionDefault.InstructionsForPaper);
        }

        #endregion

        #region Helper methods

        private void RedirectToFinder() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/outline?bank={BankID}", true);

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}