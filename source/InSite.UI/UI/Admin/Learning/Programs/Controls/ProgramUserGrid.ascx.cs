﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;

using Humanizer;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class ProgramUserGrid : SearchResultsGridViewController<VProgramEnrollmentFilter>
    {
        protected override bool IsFinder => false;

        public bool IsFilterVisible
        {
            get => FilterSection.Visible;
            set => FilterSection.Visible = value;
        }

        public bool IsDownloadVisible
        {
            get => DownloadDropDown.Visible;
            set => DownloadDropDown.Visible = value;
        }

        public int ProgramTaskCount
        {
            get => (int)ViewState[nameof(ProgramTaskCount)];
            set => ViewState[nameof(ProgramTaskCount)] = value;
        }

        public Guid? ProgramAchievement
        {
            get => (Guid?)ViewState[nameof(ProgramAchievement)];
            set => ViewState[nameof(ProgramAchievement)] = value;
        }

        public Guid? ProgramIdentifier
        {
            get => (Guid?)ViewState[nameof(ProgramIdentifier)];
            set => ViewState[nameof(ProgramIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RefreshButton.Click += RefreshButton_Click;
            SearchInput.Click += FilterButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ScriptManager.GetCurrent(Page).RegisterPostBackControl(DownloadDropDown);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            if (ProgramIdentifier == null)
                return;

            TaskStore.EnrollUserToProgramTasks(Organization.Identifier, ProgramIdentifier.Value);
            FilterButton_Click(sender, e);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (Filter.ProgramIdentifier.HasValue)
                Filter.UserFullName = SearchInput.Text;
            else if (Filter.UserIdentifier.HasValue)
                Filter.ProgramName = SearchInput.Text;

            RefreshGrid();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ProgramUserGrid),
                "filter_focus",
                $"inSite.common.baseInput.focus('{SearchInput.ClientID}', true);",
                true);
        }

        public void LoadDataByProgram(Guid program, Guid? programAchievement, bool showActions, string returnUrl)
        {
            ProgramIdentifier = program;

            Filter = new VProgramEnrollmentFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                ProgramIdentifier = program
            };

            ProgramAchievement = programAchievement;
            Grid.Columns.FindByHeaderText("Program").Visible = false;
            Grid.Columns.FindByName("ActionColumn").Visible = showActions;

            ProgramTaskCount = ProgramSearch1
                .GetProgramTasks(
                new TTaskFilter 
                { 
                    ProgramIdentifier = program, 
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    ExcludedObject = ProgramAchievement,
                    ExcludeObjectTypes = new string[] { "Assessment" }
                }).Count;

            var returnUrlParam = !string.IsNullOrEmpty(returnUrl) ? "&return=" + HttpUtility.UrlEncode(returnUrl) : "";

            AddButton.NavigateUrl = $"/ui/admin/learning/programs/enrollments/add?program={program}" + returnUrlParam;
            AddButton.Visible = true;

            Search(Filter);
        }

        public void LoadDataByUser(Guid user)
        {
            Filter = new VProgramEnrollmentFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                UserIdentifier = user
            };

            Grid.Columns.FindByHeaderText("Learner").Visible = false;

            AddButton.Visible = false;

            Search(Filter);
        }

        protected override int SelectCount(VProgramEnrollmentFilter filter)
        {
            return ProgramSearch1.CountProgramUsers(filter);
        }

        protected override IListSource SelectData(VProgramEnrollmentFilter filter)
        {
            if (filter == null)
                return null;

            ProgramIdentifier = filter.ProgramIdentifier;

            var enrollments = ProgramSearch1
                .GetProgramUsers(filter);

            var taskEnrollments = TaskSearch.GetProgramTaskEnrollments(Organization.Identifier, filter.ProgramIdentifier.Value);

            List<VProgramEnrollmentExtended> results = new List<VProgramEnrollmentExtended>();
            foreach (var enrollment in enrollments)
            {
                var result = new VProgramEnrollmentExtended()
                {
                    CompletionCounter = $"0/{ProgramTaskCount}",
                    UserIdentifier = enrollment.UserIdentifier,
                    EnrollmentIdentifier = enrollment.UserIdentifier,
                    OrganizationIdentifier = enrollment.OrganizationIdentifier,
                    ProgramCode = enrollment.ProgramCode,
                    ProgramDescription = enrollment.ProgramDescription,
                    ProgramIdentifier = enrollment.ProgramIdentifier,
                    ProgramName = enrollment.ProgramName,
                    ProgressAssigned = enrollment.ProgressAssigned,
                    ProgressCompleted = enrollment.ProgressCompleted,
                    ProgressStarted = enrollment.ProgressStarted,
                    TimeTaken = enrollment.TimeTaken,
                    UserEmail = enrollment.UserEmail,
                    UserEmailAlternate = enrollment.UserEmailAlternate,
                    UserFullName = enrollment.UserFullName,
                    UserPhone = enrollment.UserPhone,
                };

                var counter = taskEnrollments.Where(x => x.LearnerUserIdentifier == enrollment.UserIdentifier && x.ObjectIdentifier != ProgramAchievement).ToList();

                if (counter != null && counter.Count > 0)
                {
                    var completionCount = counter.Where(x => x.ProgressCompleted != null).Count();
                    result.CompletionCounter = $"{completionCount}/{ProgramTaskCount}";
                    if (ProgramTaskCount > 0)
                        result.CompletionPercent = string.Format("{0}%", Math.Round((double)completionCount / ProgramTaskCount * 100));
                }

                result.LearnerNameLink = ((AdminBasePage)Page).Navigator.IsCmds
                    ? $"<a href='/ui/cmds/admin/users/edit?userID={result.UserIdentifier}'>{result.UserFullName}</a>"
                    : $"<a href='/ui/admin/contacts/people/edit?contact={result.UserIdentifier}'>{result.UserFullName}</a>";

                var link = $"<a href='/ui/admin/learning/programs/enrollments/remove?program={result.ProgramIdentifier}&user={result.UserIdentifier}&return={HttpUtility.UrlEncode(Request.RawUrl)}' title='Delete enrollment'><i class='fas fa-trash-alt'></i></a>";
                result.DeleteEnrollmentLink = link;

                if (result.TimeTaken.HasValue && result.TimeTaken.Value >= 0)
                    result.DaysTaken = "day".ToQuantity(result.TimeTaken.Value, "N0");

                if (enrollment.CreatedWhen != null && enrollment.CreatedWho != null)
                {
                    var when = TimeZones.Format(enrollment.CreatedWhen.Value, User.TimeZone);
                    var tooltip = $"<span data-bs-toggle=\"tooltip\" data-bs-placement=\"bottom\" data-bs-html=\"true\" title='{enrollment.CreatedWho} assigned <div>{when}</div>'><i class='far fa-info-circle ms-1'></i></span>";
                    result.EnrollmentToolTip = tooltip;
                }

                results.Add(result);
            }

            return results.ToSearchResult();
        }

        protected override IEnumerable<DownloadColumn> GetDownloadColumns(IList dataList)
        {
            return new[]
            {
                new DownloadColumn("ProgramName", "Program"),
                new DownloadColumn("UserFullName", "Learner"),
                new DownloadColumn("UserEmail", "Email"),
                new DownloadColumn("UserPhone", "Phone"),
                new DownloadColumn("ProgressAssigned", "Assigned"),
                new DownloadColumn("CompletionCounter", "Progress"),
                new DownloadColumn("CompletionPercent", "Completion")
            };
        }
    }
}