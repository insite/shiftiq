using System;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Content : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.Parse(Request.QueryString["form"]);

        private Guid? SectionID => Guid.TryParse(Request.QueryString["section"], out var value) ? value : (Guid?)null;

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
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Save())
                RedirectToReader(FormID, SectionID);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToReader(FormID, SectionID);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var form = bank.FindForm(FormID);
            if (form == null)
                RedirectToReader();

            SetInputValues(form);
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var form = bank.FindForm(FormID);
            var content = form.Content?.Clone() ?? new ContentExamForm();

            GetInputValues(content);

            ServiceLocator.SendCommand(new ChangeFormContent(
                BankID,
                FormID,
                content,
                HasDiagrams.Checked,
                HasReferenceMaterials.Value.ToEnum(ReferenceMaterialType.None)));

            Course2Store.ClearCache(Organization.Identifier);

            return true;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Form form)
        {
            PageHelper.AutoBindHeader(
                this, 
                qualifier: $"{(form.Content.Title?.Default).IfNullOrEmpty(form.Name)} <span class=\"form-text\">Asset #{form.Asset}</span>");

            if (ContentEditor.IsEmpty)
            {
                var content = form.Content ?? new ContentExamForm();

                ContentEditor.Add(ContentSectionDefault.Title, content);
                ContentEditor.Add(ContentSectionDefault.Summary, content);
                ContentEditor.Add(ContentSectionDefault.Materials, content);
                ContentEditor.Add(ContentSectionDefault.Instructions, content);

                ContentEditor.SetLanguage(CurrentSessionState.Identity.Language);
                ContentEditor.OpenTab(Request["tab"]);
            }

            HasDiagrams.Checked = form.HasDiagrams;

            HasReferenceMaterials.LoadItems(
                ReferenceMaterialType.Acronyms,
                ReferenceMaterialType.Formulas,
                ReferenceMaterialType.All);
            HasReferenceMaterials.Value = form.HasReferenceMaterials.GetName(ReferenceMaterialType.None);
        }

        private void GetInputValues(ContentExamForm content)
        {
            content.Title = ContentEditor.GetValue(ContentSectionDefault.Title);
            content.Summary = ContentEditor.GetValue(ContentSectionDefault.Summary);
            content.MaterialsForDistribution = ContentEditor.GetValue(ContentSectionDefault.Materials, ContentSectionDefault.MaterialsForDistribution);
            content.MaterialsForParticipation = ContentEditor.GetValue(ContentSectionDefault.Materials, ContentSectionDefault.MaterialsForParticipation);
            content.InstructionsForOnline = ContentEditor.GetValue(ContentSectionDefault.Instructions, ContentSectionDefault.InstructionsForOnline);
            content.InstructionsForPaper = ContentEditor.GetValue(ContentSectionDefault.Instructions, ContentSectionDefault.InstructionsForPaper);
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? formId = null, Guid? sectionId = null)
        {
            var url = GetReaderUrl(formId, sectionId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? formId = null, Guid? sectionId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (formId.HasValue)
                url += $"&form={formId.Value}";

            if (sectionId.HasValue)
                url += $"&section={sectionId.Value}";

            url += $"&tab=content";

            return url;
        }

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