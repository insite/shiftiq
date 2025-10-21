using System;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class ChangeFormNotification : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.Parse(Request.QueryString["form"]);

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            WhenAttemptStartedNotifyAdminMessageIdentifier.AutoPostBack = true;
            WhenAttemptCompletedNotifyAdminMessageIdentifier.AutoPostBack = true;

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

            Save();

            RedirectToReader();
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

        private void Save()
        {
            var startedId = WhenAttemptStartedNotifyAdminMessageIdentifier.Value as Guid?;
            if (startedId == Guid.Empty)
                startedId = null;

            ServiceLocator.SendCommand(
                new ConnectFormMessage(BankID, FormID, FormMessageType.WhenAttemptStartedNotifyAdmin, startedId));

            var completedId = WhenAttemptCompletedNotifyAdminMessageIdentifier.Value as Guid?;
            if (completedId == Guid.Empty)
                completedId = null;

            ServiceLocator.SendCommand(
                new ConnectFormMessage(BankID, FormID, FormMessageType.WhenAttemptCompletedNotifyAdmin, completedId));
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Form form)
        {
            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{(form.Content.Title?.Default).IfNullOrEmpty(form.Name)} <span class=\"form-text\">Asset #{form.Asset}</span>");

            FormDetails.BindForm(form, BankID, form.Specification.Bank.IsAdvanced, true, false);

            WhenAttemptStartedNotifyAdminMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            WhenAttemptCompletedNotifyAdminMessageIdentifier.Filter.Type = MessageTypeName.Notification;

            WhenAttemptStartedNotifyAdminMessageIdentifier.Value = form.WhenAttemptStartedNotifyAdminMessageIdentifier;
            WhenAttemptCompletedNotifyAdminMessageIdentifier.Value = form.WhenAttemptCompletedNotifyAdminMessageIdentifier;

            CancelButton.NavigateUrl = GetReaderUrl();
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader()
        {
            var url = GetReaderUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl()
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}&form={FormID}&tab=content";

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