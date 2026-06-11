using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using InSite.Application.Contents.Read;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class ProgramSearch2 : IProgramSearch
    {
        private readonly IContentSearch _contentSearch;

        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext(false);
        }

        public ProgramSearch2(IContentSearch contentSearch)
        {
            _contentSearch = contentSearch;
        }

        public Guid? GetGroupEnrollmentProgramId(Guid userId, Guid objectId)
        {
            using (var db = CreateContext())
            {
                return db.TTasks
                    .Where(x =>
                        x.ObjectIdentifier == objectId
                        && db.TProgramGroupEnrollments
                            .Where(y =>
                                y.ProgramIdentifier == x.ProgramIdentifier
                                && db.QMemberships.Where(z =>
                                    z.GroupIdentifier == y.GroupIdentifier
                                    && z.UserIdentifier == userId
                                ).Any()
                            )
                            .Any()
                    )
                    .Select(x => (Guid?)x.ProgramIdentifier)
                    .FirstOrDefault();
            }
        }

        public bool IsTaskEnrollmentExist(Guid userId, Guid objectId)
        {
            using (var db = CreateContext())
            {
                return db.TTaskEnrollments.Where(x =>
                    x.LearnerUserIdentifier == userId
                    && x.ObjectIdentifier == objectId
                ).Any();
            }

        }

        public int CountProgramGroups(Guid programId, string keyword)
        {
            using (var db = CreateContext())
            {
                var query = db.TProgramGroupEnrollments.Where(x => x.ProgramIdentifier == programId);
                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(x => x.Group.GroupName.Contains(keyword));

                return query.Count();
            }
        }

        public List<ProgramGroup> GetProgramGroups(Guid programId, string keyword, Paging paging)
        {
            using (var db = CreateContext())
            {
                var query = db.TProgramGroupEnrollments.Where(x => x.ProgramIdentifier == programId);
                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(x => x.Group.GroupName.Contains(keyword));

                return query
                    .Select(x => new ProgramGroup
                    {
                        GroupIdentifier = x.GroupIdentifier,
                        GroupName = x.Group.GroupName,
                        GroupSize = x.Group.QMemberships.Count(),
                        Added = x.Created
                    })
                    .ApplyPaging(paging)
                    .ToList();
            }
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

        public List<SubmittedProgram> GetProgramsForSubmit(Guid organizationId, List<Guid> programsIds, string language)
        {
            List<SubmittedProgram> programs;

            using (var db = CreateContext())
            {
                programs = db.TPrograms
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

            var programSummaries = _contentSearch.GetBlocks(programsIds, new[] { language, ContentContainer.DefaultLanguage }, new[] { ContentLabel.Summary });

            foreach (var program in programs)
            {
                if (programSummaries.TryGetValue(program.ProgramId, out var content))
                {
                    program.ProgramSummary = content.Summary.GetText(language)
                        ?? content.Summary.GetText(ContentContainer.DefaultLanguage);
                }
            }

            return programs;
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
