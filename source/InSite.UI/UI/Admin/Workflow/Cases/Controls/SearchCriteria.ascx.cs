using System;
using System.Linq;
using System.Web.WebPages;

using InSite.Application.Contacts.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;

using Shift.Constant;

using static InSite.UI.Admin.Issues.Issues.Search;

namespace InSite.Admin.Issues.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QIssueFilter>
    {
        const string OutstandingOption = "Outstanding";
        public event EventHandler<FilterIssueTypeEventArgs> IssueTypeSet;

        private bool? _hasConnections;
        private bool HasConnections
        {
            get
            {
                if (_hasConnections == null)
                {
                    var filter1 = new QUserConnectionFilter
                    {
                        FromUserId = User.Identifier,
                        ToUserOrganizationId = Organization.Identifier,
                    };

                    var filter2 = new QUserConnectionFilter
                    {
                        ToUserId = User.Identifier,
                        FromUserOrganizationId = Organization.Identifier,
                    };

                    _hasConnections = ServiceLocator.UserSearch.CountConnections(filter1) > 0
                        || ServiceLocator.UserSearch.CountConnections(filter2) > 0;
                }

                return _hasConnections.Value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IssueType.Settings.CollectionName = CollectionName.Cases_Classification_Type;
            IssueType.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;

            IssueType.AutoPostBack = true;
            IssueType.ValueChanged += (x, y) => SetIssueStatusVisible();

            MembershipStatus.Settings.CollectionName = CollectionName.Contacts_People_Membership_Status;
            MembershipStatus.Settings.OrganizationIdentifier = Organization.Key;

            AttachmentFileStatus.AutoPostBack = true;
            AttachmentFileStatus.ValueChanged += (x, y) => SetAttachmentsVisible();

            CommentCategory.AutoPostBack = true;
            CommentCategory.ValueChanged += (s, a) => OnCommentCategoryChanged(a.NewValue);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            SetCheckAll(MembershipStatus, "Membership Status");

            CommentCategory.EnsureDataBound();

            CommentTag.Settings.CollectionName = CollectionName.Cases_Comments_Categorization_Tag;
            CommentTag.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;

            AssigneeEmployerIdentifiers.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;

            EnsureFileStatusBound();
        }

        public override QIssueFilter Filter
        {
            get
            {
                var filter = new QIssueFilter
                {
                    OrganizationIdentifier = Organization.Identifier,

                    AdministratorUserIdentifier = ManagerIdentifier.Value,
                    IssueType = IssueType.Value,
                    IssueStatusIdentifier = IssueStatus.ValueAsGuid,
                    IssueStatusCategory = IssueStatusCategory.Value,
                    IssueTitle = IssueTitle.Text,
                    AssigneeName = AssigneeName.Text,
                    LawyerIdentifier = LawyerID.ValueAsGuid,
                    IssueDescription = IssueDescription.Text,
                    AssigneeEmployer = AssigneeEmployerIdentifiers.Values?.Select(x => (Guid?)x).ToList(),
                    PersonCode = PersonCode.Text,
                    AssigneeOrganization = AssigneeOrganization.Text,

                    DateReportedSince = DateReportedSince.Value,
                    DateReportedBefore = DateReportedBefore.Value,
                    DateOpenedSince = DateOpenedSince.Value,
                    DateOpenedBefore = DateOpenedBefore.Value,
                    DateClosedSince = DateClosedSince.Value,
                    DateClosedBefore = DateClosedBefore.Value,
                    DateCaseStatusEffectiveSince = DateCaseStatusEffectiveSince.Value,
                    DateCaseStatusEffectiveBefore = DateCaseStatusEffectiveBefore.Value,

                    OwnerUserIdentifier = OwnerIdentifier.Value,

                    IssueCommentAssigneeIdentifier = IssueCommentAssignedIdentifier.Value,
                    IssueCommentCategory = CommentCategory.Value,
                    IssueCommentSubCategory = CommentSubCategory.Value,
                    IssueCommentFlag = CommentFlag.Value,
                    IssueCommentTag = CommentTag.Value,

                    AttachmentFileStatus = AttachmentFileStatus.Value,
                    AttachmentFileCategory = AttachmentFileCategory.Value,
                    AttachmentDocumentName = AttachmentDocumentName.Text,
                    AttachmentHasClaims = !string.IsNullOrEmpty(AttachmentHasClaims.Value) ? bool.Parse(AttachmentHasClaims.Value) : (bool?)null,
                    AttachmentFileExpirySince = AttachmentFileExpirySince.Value,
                    AttachmentFileExpiryBefore = AttachmentFileExpiryBefore.Value,
                    AttachmentFileReceivedSince = AttachmentFileReceivedSince.Value,
                    AttachmentFileReceivedBefore = AttachmentFileReceivedBefore.Value,
                    AttachmentFileAlternatedSince = AttachmentFileAlternatedSince.Value,
                    AttachmentFileAlternatedBefore = AttachmentFileAlternatedBefore.Value,
                    AttachmentApprovedSince = AttachmentApprovedSince.Value,
                    AttachmentApprovedBefore = AttachmentApprovedBefore.Value,
                    AttachmentUploadedSince = AttachmentUploadedSince.Value,
                    AttachmentUploadedBefore = AttachmentUploadedBefore.Value,

                    OnlyRequestedFiles = AttachmentFileStatus.Value == OutstandingOption,

                    TopicUserConnectedFromUserIdentifier = Organization.Toolkits.Issues?.DisplayOnlyConnectedCases == true && HasConnections
                        ? User.Identifier
                        : (Guid?)null
                };

                if (int.TryParse(IssueNumber.Text, out int number))
                    filter.IssueNumber = number;

                filter.MembershipStatuses = MembershipStatus.ValuesArray;

                IssueTypeSet?.Invoke(this, new FilterIssueTypeEventArgs(filter.IssueType != null, filter.IssueType));

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                IssueNumber.Text = value.IssueNumber?.ToString();

                ManagerIdentifier.Value = value.AdministratorUserIdentifier;

                IssueType.Value = value.IssueType;
                IssueStatusCategory.Value = value.IssueStatusCategory;
                IssueTitle.Text = value.IssueTitle;
                AssigneeName.Text = value.AssigneeName;
                LawyerID.ValueAsGuid = value.LawyerIdentifier;
                MembershipStatus.Values = value.MembershipStatuses;
                IssueDescription.Text = value.IssueDescription;
                AssigneeEmployerIdentifiers.Values = value.AssigneeEmployer?.Where(x => x.HasValue).Select(x => x.Value).ToArray();
                PersonCode.Text = value.PersonCode;
                AssigneeOrganization.Text = value.AssigneeOrganization;

                DateReportedSince.Value = value.DateReportedSince;
                DateReportedBefore.Value = value.DateReportedBefore;
                DateOpenedSince.Value = value.DateOpenedSince;
                DateOpenedBefore.Value = value.DateOpenedBefore;
                DateClosedSince.Value = value.DateClosedSince;
                DateClosedBefore.Value = value.DateClosedBefore;
                DateCaseStatusEffectiveSince.Value = value.DateCaseStatusEffectiveSince;
                DateCaseStatusEffectiveBefore.Value = value.DateCaseStatusEffectiveBefore;

                OwnerIdentifier.Value = value.OwnerUserIdentifier;

                IssueStatus.ValueAsGuid = value.IssueStatusIdentifier;

                IssueCommentAssignedIdentifier.Value = value.IssueCommentAssigneeIdentifier;
                CommentCategory.Value = value.IssueCommentCategory;
                OnCommentCategoryChanged(value.IssueCommentCategory);
                CommentSubCategory.Value = value.IssueCommentSubCategory;
                CommentFlag.Value = value.IssueCommentFlag;
                CommentTag.Value = value.IssueCommentTag;

                EnsureFileStatusBound();
                AttachmentFileStatus.Value = value.AttachmentFileStatus;

                AttachmentFileCategory.EnsureDataBound();
                AttachmentFileCategory.Value = value.AttachmentFileCategory;

                AttachmentDocumentName.Text = value.AttachmentDocumentName;

                AttachmentHasClaims.ClearSelection();
                AttachmentHasClaims.Value = value.AttachmentHasClaims?.ToString()?.ToLower();

                AttachmentFileExpirySince.Value = value.AttachmentFileExpirySince?.LocalDateTime;
                AttachmentFileExpiryBefore.Value = value.AttachmentFileExpiryBefore?.LocalDateTime;
                AttachmentFileReceivedSince.Value = value.AttachmentFileReceivedSince?.LocalDateTime;
                AttachmentFileReceivedBefore.Value = value.AttachmentFileReceivedBefore?.LocalDateTime;
                AttachmentFileAlternatedSince.Value = value.AttachmentFileAlternatedSince?.LocalDateTime;
                AttachmentFileAlternatedBefore.Value = value.AttachmentFileAlternatedBefore?.LocalDateTime;
                AttachmentApprovedSince.Value = value.AttachmentApprovedSince?.LocalDateTime;
                AttachmentApprovedBefore.Value = value.AttachmentApprovedBefore?.LocalDateTime;
                AttachmentUploadedSince.Value = value.AttachmentUploadedSince?.LocalDateTime;
                AttachmentUploadedBefore.Value = value.AttachmentUploadedBefore?.LocalDateTime;

                SetIssueStatusVisible();
                SetAttachmentsVisible();
            }
        }

        public override void Clear()
        {
            IssueNumber.Text = null;
            IssueStatus.Value = null;
            IssueStatusCategory.ClearSelection();
            IssueType.Value = null;
            IssueTitle.Text = null;
            IssueDescription.Text = null;

            AssigneeName.Text = null;
            AssigneeEmployerIdentifiers.Values = null;
            PersonCode.Text = null;
            AssigneeOrganization.Text = null;
            ManagerIdentifier.Value = null;
            LawyerID.ValueAsGuid = null;
            MembershipStatus.ClearSelection();

            DateReportedSince.Value = null;
            DateReportedBefore.Value = null;
            DateOpenedSince.Value = null;
            DateOpenedBefore.Value = null;
            DateClosedSince.Value = null;
            DateClosedBefore.Value = null;
            DateCaseStatusEffectiveSince.Value = null;
            DateCaseStatusEffectiveBefore.Value = null;

            OwnerIdentifier.Value = null;

            IssueCommentAssignedIdentifier.Value = null;
            CommentCategory.Value = null;
            OnCommentCategoryChanged(null);
            CommentFlag.Value = null;
            CommentTag.Value = null;

            AttachmentFileStatus.ClearSelection();
            AttachmentFileCategory.ClearSelection();
            AttachmentDocumentName.Text = null;
            AttachmentHasClaims.ClearSelection();
            AttachmentFileExpirySince.Value = null;
            AttachmentFileExpiryBefore.Value = null;
            AttachmentFileReceivedSince.Value = null;
            AttachmentFileReceivedBefore.Value = null;
            AttachmentFileAlternatedSince.Value = null;
            AttachmentFileAlternatedBefore.Value = null;
            AttachmentApprovedSince.Value = null;
            AttachmentApprovedBefore.Value = null;
            AttachmentUploadedSince.Value = null;
            AttachmentUploadedBefore.Value = null;

            SetIssueStatusVisible();
            SetAttachmentsVisible();

            IssueTypeSet?.Invoke(this, new FilterIssueTypeEventArgs(false, null));
        }

        private void SetIssueStatusVisible()
        {
            IssueStatus.IssueType = IssueType.Value;
            IssueStatus.RefreshData();
            IssueStatusField.Visible = IssueStatus.Items.Count > 0;
        }

        private void SetAttachmentsVisible()
        {
            var isAttachmentsVisible = AttachmentFileStatus.Value != OutstandingOption;

            AttachmentFileCategoryField.Visible = isAttachmentsVisible;
            AttachmentDocumentNameField.Visible = isAttachmentsVisible;
            AttachmentHasClaimsField.Visible = isAttachmentsVisible;
            AttachmentFileExpirySinceField.Visible = isAttachmentsVisible;
            AttachmentFileExpiryBeforeField.Visible = isAttachmentsVisible;
            AttachmentFileReceivedSinceField.Visible = isAttachmentsVisible;
            AttachmentFileReceivedBeforeField.Visible = isAttachmentsVisible;
            AttachmentFileAlternatedSinceField.Visible = isAttachmentsVisible;
            AttachmentFileAlternatedBeforeField.Visible = isAttachmentsVisible;
            AttachmentApprovedSinceField.Visible = isAttachmentsVisible;
            AttachmentApprovedBeforeField.Visible = isAttachmentsVisible;
            AttachmentUploadedSinceField.Visible = isAttachmentsVisible;
            AttachmentUploadedBeforeField.Visible = isAttachmentsVisible;
        }

        private void EnsureFileStatusBound()
        {
            if (AttachmentFileStatus.Items.Count > 1)
                return;

            AttachmentFileStatus.EnsureDataBound();

            int insertIndex = Math.Min(1, AttachmentFileStatus.Items.Count);

            AttachmentFileStatus.Items.Insert(insertIndex, new ComboBoxOption(OutstandingOption, OutstandingOption));
        }

        private void OnCommentCategoryChanged(string value)
        {
            if (value.IsEmpty())
            {
                CommentSubCategory.ClearSelection();
                CommentSubCategoryPanel.Visible = false;
            }

            CommentSubCategory.Settings.CollectionName = $"Cases/Comments/Subcategorization/{value}";
            CommentSubCategory.Settings.OrganizationIdentifier = Organization.Identifier;
            CommentSubCategory.RefreshData();
            CommentSubCategoryPanel.Visible = CommentSubCategory.Items.Count > 0;
        }
    }
}