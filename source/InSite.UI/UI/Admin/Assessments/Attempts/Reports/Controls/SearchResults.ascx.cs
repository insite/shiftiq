using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using AttemptView = InSite.Admin.Assessments.Attempts.Forms.View;

namespace InSite.Admin.Attempts.Reports.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QAttemptFilter>
    {
        public event EventHandler AssessorAssigned;
        public event EventHandler AssessorUnassigned;

        private bool _allowView;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
            Grid.RowCommand += Grid_RowCommand;

            AssignButton.Click += AssignButton_Click;
            UnassignButton.Click += UnassignButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BindControls();
        }

        private void BindControls()
        {
            var identity = CurrentSessionState.Identity;

            ButtonPanel.Visible = identity.Organization.Toolkits.Assessments?.AttemptGradingAssessor ?? false;

            var groupIds = TGroupPermissionSearch.SelectGroupFromActionPermission(PermissionNames.Design_Grading_Assessors);
            NewAssessorID.EmptyOnLoad = true;

            if (groupIds != null && groupIds.Length > 0)
            {
                NewAssessorID.Filter.Groups = groupIds;
                NewAssessorID.Filter.OrganizationIdentifier = identity.Organization.OrganizationIdentifier;
                NewAssessorID.EmptyOnLoad = false;
            }
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var link = (IconLink)e.Row.FindControl("ViewAttemptLink");
            link.Visible = _allowView;

            var attempt = (QAttempt)e.Row.DataItem;

            link.ToolTip = attempt.AttemptGraded == null && attempt.AttemptSubmitted.HasValue
                ? "Grade Attempt"
                : "View Attempt";

            var submitButton = (IconButton)e.Row.FindControl("SubmitButton");
            submitButton.Visible = !attempt.AttemptSubmitted.HasValue;
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Submit")
                return;

            var attemptId = Grid.GetDataKey<Guid>(e);

            if (AttemptView.SubmitAttempt(attemptId))
                SearchWithCurrentPageIndex(Filter);
        }

        protected override int SelectCount(QAttemptFilter filter)
        {
            return ServiceLocator.AttemptSearch.CountAttempts(filter);
        }

        protected override IListSource SelectData(QAttemptFilter filter)
        {
            _allowView = true;

            return ServiceLocator.AttemptSearch
                .GetAttempts(filter, x => x.AssessorPerson, x => x.LearnerPerson, x => x.Form, x => x.GradingAssessor)
                .ToSearchResult();
        }

        protected override void SetGridVisibility(bool isVisible, bool showInstructions)
        {
            ReportLink.Visible = isVisible;
            TagLink.Visible = isVisible;

            base.SetGridVisibility(isVisible, showInstructions);
        }

        protected string GetBrowserInfo()
        {
            var attempt = (QAttempt)Page.GetDataItem();
            var sebVersion = AttemptHelper.GetSebVersion(attempt.UserAgent);

            return sebVersion.IsEmpty() ? null : $"<span class='badge bg-info'>SEB v{sebVersion}</span>";
        }

        protected string GetCandidateInfo(VPerson assessor, VPerson learner)
        {
            if (assessor.UserIdentifier == learner.UserIdentifier)
                return GetCandidateInfo(assessor);
            return GetCandidateInfo(learner);
        }

        private static string GetCandidateInfo(VPerson person)
        {
            return $@"<a href='/ui/admin/contacts/people/edit?contact={person.UserIdentifier}'>{person.UserFullName}</a>
                <span class='form-text'>{person.PersonCode}</span>
                <div>
                    <a href='mailto:{person.UserEmail}'>
                        {person.UserEmail}
                    </a>
                </div>";
        }

        protected string FormatScore()
        {
            // Access to individual performance metrics may be denied.

            if (!_allowView)
                return "<span class='text-danger' title='Access to individual performance metrics is denied'>******</span>";

            var attempt = (QAttempt)Page.GetDataItem();
            if (!attempt.AttemptGraded.HasValue)
            {
                return attempt.AttemptSubmitted.HasValue
                    ? "<span class='badge bg-warning'>Pending</span>"
                    : string.Empty;
            }

            var statusHtml = attempt.AttemptIsPassing
                ? "<span class='badge bg-success'>Pass</span>"
                : "<span class='badge bg-danger'>Fail</span>";

            return $"<div>{attempt.AttemptScore:p0}</div>"
                + $"<div class='form-text'>{attempt.AttemptPoints} / {attempt.FormPoints}</div>"
                + statusHtml;
        }

        protected string FormatTime()
        {
            var html = new StringBuilder();

            var attempt = (QAttempt)Page.GetDataItem();
            if (attempt.AttemptStarted.HasValue)
                html.Append("<div>Started " + attempt.AttemptStarted.Value.Format(User.TimeZone, true) + "</div>");

            if (attempt.AttemptGraded.HasValue)
                html.Append("<div>Completed " + attempt.AttemptGraded.Value.Format(User.TimeZone, true) + "</div>");

            if (attempt.AttemptImported.HasValue)
                html.Append("<div class='form-text'>Imported " + attempt.AttemptImported.Value.Format(User.TimeZone, true) + "</div>");

            if (attempt.AttemptDuration.HasValue)
                html.Append("<div class='form-text'>Time Taken = " + ((double)attempt.AttemptDuration).Seconds().Humanize(2, minUnit: TimeUnit.Second) + "</div>");

            return html.ToString();
        }

        protected string GetFormAsset()
        {
            var attempt = (QAttempt)Page.GetDataItem();

            var form = attempt.Form;
            if (form == null)
                return string.Empty;

            var assetVersion = attempt.Form.FormAssetVersion;
            if (form.FormFirstPublished.HasValue && attempt.AttemptStarted.HasValue && attempt.AttemptStarted.Value < form.FormFirstPublished.Value)
                assetVersion = 0;

            return $"{form.FormAsset}.{assetVersion}";
        }

        private void AssignButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var attemptIds = GetSelectedAttempts();
            if (attemptIds.Count == 0)
                return;

            var assessorUserId = NewAssessorID.Value ?? throw new ArgumentNullException("NewAssessorID");
            var commands = new List<Command>();

            foreach (var attemptId in attemptIds)
            {
                var attempt = ServiceLocator.AttemptSearch.GetAttempt(attemptId);
                if (attempt.GradingAssessorUserIdentifier == assessorUserId)
                    continue;

                commands.Add(new ChangeAttempGradingAssessor(attemptId, assessorUserId));
            }

            NewAssessorID.Value = null;

            if (commands.Count == 0)
                return;

            ServiceLocator.SendCommands(commands);

            AssessorAssigned?.Invoke(this, new EventArgs());
        }

        private void UnassignButton_Click(object sender, EventArgs e)
        {
            var commands = new List<Command>();

            foreach (GridViewRow row in Grid.Rows)
            {
                var checkbox = (System.Web.UI.WebControls.CheckBox)row.FindControl("AssignCheckBox");
                var wasAssignedField = (HiddenField)row.FindControl("HasGradingAssessor");

                bool wasAssigned = wasAssignedField?.Value == "1";
                bool isChecked = checkbox != null && checkbox.Checked;
                bool isEnabled = checkbox != null && checkbox.Enabled;

                if (wasAssigned && isChecked && isEnabled)
                {
                    var attemptId = (Guid)GetDataKeys(row)[0];
                    commands.Add(new ChangeAttempGradingAssessor(attemptId, null));
                }
            }

            if (commands.Count > 0)
            {
                ServiceLocator.SendCommands(commands);
                AssessorUnassigned?.Invoke(this, EventArgs.Empty);
            }
        }

        private List<Guid> GetSelectedAttempts()
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
    }
}