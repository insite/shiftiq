using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Gradebooks.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class EnrollmentUserGrid : SearchResultsGridViewController<QEnrollmentFilter>
    {
        private class EnrollmentUserGridItem
        {
            public Guid LearnerIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }
            public string UserFullName { get; set; }
            public string UserEmail { get; set; }
            public string CourseName { get; set; }
            public DateTimeOffset? EnrollmentStarted { get; set; }
            public DateTimeOffset? CourseCompleted { get; set; }
            public string TimeTaken { get; set; }
            public string CompletionCounter { get; set; }
            public string CompletionPercent { get; set; }
        }

        public event EventHandler Refreshed;

        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        protected override bool IsFinder => false;

        private Guid CourseId
        {
            get => (Guid)(ViewState[nameof(CourseId)] ?? Guid.Empty);
            set => ViewState[nameof(CourseId)] = value;
        }

        private string CourseName
        {
            get => (string)ViewState[nameof(CourseName)];
            set => ViewState[nameof(CourseName)] = value;
        }

        private Guid GradebookIdentifier
        {
            get => (Guid)(ViewState[nameof(GradebookIdentifier)] ?? Guid.Empty);
            set => ViewState[nameof(GradebookIdentifier)] = value;
        }

        private int CourseGradeItemCount
        {
            get => (int)(ViewState[nameof(CourseGradeItemCount)] ?? 0);
            set => ViewState[nameof(CourseGradeItemCount)] = value;
        }

        internal bool IsCompletedFilter => EnrollmentStatus.Value == "Completed";

        protected override string ExportFileName => "course-enrollments";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDeleting += Grid_RowDeleting;

            EnrollmentStatus.AutoPostBack = true;
            EnrollmentStatus.ValueChanged += (s, a) =>
            {
                if (GradebookIdentifier != Guid.Empty)
                {
                    Search(BuildFilter(GradebookIdentifier));
                    OnRefreshed();
                }
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ScriptManager.GetCurrent(Page).RegisterPostBackControl(DownloadDropDown);
        }

        private void Grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var grid = (Grid)sender;
            var learnerId = grid.GetDataKey<Guid>(e);

            ServiceLocator.SendCommand(new DeleteEnrollment(Filter.GradebookIdentifier, learnerId));

            Refresh();
            OnRefreshed();
        }

        public void LoadData(Guid courseId, string courseName, Guid gradebookId, bool completedOnly)
        {
            CourseId = courseId;
            CourseName = courseName;
            GradebookIdentifier = gradebookId;
            CourseGradeItemCount = CourseSearch.CountCourseGradeItems(courseId);
            EnrollmentStatus.Value = completedOnly ? "Completed" : "Started";

            Search(BuildFilter(gradebookId));
        }

        public void Refresh()
        {
            SearchWithCurrentPageIndex(Filter);
        }

        private QEnrollmentFilter BuildFilter(Guid gradebookId) => new QEnrollmentFilter
        {
            GradebookIdentifier = gradebookId,
            CompletedCourseIdentifier = IsCompletedFilter ? CourseId : (Guid?)null
        };

        protected override int SelectCount(QEnrollmentFilter filter)
        {
            return ServiceLocator.RecordSearch.CountEnrollments(filter);
        }

        protected override IListSource SelectData(QEnrollmentFilter filter)
        {
            if (filter == null)
                return null;

            var enrollments = ServiceLocator.RecordSearch.GetEnrollments(filter, x => x.Learner);
            if (enrollments.IsEmpty())
                return new List<EnrollmentUserGridItem>().ToSearchResult();

            var learnerIds = enrollments.Select(x => x.LearnerIdentifier).Distinct().ToArray();
            var progresses = CourseSearch.GetCourseLearnerProgress(CourseId, filter.GradebookIdentifier, learnerIds)
                .ToDictionary(x => x.LearnerIdentifier);

            var items = new List<EnrollmentUserGridItem>(enrollments.Count);
            foreach (var enrollment in enrollments)
            {
                int completedCount = 0;
                DateTimeOffset? courseCompleted = null;

                if (progresses.TryGetValue(enrollment.LearnerIdentifier, out var progress))
                {
                    completedCount = progress.CompletedCount;
                    courseCompleted = progress.CourseCompleted;
                }

                items.Add(new EnrollmentUserGridItem
                {
                    LearnerIdentifier = enrollment.LearnerIdentifier,
                    UserIdentifier = enrollment.LearnerIdentifier,
                    UserFullName = enrollment.Learner?.UserFullName,
                    UserEmail = enrollment.Learner?.UserEmail,
                    CourseName = CourseName,
                    EnrollmentStarted = enrollment.EnrollmentStarted,
                    CourseCompleted = courseCompleted,
                    TimeTaken = FormatTimeTaken(enrollment.EnrollmentStarted, courseCompleted),
                    CompletionCounter = CourseGradeItemCount > 0 ? $"{completedCount}/{CourseGradeItemCount}" : "0/0",
                    CompletionPercent = CourseGradeItemCount > 0 ? $"{Math.Round((double)completedCount / CourseGradeItemCount * 100)}%" : string.Empty
                });
            }

            return items.ToSearchResult();
        }

        protected override IEnumerable<DownloadColumn> GetDownloadColumns(IList dataList) => new[]
        {
            new DownloadColumn("CourseName", "Course"),
            new DownloadColumn("UserFullName", "Learner"),
            new DownloadColumn("UserEmail", "Email"),
            new DownloadColumn("EnrollmentStarted", "Enrolled"),
            new DownloadColumn("CourseCompleted", "Completed"),
            new DownloadColumn("TimeTaken", "Time Taken"),
            new DownloadColumn("CompletionCounter", "Progress"),
            new DownloadColumn("CompletionPercent", "Completion")
        };

        private static string FormatTimeTaken(DateTimeOffset? start, DateTimeOffset? end)
        {
            if (!start.HasValue)
                return string.Empty;

            var completed = end ?? DateTimeOffset.Now;
            if (completed < start.Value)
                return string.Empty;

            var diff = completed - start.Value;
            if (diff.TotalDays >= 1)
                return "day".ToQuantity((int)diff.TotalDays, "N0");

            if (diff.TotalHours >= 1)
                return "hour".ToQuantity((int)diff.TotalHours, "N0");

            return "< 1 hour";
        }
    }
}
