using System;
using System.Linq;

using InSite.Application.Banks.Read;
using InSite.Common.Web;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Outlines.Forms
{
    public partial class Outline : AdminBasePage
    {
        #region Constants

        private const string DownloadUrl = "/ui/admin/assessments/banks/download";
        private const string MigrateUrl = "/ui/admin/assessments/banks/migrate";
        private const string PrintUrl = "/ui/admin/assessments/banks/print";

        private const string SearchUrl = "/ui/admin/assessments/banks/search";
        private const string CreateUrl = "/ui/admin/assessments/banks/create";
        private const string SelfUrl = "/ui/admin/assessments/banks/outline";

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        #endregion

        #region Fields

        private bool _isLoaded = false;
        private QBank _bankQuery;
        private BankState _bank;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PrintBankButton.Click += PrintBankButton_Click;
            DownloadSummariesButton.Click += DownloadSummariesButton_Click;

            BankSection.LoadBank = LoadBank;

            QuestionsSection.LoadBank = LoadBank;
            SpecificationsSection.LoadBank = LoadBank;
            SpecificationsSection.ReloadOutline = ReloadOutline;
            FormsSection.LoadBank = LoadBank;
            FormsSection.ReloadOutline = ReloadOutline;
            CommentsSection.LoadBank = LoadBank;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!InitBank() || _bank.Tenant != Organization.OrganizationIdentifier)
            {
                RedirectToSearch();
                return;
            }

            SetInputValues();
        }

        private BankState LoadBank()
        {
            if (!InitBank())
                RedirectToSearch();

            return _bank;
        }

        private bool InitBank()
        {
            if (!_isLoaded)
            {
                _bankQuery = ServiceLocator.BankSearch.GetBank(BankID);
                _bank = ServiceLocator.BankSearch.GetBankState(BankID);
                _isLoaded = true;
            }

            return _bankQuery != null && _bank != null;
        }

        #endregion

        #region Setting/getting input values

        private void SetInputValues()
        {
            var query = _bankQuery;

            PageHelper.AutoBindHeader(
                this,
                qualifier: (query.BankTitle ?? query.BankName) + $" <span class='form-text'>Asset #{query.AssetNumber}</span>");

            DownloadBankLink.NavigateUrl = DownloadUrl + $"?bank={_bank.Identifier}";
            MigrateBankLink.NavigateUrl = MigrateUrl + $"?bank={_bank.Identifier}";
            MigrateBankLink.Visible = _bank.IsAdvanced;
            DuplicateBankLink.NavigateUrl = CreateUrl + $"?action=duplicate&bank={_bank.Identifier}";
            DuplicateBankLink.Visible = true;
            ViewHistoryLink.NavigateUrl = InSite.Admin.Logs.Aggregates.Outline.GetUrl(_bank.Identifier, SelfUrl + $"?bank={_bank.Identifier}");

            BankSection.LoadData(_bank, _bankQuery, CanEdit, CanDelete, out var isBanksPanelSelected);
            QuestionsSection.LoadData(_bank, CanEdit, out var isQuestionsPanelSelected);
            FormsSection.LoadData(_bank, CanEdit, out var isFormsPanelSelected, out var isFormsPanelVisible);

            ValidateAssertions();

            SpecificationsTab.Visible = _bank.IsAdvanced;
            FormsTab.Visible = isFormsPanelVisible;
            CommentsTab.Visible = _bank.IsAdvanced;
            AttachmentsTab.Visible = _bank.IsAdvanced;

            bool isSpecPanelSelected = false, isCommentsPanelVisible = false, isAttachmentsPanelVisible = false;

            if (_bank.IsAdvanced)
            {
                SpecificationsSection.LoadData(_bank, CanEdit, out isSpecPanelSelected);
                CommentsSection.LoadData(_bank, CanEdit, out isCommentsPanelVisible);
                AttachmentsSection.LoadData(_bank, CanEdit, out isAttachmentsPanelVisible);
            }

            if (isBanksPanelSelected)
                BankTab.IsSelected = true;
            if (isQuestionsPanelSelected)
                QuestionsTab.IsSelected = true;
            else if (isSpecPanelSelected)
                SpecificationsTab.IsSelected = true;
            else if (isFormsPanelSelected)
                FormsTab.IsSelected = true;
            else if (isCommentsPanelVisible)
                CommentsTab.IsSelected = true;
            else if (isAttachmentsPanelVisible)
                AttachmentsTab.IsSelected = true;
            else if (!_bank.IsAdvanced)
                FormsTab.IsSelected = true;

            NewBankLink.Visible = CanEdit;
            NewBankSpacer.Visible = CanEdit;
            MigrateBankLink.Visible = CanEdit;
            DownloadSummariesButton.Visible = BankSection.HasSummaries;
        }

        private void ValidateAssertions()
        {
            foreach (var spec in _bank.Specifications)
            {
                foreach (var form in spec.EnumerateAllForms())
                {
                    if (spec.Type == SpecificationType.Static && form.StaticQuestionOrder.IsNotEmpty())
                    {
                        var currentQuestions = form.GetStaticFormQuestionIdentifiersInOrder();
                        var assertionIsTrue = form.StaticQuestionOrder.SequenceEqual(currentQuestions);

                        if (!assertionIsTrue)
                        {
                            var error = Attempts.Forms.Upload.GetQuestionOrderVerificationError(form);
                            ScreenStatus.AddMessage(AlertType.Error, error);
                        }
                    }

                    if (spec.IsTabTimeLimitAllowed && spec.TabTimeLimit == SpecificationTabTimeLimit.AllTabs)
                    {
                        if (form.Sections.Any(x => x.TimeLimit == 0))
                        {
                            var formLink = $"/ui/admin/assessments/banks/outline?bank={_bank.Identifier}&form={form.Identifier}";

                            ScreenStatus.AddMessage(
                                AlertType.Warning,
                                $"One or more sections in the <a href='{formLink}'>{form.Name}</a> form have a missing <i>Time Limit</i>. " +
                                "Please ensure all sections in the form have a specified Time Limit.");
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() =>
            HttpResponseHelper.Redirect(SearchUrl, true);

        private void ReloadOutline() =>
            HttpResponseHelper.Redirect(SelfUrl + $"?bank={BankID}");

        #endregion

        #region Event handlers

        private void PrintBankButton_Click(object sender, EventArgs e) =>
            HttpResponseHelper.Redirect(PrintUrl + $"?bank={BankID}");

        private void DownloadSummariesButton_Click(object sender, EventArgs e)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var data = BankSection.GetSummariesXlsx(bank);

            Response.SendFile("bank-summaries", "xlsx", data);
        }

        #endregion
    }
}