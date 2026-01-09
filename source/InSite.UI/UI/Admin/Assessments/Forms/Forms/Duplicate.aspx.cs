using System;
using System.Web.UI;

using Shift.Common.Timeline.Exceptions;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Duplicate : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string CannotUpgradeOldFormExceptionErrorMessage =
            "This form has already been upgraded to a new version. "
            + "Please use the most recent version of this form if you want to create another new version of it.";

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.Parse(Request.QueryString["form"]);

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
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                var form = Save();
                RedirectToReader(form);
            }
            catch (UnhandledCommandException ex)
            {
                if (ex.InnerException is CannotUpgradeOldFormException)
                    CommandStatus.AddMessage(AlertType.Error, CannotUpgradeOldFormExceptionErrorMessage);
                else
                    throw;
            }
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
                RedirectToReader(null);

            SetInputValues(form);
        }

        private Guid Save()
        {
            var duplicateFormId = UniqueIdentifier.Create();

            ServiceLocator.SendCommand(new UpgradeForm(BankID, FormID, duplicateFormId, Name.FormNameInput));

            return duplicateFormId;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Form form)
        {
            var title = $"{(form.Content.Title?.Default).IfNullOrEmpty(form.Name)} <span class=\"form-text\">Asset #{form.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            FormDetails.BindForm(form, BankID, form.Specification.Bank.IsAdvanced, false);

            Name.FormNameInput = $"{form.Name} (Copy)";

            var formSummary = ServiceLocator.BankSearch.GetForm(FormID);

            if (formSummary == null)
                RedirectToReader(form.Identifier);

            DraftedMessage.Visible = formSummary.FormPublicationStatus.StartsWith(PublicationStatus.Drafted.GetDescription(), StringComparison.OrdinalIgnoreCase);

            if (!form.IsLastVersion())
            {
                CommandStatus.AddMessage(AlertType.Error, CannotUpgradeOldFormExceptionErrorMessage);
                SaveButton.Visible = false;
            }

            CancelButton.NavigateUrl = GetReaderUrl(FormID);
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? formID)
        {
            var url = GetReaderUrl(formID);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? formID)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (formID.HasValue)
                url += $"&form={formID}";

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