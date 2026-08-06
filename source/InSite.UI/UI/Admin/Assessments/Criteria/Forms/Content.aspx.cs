using System;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assessments.Criteria.Forms
{
    public partial class Content : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankId => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid CriterionId => Guid.TryParse(Request.QueryString["criterion"], out var value) ? value : Guid.Empty;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToReader();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Save())
                RedirectToReader(CriterionId);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankId);
            if (bank == null)
                RedirectToFinder();

            var criterion = bank.FindCriterion(CriterionId);
            if (criterion == null || criterion.Specification.Type != SpecificationType.Dynamic)
                RedirectToReader();

            SetInputValues(criterion);

            CancelButton.NavigateUrl = GetReaderUrl();
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            var bank = ServiceLocator.BankSearch.GetBankState(BankId);
            var criterion = bank.FindCriterion(CriterionId);
            var content = criterion.Content?.Clone() ?? new ContentExamCriterion();

            GetInputValues(content);

            ServiceLocator.SendCommand(new ChangeCriterionContent(BankId, criterion.Identifier, content));

            return true;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Criterion criterion)
        {
            var bank = criterion.Specification.Bank;
            var title = $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class='form-text'>Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            if (ContentEditor.IsEmpty)
            {
                var content = criterion.Content ?? new ContentExamCriterion();

                ContentEditor.Add(ContentSectionDefault.Title, content, false);

                {
                    var summary = (AssetContentSection.Markdown)AssetContentSection.Create(ContentSectionDefault.Summary, content);
                    summary.AllowUpload = true;
                    summary.UploadFolderPath = $"/assessments/{bank.Asset}/criteria/{criterion.Identifier}";
                    ContentEditor.Add(summary);
                }

                ContentEditor.SetLanguage(CurrentSessionState.Identity.Language);
                ContentEditor.OpenTab(Request["tab"]);
            }
        }

        private void GetInputValues(ContentExamCriterion content)
        {
            content.Title = ContentEditor.GetValue(ContentSectionDefault.Title);
            content.Summary = ContentEditor.GetValue(ContentSectionDefault.Summary);
        }

        #endregion

        #region Helper methods

        private void RedirectToFinder() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? criterionId = null)
        {
            var url = GetReaderUrl(criterionId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? criterionId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankId}";

            if (criterionId.HasValue)
                url += $"&sieve={criterionId.Value}";
            else
                url += "&panel=specifications";

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankId}"
                : null;
        }

        #endregion
    }
}
