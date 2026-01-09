using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using DocumentFormat.OpenXml.Spreadsheet;

using InSite.Application.Cases.Write;
using InSite.Application.Contents.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using CheckBox = InSite.Common.Web.UI.CheckBox;

namespace InSite.Admin.Issues.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QIssueFilter>
    {
        #region Properties

        private bool ManyValues
        {
            get { return (bool)ViewState[nameof(ManyValues)]; }
            set { ViewState[nameof(ManyValues)] = value; }
        }

        private string IssueType
        {
            get { return (string)ViewState[nameof(IssueType)]; }
            set { ViewState[nameof(IssueType)] = value; }
        }

        private Dictionary<Guid, bool> SelectedItems
        {
            get => (Dictionary<Guid, bool>)(ViewState[nameof(SelectedItems)]
                ?? (ViewState[nameof(SelectedItems)] = new Dictionary<Guid, bool>()));
            set => ViewState[nameof(SelectedItems)] = value;
        }

        public bool IsCheckboxColumnVisible
        {
            get => (bool)(ViewState[nameof(IsCheckboxColumnVisible)] ?? false);
            set => ViewState[nameof(IsCheckboxColumnVisible)] = value;
        }

        #endregion

        #region Fields

        public event EventHandler OwnerAssigned;

        private List<VComment> _comments = new List<VComment>();

        private Dictionary<Guid, int> _responseAttachmentCounts = new Dictionary<Guid, int>();

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
            Grid.PageIndexChanging += Grid_PageIndexChanging;

            AssignButton.Click += AssignButton_Click;
            SaveBulkButton.Click += SaveBulkButton_Click;

            IssueStatus.StatusCategory = "Closed";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
            {
                var hiddenShowValue = hfShowCheckboxColumn.Value == "true";
                ScriptManager.RegisterStartupScript(this, GetType(), "enableBulkSaveButton", "enableBulkSaveButton();", true);
                SetSaveButtonVisibility();

                if (hiddenShowValue == IsCheckboxColumnVisible)
                    RestoreScrollPosition();
                else
                    RenderBulkCloseStatusFunctionality(hiddenShowValue);
            }
            else
            {
                NewOwnerID.Filter.GroupIdentifier = Organization.AdministratorGroupIdentifier;
                NewOwnerID.Filter.IsAdministrator = true;
            }
        }

        private void RenderBulkCloseStatusFunctionality(bool hiddenShowValue)
        {
            if (!hiddenShowValue)
                SelectedItems = new Dictionary<Guid, bool>();

            AssignButtonStart.Enabled = !hiddenShowValue;
            IsCheckboxColumnVisible = hiddenShowValue;
            Grid.Columns[0].Visible = IsCheckboxColumnVisible;
            BulkUpdatePanel.Visible = IsCheckboxColumnVisible;
            BulkCloseCasesButton.Enabled = !IsCheckboxColumnVisible;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "scrollToBottom", "window.scrollTo(0, document.body.scrollHeight);", true);
        }

        #endregion

        #region Search results

        protected override int SelectCount(QIssueFilter filter)
        {
            return ServiceLocator.IssueSearch.CountIssues(filter);
        }

        protected override IListSource SelectData(QIssueFilter filter)
        {
            var issues = ServiceLocator.IssueSearch.GetIssues(filter);

            GetIssueComments(issues);
            CalcResponseAttachmentCount(issues);

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

            var lastComment = _comments.OrderBy(x => x.CommentPosted).LastOrDefault();
            if (lastComment != null)
            {
                var lastCommentDate = (ITextControl)e.Row.FindControl("LastCommentDate");
                lastCommentDate.Text = LocalizeDate(lastComment.CommentPosted);
            }

            var chkSelect = (CheckBox)e.Row.FindControl("SelectCase");
            var itemId = (Guid)Grid.DataKeys[e.Row.RowIndex].Value;

            if (chkSelect != null && SelectedItems.ContainsKey(itemId))
                chkSelect.Checked = SelectedItems[itemId];
        }

        private void AssignButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var issueIds = GetSelectedIssues();
            if (issueIds.Count == 0)
                return;

            var ownerUserId = NewOwnerID.Value ?? throw new ArgumentNullException("NewOwnerID");
            var commands = new List<Command>();

            foreach (var issueId in issueIds)
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

            OwnerAssigned?.Invoke(this, new EventArgs());
        }

        private void SaveBulkButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (SelectedItems.Count == 0)
                return;

            var issueStatusId = GetIssueStatusId();

            if (!issueStatusId.HasValue)
                return;

            var commands = new List<Command>();

            foreach (var item in SelectedItems)
                commands.Add(new ChangeIssueStatus(item.Key, issueStatusId.Value, DateTimeOffset.UtcNow));

            if (commands.Count == 0)
                return;

            ServiceLocator.SendCommands(commands);

            RenderBulkCloseStatusFunctionality(false);

            RefreshGrid();
        }

        protected void SelectAllCases_CheckedChanged(object sender, EventArgs e)
        {
            var chkSelectAll = (CheckBox)sender;

            foreach (GridViewRow row in Grid.Rows)
            {
                var chkSelect = (CheckBox)row.FindControl("SelectCase");
                if (chkSelect == null)
                    continue;

                chkSelect.Checked = chkSelectAll.Checked;
                Guid itemId = (Guid)Grid.DataKeys[row.RowIndex].Value;
                SelectedItems[itemId] = chkSelectAll.Checked;
            }
        }

        protected void Grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            foreach (GridViewRow row in Grid.Rows)
            {
                var chkSelect = (CheckBox)row.FindControl("SelectCase");
                var itemId = (Guid)Grid.DataKeys[row.RowIndex].Value;

                if (chkSelect != null)
                    SelectedItems[itemId] = chkSelect.Checked;
            }

            Grid.PageIndex = e.NewPageIndex;
        }

        protected void SelectCase_CheckedChanged(object sender, EventArgs e)
        {
            var chkSelect = (CheckBox)sender;
            var row = (GridViewRow)chkSelect.NamingContainer;
            var itemId = (Guid)Grid.DataKeys[row.RowIndex].Value;

            SelectedItems[itemId] = chkSelect.Checked;

            SaveScrollPosition();
            SetSaveButtonVisibility();
        }

        private void SetSaveButtonVisibility()
        {
            SaveBulkButton.Enabled = SelectedItems.Any() && SelectedItems.Any(item => item.Value);
        }

        #endregion

        #region Private Methods

        private List<Guid> GetSelectedIssues()
        {
            var list = new List<Guid>();

            foreach (GridViewRow row in Grid.Rows)
            {
                var assignCheckBox = (ICheckBoxControl)row.FindControl("AssignCheckBox");
                if (assignCheckBox.Checked)
                    list.Add(GetDataKeys(row)[0]);
            }

            return list;
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

        private void SaveScrollPosition()
        {
            var script = @"
        var scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
        document.getElementById('" + hfScrollPosition.ClientID + @"').value = scrollPosition;";
            ScriptManager.RegisterStartupScript(this, GetType(), "SaveScrollPosition", script, true);
        }

        private void RestoreScrollPosition()
        {
            var scrollPosition = hfScrollPosition.Value;

            if (!string.IsNullOrEmpty(scrollPosition))
            {
                var script = $@"
            window.onload = function() {{
                window.scrollTo(0, {scrollPosition});
            }};";
                ScriptManager.RegisterStartupScript(this, GetType(), "RestoreScrollPosition", script, true);
            }
        }

        internal void IssuTypeSet(bool hasValue, string issueType)
        {
            BulkCloseCasesButton.Enabled = hasValue;

            if (!hasValue)
                BulkUpdateStatusInfo.AddMessage(AlertType.Warning, "Bulk Close Case - Available only when filtering by same Case Type.");

            if (hasValue && issueType.HasValue())
            {
                IssueType = issueType;
                IssueStatus.IssueType = IssueType;
                IssueStatus.RefreshData();

                SetIssueStatusTypeControlsVisibility(IssueStatus.Items);
            }
        }

        private void SetIssueStatusTypeControlsVisibility(ComboBoxItemCollection<ComboBoxItem> items)
        {
            NoCaseStatus.Visible = OneCaseStatus.Visible = ManyCaseStatus.Visible = false;

            if (items.Count() == 0)
            {
                ManyValues = false;
                NoCaseStatus.Visible = true;
            }
            else if (items.Count() == 1)
            {
                ManyValues = false;
                OneCaseStatus.Visible = true;
                OneCaseStatusLiteral.Text = items.FirstOrDefault().Text;
            }
            else
            {
                ManyValues = true;
                ManyCaseStatus.Visible = true;
            }
        }

        private Guid? GetIssueStatusId()
        {
            if (ManyValues)
                return IssueStatus.ValueAsGuid;

            var items = ServiceLocator.IssueSearch.GetStatuses(CurrentSessionState.Identity.Organization.Identifier, IssueType, "Closed");
            return items.FirstOrDefault()?.StatusIdentifier ?? null;
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

        #endregion
    }
}