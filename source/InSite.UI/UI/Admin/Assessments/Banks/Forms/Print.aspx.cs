using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Attachments.Controls;
using InSite.Admin.Assessments.Questions.Controls;
using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Common.Web;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Banks.Forms
{
    public partial class Print : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        #endregion

        #region Fields

        private static readonly string _queueStoragePath;

        #endregion

        #region Construction

        static Print()
        {
            _queueStoragePath = PrintQueue.GetStoragePath("Assessments.Banks.Print");
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            HandleAjaxRequest();

            base.OnInit(e);

            BuildImagesButton.Click += BuildImagesButton_Click;
            BuildQuestionsInternal.Click += BuildQuestionsInternal_Click;
            BuildQuestionsCompact.Click += BuildQuestionsCompact_Click;

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

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

        private void BuildImagesAddendum()
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, BankID,
                new ImagePrint.BankOptions(Organization, BankID, OutputType.Addendum),
                (user, options) => ImagePrint.RenderPdf(options));
        }

        private void BuildImagesAll()
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, BankID,
                new ImagePrint.BankOptions(Organization, BankID, OutputType.All),
                (user, options) => ImagePrint.RenderPdf(options));
        }

        private void BuildQuestionsInternal_Click(object sender, EventArgs e)
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, BankID,
                new QuestionPrintInternal.BankOptions(Organization, User, BankID, IncludeImages.Checked, IncludeAdminComments.Checked),
                (user, options) => QuestionPrintInternal.RenderPdf(options));
        }

        private void BuildQuestionsCompact_Click(object sender, EventArgs e)
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, BankID,
                new QuestionPrintCompact.BankOptions(Organization, BankID),
                (user, options) => QuestionPrintCompact.RenderPdf(options));
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var file = PrintQueue.GetPrintFile(_queueStoragePath, User.Identifier, BankID);
            if (file != null)
                Response.SendFile(file.Name, "pdf", file.Data);
        }

        private void BuildImagesButton_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Addendum")
                BuildImagesAddendum();
            else if (e.CommandName == "All")
                BuildImagesAll();
            else
                throw new NotImplementedException();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            SetInputValues(bank);
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(BankState bank)
        {
            var title =
                $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            BankDetails.BindBank(bank);

            if (bank.Sets.Count > 0 && bank.Sets.Any(x => x.Questions.Count > 0))
            {
                BuildQuestionsInternal.Attributes.Remove("disabled");
                BuildQuestionsCompact.Attributes.Remove("disabled");
            }
            else
            {
                BuildQuestionsInternal.Attributes["disabled"] = "disabled";
                BuildQuestionsCompact.Attributes["disabled"] = "disabled";
            }

            if (bank.Attachments.Any(x => x.Type == AttachmentType.Image))
                BuildImagesButton.Items.Add(CreateDropDownButtonItem("All", "All Images", true));
            else
                BuildImagesButton.Items.Add(CreateDropDownButtonItem("All", "All Images", false));

            if (bank.Specifications.Any(x => x.EnumerateAllForms().Any(y => !y.Addendum.IsEmpty)))
                BuildImagesButton.Items.Add(CreateDropDownButtonItem("Addendum", "Addendum Images", true));
            else
                BuildImagesButton.Items.Add(CreateDropDownButtonItem("Addendum", "Addendum Images", false));

            DropDownButtonItem CreateDropDownButtonItem(string buttonName, string buttonText, bool enabled)
            {
                return new DropDownButtonItem()
                {
                    Enabled = enabled,
                    Name = buttonName,
                    IconName = "fas fa-download",
                    Text = buttonText
                };
            }

            GoBackButton.NavigateUrl = GetReaderUrl();
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
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

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