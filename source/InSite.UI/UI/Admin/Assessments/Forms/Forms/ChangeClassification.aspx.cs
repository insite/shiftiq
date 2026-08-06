using System;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class ChangeClassification : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.TryParse(Request.QueryString["form"], out var value) ? value : Guid.Empty;

        private Guid? SectionID => Guid.TryParse(Request.QueryString["section"], out var value) ? value : (Guid?)null;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
            {
                ShowAlert(a => a.ShowPermissionDenied("change form classifications"));
                return;
            }

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToReader(FormID, SectionID);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
            {
                ShowAlert(a =>
                {
                    if (BankID == Guid.Empty)
                        a.ShowBankMissing();
                    else
                        a.ShowBankNotFound(BankID);
                });
                return;
            }

            var form = bank.FindForm(FormID);
            if (form == null)
            {
                var bankName = bank.Name;
                ShowAlert(a =>
                {
                    if (FormID == Guid.Empty)
                        a.ShowFormMissing(BankID, bankName);
                    else
                        a.ShowFormNotFound(FormID, BankID, bankName);
                });
                return;
            }

            SetInputValues(form);
        }

        private void Save()
        {
            ServiceLocator.SendCommand(new ChangeFormClassification(BankID, FormID, Instrument.Value, ThemeInput.Text));
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Form form)
        {
            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{(form.Content.Title?.Default).IfNullOrEmpty(form.Name)} <span class=\"form-text\">Asset #{form.Asset}</span>");

            FormDetails.BindForm(form, BankID, form.Specification.Bank.IsAdvanced);

            Instrument.Value = form.Classification.Instrument;
            ThemeInput.Text = form.Classification.Theme;

            CancelButton.NavigateUrl = GetReaderUrl(FormID, SectionID);
        }

        #endregion

        #region Methods (redirect)

        private void ShowAlert(Action<InSite.Admin.Assessments.Forms.Controls.FormPageAlert> configure)
        {
            ContentPanel.Visible = false;
            configure(PageAlert);
        }

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