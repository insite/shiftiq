using System;
using System.Data.SqlClient;

using InSite.Application.Pages.Write;
using InSite.Application.Sites.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Publications.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private const string EditUrl = "/ui/admin/assessments/publications/edit";
        private const string SearchUrl = "/ui/admin/assessments/publications/search";

        private Guid? PageIdentifier => Guid.TryParse(Request["page"], out var value) ? value : (Guid?)null;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/edit") ? $"page={PageIdentifier}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteCheck.AutoPostBack = true;
            DeleteCheck.CheckedChanged += (x, y) => { DeleteButton.Enabled = DeleteCheck.Checked; };

            DeleteButton.Click += OnConfirmed;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanDelete)
                NavigateToSearch();

            if (IsPostBack)
                return;

            var entity = PageIdentifier.HasValue
                ? ServiceLocator.PageSearch.GetAssessmentPage(PageIdentifier.Value)
                : null;

            if (entity == null || entity.OrganizationIdentifier != Organization.OrganizationIdentifier)
                NavigateToSearch();

            SetInputValues(entity);

            CancelButton.NavigateUrl = GetReturnUrlInternal();
        }

        private void SetInputValues(VAssessmentPage entity)
        {
            var title = entity.FormTitle.IfNullOrEmpty(entity.FormName).IfNullOrEmpty("Untitled");
            var examForm = ServiceLocator.BankSearch.GetForm(entity.FormIdentifier);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{title} <span class='form-text'>Asset #{entity.FormAsset}.{entity.FormAssetVersion}</span>");

            ResourceTitle.Text = title;

            if (ExamFormField.Visible = examForm != null)
                ExamFormTitle.Text = $"{examForm.FormName} [{examForm.FormAsset}.{examForm.FormAsset}]";
        }

        private void OnConfirmed(object sender, EventArgs e)
        {
            try
            {
                ServiceLocator.SendCommand(new DeletePage(PageIdentifier.Value));
            }
            catch (SqlException sqlex)
            {
                DeleteContainer.Visible = false;
                DeleteButton.Visible = false;

                ScreenStatus.AddMessage(AlertType.Error, "An unexpected error occurred.");

                AppSentry.SentryError(sqlex);
            }

            NavigateToSearch();
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect(SearchUrl, true);

        private string GetReturnUrlInternal() => StringHelper.FirstValue(
            GetReturnUrl,
            () => Request.QueryString["returnUrl"],
            () => $"{EditUrl}?page={PageIdentifier}");
    }
}