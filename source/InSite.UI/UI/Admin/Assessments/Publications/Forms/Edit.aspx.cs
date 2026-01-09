using System;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Application.Pages.Write;
using InSite.Application.Sites.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Publications.Forms
{
    public partial class Edit : AdminBasePage
    {
        #region Properties

        private const string SearchUrl = "/ui/admin/assessments/publications/search";

        private Guid PageIdentifier => Guid.TryParse(Request.QueryString["page"], out var value) ? value : Guid.Empty;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupType.AutoPostBack = true;
            GroupType.ValueChanged += PrivacyGroupType_ValueChanged;

            PermissionRepeater.DataBinding += PrivacyGroupRepeater_DataBinding;
            PermissionRepeater.ItemCommand += PrivacyGroupRepeater_ItemCommand;

            GrantPermission.Click += AddPrivacyGroup_Click;

            SaveButton.Click += SaveButton_Click;
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
            DeleteButton.Visible = CanDelete;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            Open();

            var tab = Request.QueryString["tab"];
            if (tab.IsNotEmpty())
            {
                tab = tab.ToLower();

                if (tab == "content")
                {
                    ContentTab.IsSelected = true;
                }
                else if (tab == "content.title")
                {
                    ContentTab.IsSelected = true;
                    ContentTitleTab.IsSelected = true;
                }
                else if (tab == "content.summary")
                {
                    ContentTab.IsSelected = true;
                    ContentSummaryTab.IsSelected = true;
                }
            }

            DeleteButton.NavigateUrl = new ReturnUrl("page")
                .GetRedirectUrl($"/ui/admin/assessments/publications/delete?page={PageIdentifier}");
            CancelButton.NavigateUrl = SearchUrl;
        }

        private void Open()
        {
            var entity = ServiceLocator.PageSearch.GetAssessmentPage(PageIdentifier);

            if (entity == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{entity.FormTitle} <span class='form-text'>Asset #{entity.FormAsset}</span>");

            SetInputValues(entity);
        }

        #endregion

        #region Event handlers

        private void PrivacyAccessType_ValueChanged(object sender, EventArgs e) => OnPrivacyAccessTypeChanged();

        private void PrivacyGroupType_ValueChanged(object sender, EventArgs e) => OnPrivacyGroupTypeChanged();

        private void OnPrivacyAccessTypeChanged()
        {
            var isContact = true;

            PermissionContainer.Visible = isContact;

            if (isContact)
            {
                GroupType.Value = null;
                OnPrivacyGroupTypeChanged();
                PermissionRepeater.DataBind();
            }
        }

        private void OnPrivacyGroupTypeChanged()
        {
            GroupIdentifier.Filter.GroupType = GroupType.Value;
            GroupIdentifier.Filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            GroupIdentifier.Value = null;
        }

        private void PrivacyGroupRepeater_DataBinding(object sender, EventArgs e)
        {
            var filter = LinqExtensions1.Expr((TGroupPermission x) => x.ObjectIdentifier == PageIdentifier);

            PermissionRepeater.DataSource = TGroupPermissionSearch.Bind(
                x => new
                {
                    x.GroupIdentifier,
                    ContactName = x.Group.GroupName,
                    x.Group.GroupType
                }, filter.Expand(), "ContactName");
        }

        private void PrivacyGroupRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var contactId = Guid.Parse((string)e.CommandArgument);

                var permission = TGroupPermissionSearch.Select(contactId, PageIdentifier);
                if (permission != null)
                    TGroupPermissionStore.Delete(permission.PermissionIdentifier);

                if (source == PermissionRepeater)
                    PermissionRepeater.DataBind();
            }
        }

        private void AddPrivacyGroup_Click(object sender, EventArgs e)
        {
            var groupId = GroupIdentifier.Value.Value;

            TGroupPermissionStore.Save("Web Page", PageIdentifier, groupId);

            PermissionRepeater.DataBind();
            GroupIdentifier.Value = null;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var before = ServiceLocator.PageSearch.GetAssessmentPage(PageIdentifier);
            var after = ServiceLocator.PageSearch.GetAssessmentPage(PageIdentifier);
            GetInputValues(after);

            if (before.PageIcon != after.PageIcon)
                ServiceLocator.SendCommand(new ChangePageIcon(PageIdentifier, after.PageIcon));

            if (before.PageIsHidden != after.PageIsHidden)
                ServiceLocator.SendCommand(new ChangePageVisibility(PageIdentifier, after.PageIsHidden));

            if (before.PageTitle != after.PageTitle)
                ServiceLocator.SendCommand(new ChangePageTitle(PageIdentifier, after.PageTitle));

            Open();

            SetStatus(ScreenStatus, StatusType.Saved);
        }

        #endregion

        private void SetInputValues(VAssessmentPage page)
        {
            var content = ServiceLocator.ContentSearch.GetBlock(
                page.PageIdentifier,
                ContentContainer.DefaultLanguage,
                new[] { ContentLabel.Title, ContentLabel.Summary });

            // Assessment

            PageTitle.Text = page.PageTitle;
            PageIcon.Text = page.PageIcon;
            PageIsHidden.Checked = page.PageIsHidden;
            PageUrl.Text = AttemptUrlResource.GetStartUrl(page.PageIdentifier);

            FormTitle.Text = page.FormTitle;
            FormName.Text = page.FormName;
            FormCode.Text = page.FormCode;
            FormAsset.Text = $"{page.FormAsset}.{page.FormAssetVersion}";
            PublicationStatus.Text = page.FormPublicationStatus;

            OnPrivacyAccessTypeChanged();

            // Content

            ContentTitleTab.Title = "Title";
            ContentSummaryTab.Title = "Summary";

            EditContentTitleLink.NavigateUrl = $"/ui/admin/assessments/publications/content?page={page.PageIdentifier}&tab=title";
            EditContentSummaryLink.NavigateUrl = $"/ui/admin/assessments/publications/content?page={page.PageIdentifier}&tab=summary";

            ContentTitle.LoadData(content.Title.Text);
            ContentSummary.LoadData(content.Summary.Text);
        }

        private void GetInputValues(VAssessmentPage page)
        {
            page.OrganizationIdentifier = Organization.Identifier;

            page.PageTitle = PageTitle.Text;
            page.PageIcon = PageIcon.Text;
            page.PageIsHidden = PageIsHidden.Checked;
        }
    }
}