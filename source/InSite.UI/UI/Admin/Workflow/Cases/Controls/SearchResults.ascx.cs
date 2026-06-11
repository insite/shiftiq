using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Cases.Write;
using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Common.Timeline.Commands;
using Shift.Constant;

namespace InSite.Admin.Issues.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QIssueFilter>
    {
        #region Properties

        private HashSet<Guid> SelectedItems
        {
            get => (HashSet<Guid>)(ViewState[nameof(SelectedItems)]
                ?? (ViewState[nameof(SelectedItems)] = new HashSet<Guid>()));
            set => ViewState[nameof(SelectedItems)] = value;
        }

        #endregion

        #region Fields

        public event EventHandler OwnerAssigned;

        private List<VComment> _comments = new List<VComment>();

        private Dictionary<Guid, int> _responseAttachmentCounts = new Dictionary<Guid, int>();

        private Dictionary<Guid, string> _departments = new Dictionary<Guid, string>();

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;

            AssignButton.Click += AssignButton_Click;
            SaveBulkButton.Click += SaveBulkButton_Click;
            SaveBulkCaseStatusButton.Click += SaveBulkCaseStatusButton_Click;

            IssueStatus.StatusCategory = "Closed";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
            {
                SyncSelectedItems();
            }
            else
            {
                NewOwnerID.Filter.GroupIdentifier = Organization.AdministratorGroupIdentifier;
                NewOwnerID.Filter.IsAdministrator = true;
            }
        }

        #endregion

        #region Search results

        protected override int SelectCount(QIssueFilter filter)
        {
            var count = ServiceLocator.IssueSearch.CountIssues(filter);

            var canUpdateStatus = !string.IsNullOrEmpty(filter.IssueType) && count > 0;

            BulkUpdateCaseStatusPanel.Visible = canUpdateStatus;
            BulkCloseCasePanel.Visible = canUpdateStatus;

            return count;
        }

        protected override IListSource SelectData(QIssueFilter filter)
        {
            var issues = ServiceLocator.IssueSearch.GetIssues(filter);

            GetIssueComments(issues);
            CalcResponseAttachmentCount(issues);
            GetDepartments(issues);

            BulkHasSelectionOnOtherPages.Value =
                SelectedItems.Any(x => !issues.Any(y => y.IssueIdentifier == x))
                    ? "true"
                    : "false";

            return issues.ToSearchResult();
        }

        private void CalcResponseAttachmentCount(List<VIssue> issues)
        {
            var userIds = issues
                .Where(x => string.Equals(x.IssueSource, "Survey Response") && x.TopicUserIdentifier.HasValue)
                .Select(x => x.TopicUserIdentifier.Value)
                .Distinct()
                .ToArray();

            _responseAttachmentCounts.Clear();
            foreach (var userId in userIds)
                _responseAttachmentCounts.Add(userId, 0);

            var uploads = ServiceLocator.SurveySearch.GetResponseSurveyUploads(Organization.Identifier, userIds, true);
            foreach (var upload in uploads)
            {
                var list = ServiceLocator.StorageService.ParseSurveyResponseAnswer(upload.ResponseAnswerText);

                _responseAttachmentCounts[upload.RespondentUserIdentifier] += list.Count;
            }
        }

        #endregion

        #region Export

        public override IListSource GetExportData(QIssueFilter filter, bool empty)
        {
            return ServiceLocator.IssueSearch.GetExportCases(filter).ToSearchResult();
        }

        #endregion

        #region Event handlers

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var issueIdentifier = (Guid)DataBinder.Eval(e.Row.DataItem, "IssueIdentifier");

            var lastComment = _comments.Where(x => x.IssueIdentifier == issueIdentifier).OrderBy(x => x.CommentPosted).LastOrDefault();
            if (lastComment != null)
            {
                var lastCommentDate = (ITextControl)e.Row.FindControl("LastCommentDate");
                lastCommentDate.Text = LocalizeDate(lastComment.CommentPosted);
            }

            var chk = (ICheckBoxControl)e.Row.FindControl("SelectCheckBox");
            chk.Checked = SelectedItems.Contains(issueIdentifier);
        }

        private void AssignButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (SelectedItems.Count == 0)
                return;

            var ownerUserId = NewOwnerID.Value ?? throw new ArgumentNullException("NewOwnerID");
            var commands = new List<Command>();

            foreach (var issueId in SelectedItems)
            {
                var issue = ServiceLocator.IssueSearch.GetIssue(issueId);
                if (issue.OwnerUserIdentifier == ownerUserId)
                    continue;

                if (issue.OwnerUserIdentifier.HasValue)
                    commands.Add(new UnassignUser(issueId, issue.OwnerUserIdentifier.Value, "Owner"));

                commands.Add(new AssignUser(issueId, ownerUserId, "Owner"));
            }

            NewOwnerID.Value = null;

            if (commands.Count == 0)
                return;

            ServiceLocator.SendCommands(commands);

            ClearSelectedItems();

            OwnerAssigned?.Invoke(this, new EventArgs());
        }

        private void SaveBulkButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (SelectedItems.Count == 0)
                return;

            var issueStatusId = IssueStatus.ValueAsGuid;

            if (!issueStatusId.HasValue)
                return;

            var commands = new List<Command>();

            foreach (var item in SelectedItems)
                commands.Add(new ChangeIssueStatus(item, issueStatusId.Value, DateTimeOffset.UtcNow));

            if (commands.Count == 0)
                return;

            ServiceLocator.SendCommands(commands);

            ClearSelectedItems();

            RefreshGrid();
        }

        private void SaveBulkCaseStatusButton_Click(object sender, EventArgs e)
        {
            if (SelectedItems.Count == 0 || BulkUpdateCaseStatus.ValueAsGuid == null)
                return;

            var statusId = BulkUpdateCaseStatus.ValueAsGuid.Value;

            var commands = new List<Command>();
            foreach (var caseId in SelectedItems)
                commands.Add(new ChangeIssueStatus(caseId, statusId, DateTimeOffset.UtcNow));

            ServiceLocator.SendCommands(commands);

            ClearSelectedItems();

            RefreshGrid();
        }

        #endregion

        #region Public methods

        public void ClearSelectedItems()
        {
            SelectedItems.Clear();

            BulkMode.Value = "";
        }

        #endregion

        #region Private Methods

        private void SyncSelectedItems()
        {
            foreach (GridViewRow row in Grid.Rows)
            {
                var chk = (ICheckBoxControl)row.FindControl("SelectCheckBox");
                var caseId = (Guid)Grid.DataKeys[row.RowIndex].Value;

                if (chk.Checked)
                    SelectedItems.Add(caseId);
                else
                    SelectedItems.Remove(caseId);
            }

            StartBulkCaseStatusButton.Enabled = SelectedItems.Count > 0;
            SaveBulkButton.Enabled = SelectedItems.Count > 0;
        }

        private void GetIssueComments(List<VIssue> issues)
        {
            if (issues.Count == 0)
            {
                _comments.Clear();
                return;
            }

            var commentFilter = new QIssueCommentFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                IssueIdentifiers = issues.Select(x => x.IssueIdentifier).ToArray()
            };

            _comments = ServiceLocator.IssueSearch
                .GetComments(commentFilter)
                .ToList();
        }

        private void GetDepartments(List<VIssue> issues)
        {
            var userIds = issues
                .Where(x => x.TopicUserIdentifier.HasValue)
                .Select(x => x.TopicUserIdentifier.Value)
                .ToArray();

            if (userIds.Length == 0)
            {
                _departments.Clear();
                return;
            }

            var filter = new QMembershipFilter
            {
                GroupOrganizationIdentifier = Organization.Identifier,
                UserIdentifiers = userIds,
                MembershipFunction = "Department"
            };

            _departments = ServiceLocator.MembershipSearch.Select(filter, x => x.Group)
                .GroupBy(x => x.UserIdentifier)
                .ToDictionary(x => x.Key, x => string.Join(", ", x.Select(y => y.Group.GroupName).OrderBy(y => y)));
        }

        internal void IssuTypeSet(bool hasValue, string issueType)
        {
            StartBulkCaseStatusButton.Enabled = hasValue;
            BulkCloseCasesButton.Enabled = hasValue;

            if (!hasValue)
                BulkUpdateStatusInfo.AddMessage(AlertType.Warning, "Bulk Case Update - Available only when filtering by same Case Type.");

            BulkUpdateCaseStatus.IssueType = issueType;
            BulkUpdateCaseStatus.RefreshData();

            if (hasValue && issueType.HasValue())
            {
                IssueStatus.IssueType = issueType;
                IssueStatus.RefreshData();
            }
        }

        #endregion

        #region Binding

        protected int GetTotalAttachmentCount()
        {
            var issue = (VIssue)Page.GetDataItem();

            var responseAttachmentCount = issue.TopicUserIdentifier.HasValue
                    && _responseAttachmentCounts.TryGetValue(issue.TopicUserIdentifier.Value, out var value)
                ? value
                : 0;

            return issue.AttachmentCount + responseAttachmentCount;
        }

        protected string GetTopicDepartments()
        {
            var entity = (VIssue)Page.GetDataItem();
            return entity.TopicUserIdentifier.HasValue
                ? _departments.GetOrDefault(entity.TopicUserIdentifier.Value)
                : null;
        }

        #endregion
    }
}