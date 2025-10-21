using System;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Banks.Forms
{
    public partial class ChangeLevel : AdminBasePage, IHasParentLinkParameters
    {
        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search");

            if (!IsPostBack)
                Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToReader();
        }

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            SetInputValues(bank);
        }

        private void Save()
        {
            var level = new Level
            {
                Type = LevelType.Value.NullIfEmpty(),
                Number = LevelNumber.ValueAsInt
            };

            ServiceLocator.SendCommand(new ChangeBankLevel(BankID, level));

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank.Edition.Major != EditionMajor.Text || bank.Edition.Minor != EditionMinor.Text)
                ServiceLocator.SendCommand(new ChangeBankEdition(BankID, EditionMajor.Text, EditionMinor.Text));

            ServiceLocator.SendCommand(new ChangeBankStatus(BankID, BankEnabled.ValueAsBoolean.Value));
        }

        private void SetInputValues(BankState bank)
        {
            var title = $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            BankDetails.BindBank(bank, true, true, false);

            LevelType.Value = bank.Level.Type;
            LevelNumber.ValueAsInt = bank.Level.Number;
            BankEnabled.ValueAsBoolean = bank.IsActive;

            EditionMajor.Text = bank.Edition.Major;
            EditionMinor.Text = bank.Edition.Minor;

            CancelButton.NavigateUrl = GetReaderUrl(BankID);
        }

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader()
        {
            var url = GetReaderUrl(BankID);

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