using System;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using Section = InSite.Domain.Banks.Section;

namespace InSite.Admin.Assessments.Sections.Forms
{
    public partial class Content : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SectionID => Guid.Parse(Request["section"]);

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
                RedirectToReader();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Save())
                RedirectToReader(SectionID);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToReader(SectionID);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToFinder();

            var section = bank.FindSection(SectionID);
            if (section == null || section.Form.Specification.Type != SpecificationType.Static)
                RedirectToReader();

            SetInputValues(section);
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var section = bank.FindSection(SectionID);
            var content = section.Content?.Clone() ?? new ContentExamSection();

            GetInputValues(content);

            ServiceLocator.SendCommand(new ChangeSectionContent(BankID, section.Identifier, content));

            return true;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Section section)
        {
            var bank = section.Form.Specification.Bank;

            var title =
                $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            if (ContentEditor.IsEmpty)
            {
                var content = section.Content ?? new ContentExamSection();

                ContentEditor.Add(ContentSectionDefault.Title, content, false);

                {
                    var summary = (AssetContentSection.Markdown)AssetContentSection.Create(ContentSectionDefault.Summary, content);
                    summary.AllowUpload = true;
                    summary.UploadFolderPath = $"/assessments/{bank.Asset}/sections/{section.Identifier}";
                    ContentEditor.Add(summary);
                }

                ContentEditor.SetLanguage(CurrentSessionState.Identity.Language);
                ContentEditor.OpenTab(Request["tab"]);
            }
        }

        private void GetInputValues(ContentExamSection content)
        {
            content.Title = ContentEditor.GetValue(ContentSectionDefault.Title);
            content.Summary = ContentEditor.GetValue(ContentSectionDefault.Summary);
        }

        #endregion

        #region Helper methods

        private void RedirectToFinder() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? sectionID = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (sectionID.HasValue)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                var section = bank.FindSection(SectionID);

                url += $"&form={section.Form.Identifier}&section={section.Identifier}&tab=section";
            }

            HttpResponseHelper.Redirect(url, true);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}