using System;
using System.Collections.Generic;
using System.Linq;

using Humanizer;

using InSite.Application.Banks.Read;
using InSite.Application.Gradebooks.Write;
using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Application.Records.Write;
using InSite.Application.Surveys.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Timeline.Commands;
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
            if (taskObjects == null || taskObjects.Count == 0)
                return null;

            var taskObject = taskObjects.FirstOrDefault(x => x.ObjectIdentifier == task.ObjectIdentifier);
            if (taskObject == null)
                return null;

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

        public static List<TaskObjectData> GetTaskObjectData(Guid organization)
        {
            var results = new List<TaskObjectData>();

            GetAchievementsObjectData(organization, results);
            GetLogbooksObjectData(organization, results);
            GetSurveysObjectData(organization, results);
            GetCoursesObjectData(organization, results);
            GetAssessmentBankObjectData(organization, results);
            GetAssessmentFormsObjectData(organization, results);

            return results;
        }

        public static bool EnrollLearnerByObjectId(Guid organizationId, Guid userId, Guid objectId)
        {
            if (ServiceLocator.ProgramSearch.IsTaskEnrollmentExist(userId, objectId))
                return true;

            var programId = ServiceLocator.ProgramSearch.GetGroupEnrollmentProgramId(userId, objectId);
            if (programId == null)
                return false;

            EnrollLearner(organizationId, programId.Value, userId);

            return true;
        }

        public static void EnrollLearner(Guid organizationId, Guid programId, Guid userId)
        {
            var tasks = TaskStore.EnrollUserToProgramTasks(organizationId, programId, userId);

            if (tasks != null && tasks.Length > 0)
            {
                foreach (var task in tasks.Where(x => x.ObjectType == "Course"))
                    EnsureCourseEnrollment(userId, task.ObjectIdentifier);

                foreach (var task in tasks.Where(x => x.ObjectType == "Logbook"))
                    EnsureLogbookEnrollment(userId, task.ObjectIdentifier);
            }

            ServiceLocator.ProgramService.CompletionOfProgramAchievement(programId, userId, organizationId);
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
            var gradebookId = CourseSearch.SelectCourse(taskObjectIdentifier)?.GradebookIdentifier;

            if (gradebookId.HasValue)
            {
                if (!ServiceLocator.RecordSearch.EnrollmentExists(gradebookId.Value, userIdentifier))
                    ServiceLocator.SendCommand(new AddEnrollment(gradebookId.Value, UniqueIdentifier.Create(), userIdentifier, null, DateTimeOffset.Now, null));
            }
        }

        public static void EnsureCourseEnrollmentDeletion(Guid userIdentifier, Guid taskObjectIdentifier, Guid organizationIdentifier)
        {
            var gradebookId = CourseSearch.SelectCourse(taskObjectIdentifier)?.GradebookIdentifier;

            if (gradebookId.HasValue)
            {
                if (ServiceLocator.RecordSearch.EnrollmentExists(gradebookId.Value, userIdentifier) &&
                    !TaskSearch.TaskExistInOtherProgram(organizationIdentifier, taskObjectIdentifier, userIdentifier))
                    ServiceLocator.SendCommand(new DeleteEnrollment(gradebookId.Value, userIdentifier));
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
            if (program == null || objectIdentifier != program.AchievementIdentifier)
                return;

            var credential = ServiceLocator.AchievementSearch.GetCredential(objectIdentifier, programUser.UserIdentifier);
            if (credential != null)
                TaskStore.CompleteTaskEnrollementFoLearner(programId.Value, taskId, objectIdentifier, programUser.UserIdentifier);
        }

        public static IEnumerable<Shift.Common.ListItem> GetTaskObjects(string objectType, Guid organizationId, bool includePartitionItems = false)
            => GetTaskListItemBasedOnObjectType(objectType, organizationId, includePartitionItems);

        public static (List<TTask>, List<ProgramTaskItem>) GetTasksAndItems(Guid? programId, string objectType, Guid organizationId, bool includePartitionItems = false)
        {
            var objects = GetTaskListItemBasedOnObjectType(objectType, organizationId, includePartitionItems);

            var filter = new TTaskFilter
            {
                ProgramIdentifier = programId
            };

            filter.OrganizationIdentifiers.Add(organizationId);

            if (includePartitionItems && organizationId != ServiceLocator.Partition.Identifier)
                filter.OrganizationIdentifiers.Add(ServiceLocator.Partition.Identifier);

            var programTasks = ProgramSearch1.GetProgramTasks(filter);
            var items = new List<ProgramTaskItem>();

            if (!programId.HasValue)
                return (programTasks, items);

            var tasksIndex = programTasks.ToDictionary(x => x.ObjectIdentifier, x => x);

            foreach (var o in objects)
            {
                var objectId = Guid.Parse(o.Value);
                var task = tasksIndex.GetOrDefault(objectId);
                if (task == null)
                    continue;

                var item = new ProgramTaskItem
                {
                    ObjectIdentifier = objectId,
                    ObjectType = objectType,
                    TaskName = o.Text,
                    TaskIdentifier = task.TaskIdentifier,
                    TaskCompletionRequirement = task.TaskCompletionRequirement,
                    ProgramIdentifier = programId.Value,
                    IsSelected = true
                };

                items.Add(item);
            }

            return (programTasks, items);
        }

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

        private static List<Shift.Common.ListItem> GetTaskListItemBasedOnObjectType(string objectType, Guid organizationId, bool includePartitionItems)
        {
            var list = new List<Shift.Common.ListItem>();

            switch (objectType)
            {
                case "Assessment":
                    GetAssesmentTaskListItems(organizationId, list);
                    return list;
                case "Achievement":
                    GetAchievementTaskListItems(organizationId, list, includePartitionItems);
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

        private static void GetAchievementTaskListItems(Guid organizationId, List<Shift.Common.ListItem> list, bool includePartitionItems)
        {
            var filter = new QAchievementFilter(organizationId);

            if (includePartitionItems && organizationId != ServiceLocator.Partition.Identifier)
                filter.OrganizationIdentifiers.Add(ServiceLocator.Partition.Identifier);

            var items = ServiceLocator.AchievementSearch.GetAchievements(filter);

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

        private static void GetAchievementsObjectData(Guid organization, List<TaskObjectData> results)
        {
            var filter = new QAchievementFilter(organization);

            if (organization != ServiceLocator.Partition.Identifier)
                filter.OrganizationIdentifiers.Add(ServiceLocator.Partition.Identifier);

            var achievementsItems = ServiceLocator.AchievementSearch
                .GetAchievements(filter)
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
                .Select(x => new TaskObjectData
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