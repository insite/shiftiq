using System;
using System.Web.UI;

using InSite.Application.Sites.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assessments.Publications.Forms
{
    public partial class Content : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? PageIdentifier => Guid.TryParse(Request["page"], out var value) ? value : (Guid?)null;

        private string Tab => Request["tab"];

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

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Save())
                RedirectToParent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToParent();
        }

        private void Open()
        {
            var entity = PageIdentifier.HasValue ? ServiceLocator.PageSearch.Select(PageIdentifier.Value) : null;
            if (entity == null)
                RedirectToSearch();

            PageHelper.AutoBindHeader(Page);

            SetInputValues(entity);
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            var content = ServiceLocator.ContentSearch.GetBlock(PageIdentifier.Value);

            GetInputValues(content);

            ServiceLocator.ContentStore.SaveContainer(
                Organization.OrganizationIdentifier, ContentContainerType.WebPage, PageIdentifier.Value, content);

            return true;
        }

        private void SetInputValues(QPage page)
        {
            if (!ContentEditor.IsEmpty)
                return;

            var content = ServiceLocator.ContentSearch.GetBlock(page.PageIdentifier);

            {
                var titleSection = (AssetContentSection.SingleLine)AssetContentSection.Create(ContentSectionDefault.Title, content.Title.Text);
                titleSection.Title = "Title";
                titleSection.Label = "Title";
                ContentEditor.Add(titleSection);
            }

            {
                var summarySection = (AssetContentSection.Markdown)AssetContentSection.Create(ContentSectionDefault.Summary, content.Summary.Text);
                summarySection.Title = "Summary";
                summarySection.Label = "Summary";
                summarySection.AllowUpload = true;
                summarySection.UploadFolderPath = $"/pages/{page.PageIdentifier}";
                ContentEditor.Add(summarySection);
            }

            ContentEditor.SetLanguage(CurrentSessionState.Identity.Language);
            ContentEditor.OpenTab(Request["tab"]);
        }

        private void GetInputValues(ContentContainer content)
        {
            content.Title.Text = ContentEditor.GetValue(ContentSectionDefault.Title);
            content.Summary.Text = ContentEditor.GetValue(ContentSectionDefault.Summary);
        }

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/publications/search", true);

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/publications/edit?page={PageIdentifier}&tab=content.{ContentEditor.GetCurrentTab()}", true);

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"page={PageIdentifier}"
                : null;
        }
    }
}