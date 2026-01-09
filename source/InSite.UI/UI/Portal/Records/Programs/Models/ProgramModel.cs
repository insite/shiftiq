using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Portal.Records.Programs
{
    [Serializable]
    public class ProgramModel
    {
        public Guid User { get; private set; }
        public string Slug { get; private set; }
        public TProgram Program { get; private set; }
        public ContentContainer ProgramContenet { get; private set; }
        public List<TTaskEnrollment> UserTaskEnrollments { get; private set; }
        public List<TPrerequisite> TaskPrerequisites { get; private set; }
        public Dictionary<Guid, TTask> Tasks { get; private set; }
        public List<TaskModel> UserTasks { get; private set; } = new List<TaskModel>();
        public Dictionary<Guid, ContentContainer> TaskContenet { get; private set; }


        public ProgramModel(TProgram program, Guid user, string slug)
        {
            Program = program;
            User = user;
            Slug = slug;
            Tasks = new Dictionary<Guid, TTask>();
            TaskContenet = new Dictionary<Guid, Shift.Common.ContentContainer>();

            if (program == null)
                return;

            ProgramContenet = ServiceLocator.ContentSearch.GetBlock(program.ProgramIdentifier);
            TaskPrerequisites = CourseSearch.SelectProgramPrerequisites(program.Tasks.Select(x => x.TaskIdentifier).ToArray()).ToList();
            UserTaskEnrollments = TaskSearch.GetUserTaskEnrollments(program.OrganizationIdentifier, program.ProgramIdentifier, User).ToList();

            if (program.Tasks == null)
                return;

            foreach (var task in program.Tasks.Where(x => x.ObjectType != "Assessment"))
            {
                var userTaskEnrollment = UserTaskEnrollments.FirstOrDefault(x => x.TaskIdentifier == task.TaskIdentifier);
                if (userTaskEnrollment == null)
                    continue;

                var isLocked = false;

                if (!userTaskEnrollment.ProgressCompleted.HasValue)
                {
                    if (TaskPrerequisites.Exists(x => x.ObjectIdentifier == task.TaskIdentifier))
                    {
                        var prerequisitesToCheck = TaskPrerequisites.Where(x => x.ObjectIdentifier == task.TaskIdentifier).ToList();
                        isLocked = IsLocked(prerequisitesToCheck, UserTaskEnrollments.Where(x => prerequisitesToCheck.Any(y => y.TriggerIdentifier == x.TaskIdentifier)).ToList());
                    }

                    if (program.AchievementIdentifier.HasValue
                        && !userTaskEnrollment.ProgressCompleted.HasValue
                        && task.ObjectIdentifier == program.AchievementIdentifier)
                        isLocked = true;
                }

                Tasks.Add(task.TaskIdentifier, task);
                UserTasks.Add(new TaskModel
                {
                    IsCompleted = userTaskEnrollment.ProgressCompleted.HasValue,
                    IsLocked = isLocked,
                    ObjectIdentifier = task.ObjectIdentifier,
                    ObjectType = task.ObjectType,
                    OrganizationIdentifier = task.OrganizationIdentifier,
                    ProgramIdentifier = task.ProgramIdentifier,
                    TaskCompletionRequirement = task.TaskCompletionRequirement,
                    TaskIdentifier = task.TaskIdentifier,
                    TaskImage = task.TaskImage,
                    TaskIsPlanned = task.TaskIsPlanned,
                    TaskIsRequired = task.TaskIsRequired,
                    TaskLifetimeMonths = task.TaskLifetimeMonths,
                    TaskSequence = task.TaskSequence
                });
                TaskContenet.Add(task.TaskIdentifier, ServiceLocator.ContentSearch.GetBlock(task.TaskIdentifier));
            }
        }

        private bool IsLocked(List<TPrerequisite> prerequisitesToCheck, List<TTaskEnrollment> userEnrollments)
        {
            foreach (var prerequisite in prerequisitesToCheck)
            {
                if (prerequisite.TriggerChange == "TaskCompleted")
                {
                    var condition = userEnrollments.FirstOrDefault(x => x.TaskIdentifier == prerequisite.TriggerIdentifier);
                    if (condition != null && !condition.ProgressCompleted.HasValue)
                        return true;
                }
                else if (prerequisite.TriggerChange == "TaskViewed")
                {
                    var condition = userEnrollments.FirstOrDefault(x => x.TaskIdentifier == prerequisite.TriggerIdentifier);
                    if (condition != null && !condition.ProgressStarted.HasValue)
                        return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public class TaskModel
    {
        public Guid ObjectIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid TaskIdentifier { get; set; }

        public string ObjectType { get; set; }
        public string TaskCompletionRequirement { get; set; }
        public string TaskImage { get; set; }

        public bool TaskIsRequired { get; set; }
        public bool TaskIsPlanned { get; set; }

        public int? TaskLifetimeMonths { get; set; }
        public int TaskSequence { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsLocked { get; set; }

    }
}