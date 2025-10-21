using System;
using System.Collections.Generic;
using System.Linq;

using Common.Timeline.Commands;

using Humanizer;

using InSite.Application.Banks.Read;
using InSite.Application.Gradebooks.Write;
using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Application.Records.Write;
using InSite.Application.Surveys.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Records.Programs.Utilities
{
    public class ProgramHelper
    {
        public class TaskObjectData
        {
            public Guid ObjectIdentifier { get; set; }
            public string ObjectType { get; set; }
            public string ObjectTitle { get; set; }
        }

        public static TaskInfo GetTaskInfo(List<TaskObjectData> taskObjects, TTask task)
        {
            if (taskObjects == null || taskObjects.Count == 0) return null;

            var taskObject = taskObjects.FirstOrDefault(x => x.ObjectIdentifier == task.ObjectIdentifier);

            if (taskObject == null) return null;

            return new TaskInfo()
            {
                ObjectIdentifier = task.ObjectIdentifier,
                TaskTitle = taskObject.ObjectTitle,
                DisplayTitle = $"[{task.ObjectType}] {taskObject.ObjectTitle}",
                Type = task.ObjectType,
                TaskIdentifier = task.TaskIdentifier,
                Sequence = task.TaskSequence
            };
        }

        public static List<TaskInfo> BindTaskInfo(List<TaskObjectData> taskObjectData, List<TTask> tasks)
        {
            var taskInfoContainer = new List<TaskInfo>();

            foreach (var task in tasks)
            {
                var taskInfo = GetTaskInfo(taskObjectData, task);
                if (taskInfo == null)
                    continue;
                taskInfoContainer.Add(taskInfo);
            }

            return taskInfoContainer;
        }

        public static List<TaskObjectData> GetTaskObjectData(Guid Organization, Guid? ParentOrganization = null)
        {
            var results = new List<TaskObjectData>();

            GetAchievementsObjectData(Organization, results, ParentOrganization);
            GetLogbooksObjectData(Organization, results);
            GetSurveysObjectData(Organization, results);
            GetCoursesObjectData(Organization, results);
            GetAssessmentBankObjectData(Organization, results);
            GetAssessmentFormsObjectData(Organization, results);

            return results;
        }

        public static void EnsureLogbookEnrollment(Guid userIdentifier, Guid taskObjectIdentifier)
        {
            var learner = ServiceLocator.JournalSearch
                .GetJournalSetupUser(taskObjectIdentifier, userIdentifier, JournalSetupUserRole.Learner);

            if (learner == null)
            {
                ServiceLocator.SendCommand(
                    new AddJournalSetupUser(
                        taskObjectIdentifier,
                        userIdentifier,
                        JournalSetupUserRole.Learner));
            }
        }

        public static void EnsureCourseEnrollment(Guid userIdentifier, Guid taskObjectIdentifier)
        {
            var gradebook = CourseSearch.SelectCourse(taskObjectIdentifier)?.GradebookIdentifier;

            if (gradebook.HasValue)
            {
                if (!ServiceLocator.RecordSearch.EnrollmentExists(gradebook.Value, userIdentifier))
                    ServiceLocator.SendCommand(new AddEnrollment(gradebook.Value, UniqueIdentifier.Create(), userIdentifier, null, DateTimeOffset.Now, null));
            }
        }

        public static void EnsureCourseEnrollmentDeletion(Guid userIdentifier, Guid taskObjectIdentifier, Guid organizationIdentifier)
        {
            var gradebook = CourseSearch.SelectCourse(taskObjectIdentifier)?.GradebookIdentifier;

            if (gradebook.HasValue)
            {
                if (ServiceLocator.RecordSearch.EnrollmentExists(gradebook.Value, userIdentifier) &&
                    !TaskSearch.TaskExistInOtherProgram(organizationIdentifier, taskObjectIdentifier, userIdentifier))
                    ServiceLocator.SendCommand(new DeleteEnrollment(gradebook.Value, userIdentifier));
            }
        }

        public static void EnsureLogbookEnrollmentDeletion(Guid userIdentifier, Guid taskObjectIdentifier, Guid organizationIdentifier)
        {
            var learner = ServiceLocator.JournalSearch
                .GetJournalSetupUser(taskObjectIdentifier, userIdentifier, JournalSetupUserRole.Learner);

            if (learner != null && !TaskSearch.TaskExistInOtherProgram(organizationIdentifier, taskObjectIdentifier, userIdentifier))
                ServiceLocator.SendCommand(new DeleteJournalSetupUser(taskObjectIdentifier, userIdentifier, JournalSetupUserRole.Learner));
        }

        public static void EnsureProgramAchievementEnrollement(Guid? programId, Guid objectIdentifier, Guid taskId, VProgramEnrollment programUser)
        {
            var program = programId.HasValue ? ProgramSearch.GetProgram(programId.Value) : null;

            if (program != null && objectIdentifier == program.AchievementIdentifier)
            {
                var credential = ServiceLocator.AchievementSearch.GetCredential(objectIdentifier, programUser.UserIdentifier);

                if (credential != null)
                    TaskStore.CompleteTaskEnrollementFoLearner(programId.Value, taskId, objectIdentifier, programUser.UserIdentifier);
            }
        }

        public static IEnumerable<Shift.Common.ListItem> GetTaskObjects(string objectType, Guid organizationId, Guid? parentOrganizationId = null)
            => GetTaskListItemBasedOnObjectType(objectType, organizationId, parentOrganizationId);

        public static string GenerateTitle(string objectType)
        {
            var title = objectType.Pluralize();
            return title;
        }

        public static void SendGrantCommands(TriggerEffectCommand effect, Guid organization, Guid achievement, Guid user)
        {
            var commands = new List<Command>();

            ProgramEnrollment.BuildCommands(effect, organization, achievement, user, commands, ServiceLocator.AchievementSearch, ServiceLocator.ContactSearch);

            ServiceLocator.SendCommands(commands);
        }

        #region Helper Methods

        private static List<Shift.Common.ListItem> GetTaskListItemBasedOnObjectType(string objectType, Guid organizationId, Guid? parentOrganizationId)
        {
            var list = new List<Shift.Common.ListItem>();

            switch (objectType)
            {
                case "Assessment":
                    GetAssesmentTaskListItems(organizationId, list);
                    return list;
                case "Achievement":
                    GetAchievementTaskListItems(organizationId, list, parentOrganizationId);
                    return list;
                case "Logbook":
                    GetLogbookTaskListItems(organizationId, list);
                    return list;
                case "Survey":
                    GetSurveyTaskListItems(organizationId, list);
                    return list;
                case "Course":
                    GetCourseTaskListItems(organizationId, list);
                    return list;
            }

            return list;
        }

        private static void GetAssesmentTaskListItems(Guid organizationId, List<Shift.Common.ListItem> list)
        {
            var formItems = ServiceLocator.BankSearch.GetForms(
            new QBankFormFilter()
            {
                OrganizationIdentifier = organizationId,
                IncludeFormStatus = "Published"
            }, x => x.Bank)
            .Select(x => new ChildItem(x.FormName, x.Bank.BankName, x.FormIdentifier, x.BankIdentifier)).ToList();

            var bankItems = formItems.GroupBy(d => new { d.ParentId, d.ParentName })
                .Select(m => new ParentItem(m.Key.ParentName, m.Key.ParentId)).ToList();

            foreach (var item in bankItems)
                list.Add(new Shift.Common.ListItem { Text = item.Name, Value = item.Id.ToString() });
        }

        private static void GetAchievementTaskListItems(Guid organizationId, List<Shift.Common.ListItem> list, Guid? parentOrganizationId)
        {
            var items = ServiceLocator.AchievementSearch.GetAchievements(new QAchievementFilter { OrganizationIdentifier = organizationId, ParentOrganizationIdentifier = parentOrganizationId });
            foreach (var item in items)
                list.Add(new Shift.Common.ListItem { Text = item.AchievementTitle, Value = item.AchievementIdentifier.ToString() });
        }

        private static void GetLogbookTaskListItems(Guid organizationId, List<Shift.Common.ListItem> list)
        {
            var items = ServiceLocator.JournalSearch.GetJournalSetups(new QJournalSetupFilter
            {
                OrganizationIdentifier = organizationId,
                OrderBy = "JournalSetupName"
            });

            foreach (var item in items)
                list.Add(new Shift.Common.ListItem { Text = item.JournalSetupName, Value = item.JournalSetupIdentifier.ToString() });
        }

        private static void GetSurveyTaskListItems(Guid organizationId, List<Shift.Common.ListItem> list)
        {
            var items = ServiceLocator.SurveySearch.GetSurveyForms(new QSurveyFormFilter { OrganizationIdentifier = organizationId });
            foreach (var item in items)
                list.Add(new Shift.Common.ListItem { Text = item.SurveyFormName, Value = item.SurveyFormIdentifier.ToString() });
        }

        private static void GetCourseTaskListItems(Guid organizationId, List<Shift.Common.ListItem> list)
        {
            var items = CourseSearch.BindCourses(x => x, x => x.OrganizationIdentifier == organizationId && x.Gradebook.AchievementIdentifier != null).ToList();
            foreach (var item in items)
                list.Add(new Shift.Common.ListItem { Text = item.CourseName, Value = item.CourseIdentifier.ToString() });
        }

        private static void GetAchievementsObjectData(Guid Organization, List<TaskObjectData> results, Guid? ParentOrganization)
        {
            var achievementsItems = ServiceLocator.AchievementSearch
                .GetAchievements(
                    new QAchievementFilter { OrganizationIdentifier = Organization, ParentOrganizationIdentifier = ParentOrganization })
                .Select(x => new TaskObjectData()
                {
                    ObjectIdentifier = x.AchievementIdentifier,
                    ObjectTitle = x.AchievementTitle,
                    ObjectType = "Achievement"

                }).ToList();
            results.AddRange(achievementsItems);
        }

        private static void GetLogbooksObjectData(Guid Organization, List<TaskObjectData> results)
        {
            var logbookItems = ServiceLocator.JournalSearch
                .GetJournalSetups(new QJournalSetupFilter
                {
                    OrganizationIdentifier = Organization,
                    OrderBy = "JournalSetupName"
                })
                .Select(x => new TaskObjectData()
                {
                    ObjectIdentifier = x.JournalSetupIdentifier,
                    ObjectTitle = x.JournalSetupName,
                    ObjectType = "Logbook"

                }).ToList();
            results.AddRange(logbookItems);
        }

        private static void GetSurveysObjectData(Guid Organization, List<TaskObjectData> results)
        {
            var surveyItems = ServiceLocator.SurveySearch
                .GetSurveyForms(
                    new QSurveyFormFilter { OrganizationIdentifier = Organization })
                .Select(x => new TaskObjectData()
                {
                    ObjectIdentifier = x.SurveyFormIdentifier,
                    ObjectTitle = x.SurveyFormName,
                    ObjectType = "Survey"

                }).ToList();
            results.AddRange(surveyItems);
        }

        private static void GetCoursesObjectData(Guid Organization, List<TaskObjectData> results)
        {
            var coursItems = CourseSearch.BindCourses(x => x, x => x.OrganizationIdentifier == Organization)
                .Select(x => new TaskObjectData()
                {
                    ObjectIdentifier = x.CourseIdentifier,
                    ObjectTitle = x.CourseName,
                    ObjectType = "Course"

                }).ToList();
            results.AddRange(coursItems);
        }

        private static void GetAssessmentBankObjectData(Guid Organization, List<TaskObjectData> results)
        {
            var bankItems = ServiceLocator.BankSearch.GetBanks(new QBankFilter() { OrganizationIdentifier = Organization })
                .Select(x => new TaskObjectData()
                {
                    ObjectIdentifier = x.BankIdentifier,
                    ObjectTitle = x.BankTitle,
                    ObjectType = "Assessment"

                }).ToList();
        }

        private static void GetAssessmentFormsObjectData(Guid Organization, List<TaskObjectData> results)
        {
            var formItems = ServiceLocator.BankSearch
                .GetForms(
                    new QBankFormFilter() { OrganizationIdentifier = Organization, IncludeFormStatus = "Published" })
                .Select(x => new TaskObjectData()
                {
                    ObjectIdentifier = x.FormIdentifier,
                    ObjectTitle = x.FormName,
                    ObjectType = "AssessmentForm"

                }).ToList();
            results.AddRange(formItems);
        }

        #endregion
    }
}