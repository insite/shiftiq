using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Application.Surveys.Read;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    static class FormHelper
    {
        public static List<ICommand> GetResponseGradebookCommands(QResponseSession response, QSurveyForm surveyForm, string newStatus)
        {
            var activity = ServiceLocator.CourseSearch.GetActivityBySurveyForm(surveyForm.SurveyFormIdentifier);

            if (activity?.GradeItemIdentifier == null)
                return null;

            var gradeItem = ServiceLocator.RecordSearch.GetGradeItem(activity.GradeItemIdentifier.Value);

            if (gradeItem == null)
                return null;

            var commands = new List<ICommand>();

            EnsureEnrollment(response.RespondentUserIdentifier, gradeItem.GradebookIdentifier, commands);

            var (progress, progressId) = EnsureProgress(response.RespondentUserIdentifier, gradeItem.GradebookIdentifier, activity.GradeItemIdentifier.Value, commands);

            CreateChangeCommands(progress, progressId, newStatus, commands);

            return commands;
        }

        private static void EnsureEnrollment(Guid userId, Guid gradebookId, List<ICommand> commands)
        {
            if (ServiceLocator.RecordSearch.EnrollmentExists(gradebookId, userId))
                return;

            var command= ServiceLocator.RecordSearch.CreateCommandToAddEnrollment(null, gradebookId, userId, null, DateTimeOffset.UtcNow, null);

            commands.Add(command);
        }

        private static (QProgress, Guid) EnsureProgress(Guid userId, Guid gradebookId, Guid gradeItemId, List<ICommand> commands)
        {
            var progress = ServiceLocator.RecordSearch.GetGradebookScores(
                new QProgressFilter
                {
                    GradebookIdentifier = gradebookId,
                    GradeItemIdentifier = gradeItemId,
                    StudentUserIdentifier = userId
                }
            ).FirstOrDefault();

            Guid progressId;

            if (progress != null)
            {
                progressId = progress.ProgressIdentifier;
            }
            else
            {
                var command = ServiceLocator.RecordSearch.CreateCommandToAddProgress(null, gradebookId, gradeItemId, userId);
                commands.Add(command);

                progressId = command.AggregateIdentifier;
            }

            return (progress, progressId);
        }

        private static void CreateChangeCommands(QProgress progress, Guid progressIdentifier, string newScore, List<ICommand> commands)
        {
            if (string.Equals(progress?.ProgressStatus ?? "", newScore ?? "", StringComparison.OrdinalIgnoreCase))
                return;

            if (newScore == "Started")
            {
                if (string.Equals(progress?.ProgressStatus, "Completed", StringComparison.OrdinalIgnoreCase))
                {
                    commands.Add(new DeleteProgress(progressIdentifier));
                    commands.Add(new AddProgress(progressIdentifier, progress.GradebookIdentifier, progress.GradeItemIdentifier, progress.UserIdentifier));
                }

                commands.Add(new StartProgress(progressIdentifier, DateTimeOffset.UtcNow));
                return;
            }

            if (newScore == "Completed")
            {
                commands.Add(new CompleteProgress(progressIdentifier, DateTimeOffset.UtcNow, null, null, null));
                return;
            }

            commands.Add(new DeleteProgress(progressIdentifier));
        }
    }
}