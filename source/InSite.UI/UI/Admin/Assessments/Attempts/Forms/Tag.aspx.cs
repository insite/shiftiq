using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Attempts.Forms
{
    public partial class Tag : AdminBasePage
    {
        #region Constants

        private const int PageSize = 20;

        private const string SearchUrl = "/ui/admin/assessments/attempts/reports/search";

        #endregion

        #region Properties

        private Dictionary<Guid, string> Changes
        {
            get => (Dictionary<Guid, string>)ViewState[nameof(Changes)];
            set => ViewState[nameof(Changes)] = value;
        }

        #endregion

        #region Initializtion and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            NextButton.Click += NextButton_Click;
            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this);

            LoadAttempts();

            CancelButton1.NavigateUrl = SearchUrl;
            CancelButton2.NavigateUrl = SearchUrl;
        }

        #endregion

        #region Event handlers

        private void NextButton_Click(object sender, EventArgs e)
        {
            GetChanges();
            LoadChanges();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveChanges();

            HttpResponseHelper.Redirect(SearchUrl);
        }

        #endregion

        #region Get and save changes

        private void GetChanges()
        {
            Changes = new Dictionary<Guid, string>();

            foreach (RepeaterItem repeaterItem in AttemptRepeater.Items)
            {
                var oldAttemptTag = ((ITextControl)repeaterItem.FindControl("OldAttemptTag")).Text;
                var attemptTag = ((ITextBox)repeaterItem.FindControl("AttemptTag")).Text;

                if (string.Equals(oldAttemptTag, attemptTag))
                    continue;

                var attemptIdentifierLiteral = (ITextControl)repeaterItem.FindControl("AttemptIdentifier");
                var attemptIdentifier = Guid.Parse(attemptIdentifierLiteral.Text);

                Changes.Add(attemptIdentifier, attemptTag);
            }
        }

        private void SaveChanges()
        {
            foreach (var keyValue in Changes)
                ServiceLocator.SendCommand(new TagAttempt(keyValue.Key, keyValue.Value));
        }

        #endregion

        #region Data binding

        private void LoadChanges()
        {
            ChangesTab.Visible = true;
            ChangesTab.IsSelected = true;

            NoChanges.Visible = Changes.Count == 0;
            ChangeRepeater.Visible = Changes.Count > 0;
            SaveButton.Visible = Changes.Count > 0;

            if (Changes.Count == 0)
                return;

            var data = GetAttempts().Where(x => Changes.ContainsKey(x.AttemptIdentifier)).ToList();

            foreach (var attempt in data)
                attempt.AttemptTag = Changes[attempt.AttemptIdentifier];

            ChangeRepeater.DataSource = data;
            ChangeRepeater.DataBind();
        }

        private void LoadAttempts()
        {
            var data = GetAttempts();
            var hasData = data.IsNotEmpty();

            NoAttempts.Visible = !hasData;
            AttemptRepeater.Visible = hasData;
            NextButton.Visible = hasData;

            if (!hasData)
                return;

            AttemptRepeater.DataSource = data;
            AttemptRepeater.DataBind();

            AttemptsTab.SetTitle("Attempts", data.Count);
        }

        private List<QAttempt> GetAttempts()
        {
            var searchAction = SearchUrl.Substring(1);
            var report = TReportSearch.SelectFirst(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Search
                  && x.ReportDescription == searchAction);
            var settings = ReportRequest.Deserialize(report?.ReportData)?.GetSearch<QAttemptFilter>();

            settings.Filter.Paging = Paging.SetPage(settings.PageIndex + 1, PageSize);

            return settings != null
                ? ServiceLocator.AttemptSearch.GetAttempts(settings.Filter, x => x.Form, x => x.LearnerPerson)
                : null;
        }

        private int GetSkip(int page) => page * PageSize;

        protected string FormatTime(object o)
        {
            var html = new StringBuilder();

            var attempt = (QAttempt)o;
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

        #endregion
    }
}