using System;
using System.ComponentModel;
using System.Linq;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.UI.Portal.Contacts.People.Controls
{
    public partial class PersonPrograms : SearchResultsGridViewController<VProgramEnrollmentFilter>
    {
        protected override bool IsFinder => false;

        public int LoadData(Guid organizationId, Guid userId)
        {
            var filter = new VProgramEnrollmentFilter
            {
                OrganizationIdentifier = organizationId,
                UserIdentifier = userId
            };

            Search(filter);

            NoPrograms.Visible = RowCount == 0;
            Grid.Visible = RowCount > 0;

            return RowCount;
        }

        protected override int SelectCount(VProgramEnrollmentFilter filter)
        {
            return ProgramSearch1.CountProgramUsers(filter);
        }

        protected override IListSource SelectData(VProgramEnrollmentFilter filter)
        {
            if (filter == null)
                return null;

            var enrollments = ProgramSearch1.GetProgramUsers(filter);

            var taskEnrollments = TaskSearch.GetUserTaskEnrollments(filter.OrganizationIdentifier, filter.UserIdentifier.Value);
            var programIds = enrollments.Select(x => x.ProgramIdentifier).ToList();

            var programs = ProgramSearch1.Bind(x => new
                {
                    x.ProgramIdentifier,
                    x.AchievementIdentifier,
                    TaskCount = x.Tasks.Count(t => t.ObjectType != "Assessment" && t.ObjectIdentifier != x.AchievementIdentifier)
                },
                x => programIds.Contains(x.ProgramIdentifier)
            );

            var result = enrollments.Select(e =>
                {
                    var program = programs.Single(x => x.ProgramIdentifier == e.ProgramIdentifier);
                    var counter = taskEnrollments.Where(x => x.Task.ProgramIdentifier == e.ProgramIdentifier && x.ObjectIdentifier != program.AchievementIdentifier).ToList();
                    var completionCount = counter.Count(x => x.ProgressCompleted != null);

                    return new
                    {
                        ProgramName = e.ProgramName,
                        ProgressAssigned = e.ProgressAssigned,
                        ProgressCompleted = e.ProgressCompleted,
                        DaysTaken = e.TimeTaken.HasValue && e.TimeTaken.Value >= 0 ? "day".ToQuantity(e.TimeTaken.Value, "N0") : null,
                        CompletionCounter = counter.Count > 0 ? $"{completionCount}/{program.TaskCount}" : null,
                        CompletionPercent = counter.Count > 0 && program.TaskCount > 0 ? string.Format("{0}%", Math.Round((double)completionCount / program.TaskCount * 100)) : null,
                    };
                })
                .ToList();

            return result.ToSearchResult();
        }
    }
}
