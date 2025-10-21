using System;

using InSite.Admin.Assessments.Attachments.Controls;
using InSite.Admin.Assessments.Questions.Controls;
using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Common.Web;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Print : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.TryParse(Request.QueryString["form"], out var value) ? value : Guid.Empty;

        #endregion

        #region Fields

        private static readonly string _queueStoragePath;

        #endregion

        #region Construction

        static Print()
        {
            _queueStoragePath = PrintQueue.GetStoragePath("Assessments.Forms.Print");
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            HandleAjaxRequest();

            base.OnInit(e);

            BuildForm.Click += BuildForm_Click;
            BuildAddendum.Click += BuildAddendum_Click;
            BuildFormInternal.Click += BuildFormInternal_Click;
            BuildFormCompact.Click += BuildFormCompact_Click;

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        protected override void OnPreRender(EventArgs e)
        {
            BuildingPdfPanel.Visible = PrintQueue.IsLocked(_queueStoragePath, User.UserIdentifier);

            base.OnPreRender(e);
        }

        private void HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest || !bool.TryParse(Page.Request.Form["isPageAjax"], out var isAjax) || !isAjax)
                return;

            Response.Clear();
            Response.Write(PrintQueue.IsLocked(_queueStoragePath, User.UserIdentifier) ? "BUSY" : "DONE");
            Response.End();
        }

        #endregion

        #region Event handlers

        private void BuildForm_Click(object sender, EventArgs e)
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, FormID,
                new QuestionPrintExternal.FormOptions(Organization, FormID, IncludeImages.Checked),
                (user, options) => QuestionPrintExternal.RenderPdf(options));
        }

        private void BuildAddendum_Click(object sender, EventArgs e)
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, FormID,
                new ImagePrint.FormOptions(Organization, FormID, OutputType.Addendum),
                (user, options) => ImagePrint.RenderPdf(options));
        }

        private void BuildFormInternal_Click(object sender, EventArgs e)
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, FormID,
                new QuestionPrintInternal.FormOptions(
                    Organization, User, FormID,
                    IncludeImages.Checked, IncludeAdminComments.Checked),
                (user, options) => QuestionPrintInternal.RenderPdf(options));
        }

        private void BuildFormCompact_Click(object sender, EventArgs e)
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, FormID,
                new QuestionPrintCompact.FormOptions(Organization, FormID),
                (user, options) => QuestionPrintCompact.RenderPdf(options));
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var file = PrintQueue.GetPrintFile(_queueStoragePath, User.Identifier, FormID);
            if (file != null)
                Response.SendFile(file.Name, "pdf", file.Data);
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

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Form form)
        {
            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{(form.Content.Title?.Default).IfNullOrEmpty(form.Name)} <span class=\"form-text\">Asset #{form.Asset}</span>");

            FormDetails.BindForm(form, BankID, form.Specification.Bank.IsAdvanced);

            var hasQuestions = form.GetQuestions().Count > 0;
            BuildForm.Enabled = hasQuestions;
            BuildFormInternal.Enabled = hasQuestions;

            if (!form.Addendum.IsEmpty)
                BuildAddendum.Enabled = true;
            else
                BuildAddendum.Enabled = false;

            GoBackButton.NavigateUrl = GetReaderUrl(form.Identifier);
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? formId = null)
        {
            var url = GetReaderUrl(formId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? formId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (formId.HasValue)
                url += $"&form={formId.Value}";

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