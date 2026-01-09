using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Contents.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Issues.Comments.Controls
{
    public partial class CommentInfo : UserControl
    {
        public Guid IssueIdentifier { get; set; }

        public Guid CommentIdentifier { get; set; }

        public Guid OrganizationIdentifier
            => CurrentSessionState.Identity.Organization.OrganizationIdentifier;

        public string CommentTextValue
            => CommentText.Text;

        public string CommentCategoryValue
            => CommentCategory.Value;

        public string CommentSubCategoryValue
            => CommentSubCategory.Value;

        public string CommentFlagValue
            => CommentFlag.FlagValue?.GetName();

        public string CommentTageValue
            => CommentTag.Value;

        public Guid? CommentAssignedTo
             => AssignedTo.Value;

        public bool? CommentPreviousPrivacy
        {
            get => (bool?)ViewState[nameof(CommentPreviousPrivacy)];
            set => ViewState[nameof(CommentPreviousPrivacy)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommentCategory.AutoPostBack = true;
            CommentCategory.ValueChanged += CommentCategory_ValueChanged;
        }

        protected void CommentCategory_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            BindSubcategoryControl(CommentCategory.Value);
        }

        private void BindSubcategoryControl(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                CommentSubCategory.ClearSelection();
                CommentSubCategoryPanel.Visible = false;
            }

            CommentSubCategory.Settings.CollectionName = $"Cases/Comments/Subcategorization/{category}";
            CommentSubCategory.Settings.OrganizationIdentifier = OrganizationIdentifier;
            CommentSubCategory.RefreshData();
            CommentSubCategoryPanel.Visible = CommentSubCategory.Items.Count > 0;
        }

        public void SetInputValues(VIssue issue, VComment comment)
        {
            CommentText.LoadData(issue, comment);
            CommentCategory.DataBind();
            CommentTag.Settings.CollectionName = CollectionName.Cases_Comments_Categorization_Tag;
            CommentTag.Settings.OrganizationIdentifier = OrganizationIdentifier;

            if (comment == null)
                return;

            CommentCategory.Value = comment.CommentCategory;
            CommentFlag.FlagValue = Enum.TryParse<FlagType>(comment.CommentFlag, out var flag) ? flag : (FlagType?)null;
            CommentTag.Value = comment.CommentTag;

            BindSubcategoryControl(comment.CommentCategory);
            CommentSubCategory.Value = comment.CommentSubCategory;

            AssignedTo.Value = comment.CommentAssignedToUserIdentifier;
        }

        public void RedirectToSearch()
           => HttpResponseHelper.Redirect($"/ui/admin/workflow/cases/search", true);

        public void RedirectToReader()
            => HttpResponseHelper.Redirect(GetReaderUrl(), true);

        public string GetReaderUrl()
            => $"/ui/admin/workflow/cases/outline?case={IssueIdentifier}&panel=comments";

        public string GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"case={IssueIdentifier}" : null;
    }
}