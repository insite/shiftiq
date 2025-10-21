using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Domain.Records;

namespace InSite.Persistence
{
    public class ProgramSearch2 : IProgramSearch
    {
        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext(false);
        }

        public List<Guid> GetProgramIds(Guid taskObjectId)
        {
            using (var db = CreateContext())
            {
                return db.TTasks.AsNoTracking().AsQueryable().Where(x => x.ObjectIdentifier == taskObjectId).Select(y => y.ProgramIdentifier).ToList();
            }
        }

        public List<Guid> GetProgramIdsForStandaloneAchievements(Guid taskObjectId, out List<Guid?> objects)
        {
            using (var db = CreateContext())
            {
                var taskIds = db.QJournalSetups.AsNoTracking().AsQueryable().Where(x => x.JournalSetupIdentifier == taskObjectId).Select(y => y.AchievementIdentifier).ToList();

                objects = taskIds;

                return db.TTasks.AsNoTracking().AsQueryable().Where(x => taskIds.Contains(x.ObjectIdentifier)).Select(y => y.ProgramIdentifier).ToList();
            }
        }

        public List<SubmittedProgram> GetProgramsForSubmit(Guid organizationId, List<Guid> programsIds)
        {
            using (var db = CreateContext())
            {
                return db.TPrograms
                    .Where(x =>
                        x.OrganizationIdentifier == organizationId
                        && programsIds.Contains(x.ProgramIdentifier)
                    )
                    .GroupJoin(db.TProgramCategories,
                        program => program.ProgramIdentifier,
                        category => category.ProgramIdentifier,
                        (program, categories) => new SubmittedProgram
                        {
                            ProgramId = program.ProgramIdentifier,
                            ProgramName = program.ProgramName,
                            CategoryNames = categories.Select(x => x.Category.ItemName).ToList(),
                        }
                    )
                    .OrderBy(x => x.ProgramName)
                    .ToList();
            }
        }

        public ProgramValuesResult GetProgramValues(Guid programId, Guid taskObjectId)
        {
            using (var db = CreateContext())
            {
                var tasks = db.TTasks.AsNoTracking().AsQueryable()
                    .Where(x => x.ObjectIdentifier == taskObjectId && x.ProgramIdentifier == programId)
                    .Include(x => x.Program)
                    .ToArray();

                if (tasks == null || tasks.Length != 1)
                    return null;

                return new ProgramValuesResult
                {
                    ProgramIdentifier = tasks[0].ProgramIdentifier,
                    ProgramName = tasks[0].Program.ProgramName,
                    TaskIdentifier = tasks[0].TaskIdentifier,
                    CompletionTaskIdentifier = tasks[0].Program.CompletionTaskIdentifier,
                    NotificationCompletedAdministratorMessageIdentifier = tasks[0].Program.NotificationCompletedAdministratorMessageIdentifier,
                    NotificationCompletedLearnerMessageIdentifier = tasks[0].Program.NotificationCompletedLearnerMessageIdentifier,
                    AchievementIdentifier = tasks[0].Program.AchievementIdentifier,
                };
            }
        }

        public bool IsTaskCompletionPrerequisite(Guid programId, Guid taskObjectId)
        {
            using (var db = CreateContext())
            {
                var tasks = db.TTasks.AsNoTracking().AsQueryable().Where(x => x.ObjectIdentifier == taskObjectId).Include(x => x.Program).ToArray();

                if (tasks != null && tasks.Length == 1)
                    return (tasks[0].TaskIdentifier == tasks[0].Program.CompletionTaskIdentifier);

                return false;
            }
        }

        public bool IsTaskCompletedByLearner(Guid taskIdentifier, Guid userIdentifier)
        {
            using (var db = CreateContext())
            {
                var enrollment = db.TTaskEnrollments.AsNoTracking().AsQueryable().FirstOrDefault(x => x.TaskIdentifier == taskIdentifier && x.LearnerUserIdentifier == userIdentifier);

                if (enrollment != null)
                    return enrollment.ProgressCompleted.HasValue;

                return false;
            }
        }

        public bool IsProgramFullyCompletedByLearner(Guid programId, Guid userId)
        {
            const string query = "exec records.GetProgramTaskCompletionCountForUser @ProgramIdentifier, @UserIdentifier";

            using (var db = new InternalDbContext(false))
            {
                var completionData = db.Database
                    .SqlQuery<ProgramEnrollmentTaskCompletionCounterForUsers>(query, new SqlParameter("ProgramIdentifier", programId), new SqlParameter("UserIdentifier", userId))
                    .FirstOrDefault();

                return (completionData != null && completionData.TaskCount == completionData.CompletionCounter);
            }
        }
    }
}
