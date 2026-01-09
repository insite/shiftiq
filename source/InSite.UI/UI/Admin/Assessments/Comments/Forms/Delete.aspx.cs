using System;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Comments.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid CommentID => Guid.Parse(Request["comment"]);

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            RemoveComment();

            RedirectToReader();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var comment = bank?.FindComment(CommentID);
            if (comment == null || bank.Tenant != Organization.OrganizationIdentifier)
                RedirectToReader();

            SetInputValues(bank, comment);
        }

        private void RemoveComment()
        {
            var cmd = new RejectComment(BankID, CommentID);

            ServiceLocator.SendCommand(cmd);
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(BankState bank, Comment comment)
        {
            PageHelper.AutoBindHeader(this, null, $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            SubjectOutput.LoadData(bank, comment);

            if (comment.Flag != FlagType.None)
                CommentFlag.Text = comment.Flag.ToIconHtml() + " " + comment.Flag.GetDescription();
            else
                CommentFlag.Text = "None";
            FeedbackCategory.Text = comment.Category.IfNullOrEmpty("None");
            CommentText.Text = comment.Text;

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
            return new ReturnUrl().GetReturnUrl()
                ?? $"/ui/admin/assessments/banks/outline?bank={BankID}&panel=comments";
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