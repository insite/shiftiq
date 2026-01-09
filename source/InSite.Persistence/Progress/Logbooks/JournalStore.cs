using System;
using System.Data.Entity;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class JournalStore : IJournalStore
    {
        private IContactSearch _contactSearch;

        public JournalStore(IContactSearch contactSearch)
        {
            _contactSearch = contactSearch;
        }

        public void InsertJournalSetup(JournalSetupCreated e)
        {
            using (var db = CreateContext())
            {
                var entity = new QJournalSetup
                {
                    JournalSetupIdentifier = e.AggregateIdentifier,
                    JournalSetupName = e.Name,
                    OrganizationIdentifier = e.Tenant,
                    JournalSetupCreated = e.ChangeTime
                };

                SetLastChange(entity, e);

                db.QJournalSetups.Add(entity);
                db.SaveChanges();
            }
        }

        public void UpdateJournalSetup(IChange e, Action<QJournalSetup> change)
        {
            using (var db = CreateContext())
            {
                UpdateJournalSetup(e, change, db);

                db.SaveChanges();
            }
        }

        public void DeleteJournalSetup(JournalSetupDeleted e)
        {
            using (var db = CreateContext())
            {
                var setup = db.QJournalSetups
                    .Include(x => x.CompetencyRequirements)
                    .Include(x => x.Fields)
                    .Include(x => x.Users)
                    .Include(x => x.AreaRequirements)
                    .Where(x => x.JournalSetupIdentifier == e.AggregateIdentifier)
                    .FirstOrDefault();

                if (setup == null)
                    return;

                db.QAreaRequirements.RemoveRange(setup.AreaRequirements);
                db.QCompetencyRequirements.RemoveRange(setup.CompetencyRequirements);
                db.QJournalSetupFields.RemoveRange(setup.Fields);
                db.QJournalSetupUsers.RemoveRange(setup.Users);
                db.QJournalSetups.Remove(setup);

                var contentContainers = setup.Fields
                    .Select(x => x.JournalSetupFieldIdentifier)
                    .ToList();

                contentContainers.Add(e.AggregateIdentifier);

                var contents = db.TContents
                    .Where(x => contentContainers.Contains(x.ContainerIdentifier))
                    .ToList();

                db.TContents.RemoveRange(contents);

                db.SaveChanges();
            }
        }

        public void InsertCompetencyRequirement(CompetencyRequirementAdded e)
        {
            var state = (JournalSetupState)e.AggregateState;
            var model = state.CompetencyRequirements.Find(x => x.Competency == e.Competency);

            using (var db = CreateContext())
            {
                var entity = new QCompetencyRequirement
                {
                    JournalSetupIdentifier = e.AggregateIdentifier,
                    CompetencyStandardIdentifier = model.Competency,
                    CompetencyHours = model.Hours,
                    JournalItems = model.JournalItems,
                    SkillRating = model.SkillRating,
                    IncludeHoursToArea = model.IncludeHoursToArea
                };

                db.QCompetencyRequirements.Add(entity);

                UpdateJournalSetup(e, null, db);

                db.SaveChanges();
            }
        }

        public void UpdateCompetencyRequirement(CompetencyRequirementChanged e)
        {
            var state = (JournalSetupState)e.AggregateState;
            var model = state.CompetencyRequirements.Find(x => x.Competency == e.Competency);

            using (var db = CreateContext())
            {
                var entity = db.QCompetencyRequirements
                    .Where(x => x.JournalSetupIdentifier == e.AggregateIdentifier && x.CompetencyStandardIdentifier == e.Competency)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                entity.CompetencyHours = model.Hours;
                entity.JournalItems = model.JournalItems;
                entity.SkillRating = model.SkillRating;
                entity.IncludeHoursToArea = model.IncludeHoursToArea;

                db.Entry(entity).State = EntityState.Modified;

                UpdateJournalSetup(e, null, db);

                db.SaveChanges();
            }
        }

        public void DeleteCompetencyRequirement(CompetencyRequirementDeleted e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QCompetencyRequirements
                    .Where(x => x.JournalSetupIdentifier == e.AggregateIdentifier && x.CompetencyStandardIdentifier == e.Competency)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                db.QCompetencyRequirements.Remove(entity);

                UpdateJournalSetup(e, null, db);

                db.SaveChanges();
            }
        }

        public void UpdateAreaRequirement(JournalSetupAreaHoursModified e)
        {
            var state = (JournalSetupState)e.AggregateState;
            var model = state.FindAreaRequirement(e.Area);

            using (var db = CreateContext())
            {
                var entity = db.QAreaRequirements
                    .Where(x => x.JournalSetupIdentifier == e.AggregateIdentifier && x.AreaStandardIdentifier == model.Area)
                    .FirstOrDefault();

                if (entity == null)
                    db.QAreaRequirements.Add(entity = new QAreaRequirement
                    {
                        JournalSetupIdentifier = e.AggregateIdentifier,
                        AreaStandardIdentifier = model.Area
                    });

                entity.AreaHours = model.Hours;

                UpdateJournalSetup(e, null, db);

                db.SaveChanges();
            }
        }

        public void InsertField(JournalSetupFieldAdded e)
        {
            using (var db = CreateContext())
            {
                var entity = new QJournalSetupField
                {
                    JournalSetupIdentifier = e.AggregateIdentifier,
                    JournalSetupFieldIdentifier = e.Field,
                    FieldType = e.Type.ToString(),
                    Sequence = e.Sequence,
                    FieldIsRequired = e.IsRequired
                };

                db.QJournalSetupFields.Add(entity);

                UpdateJournalSetup(e, null, db);

                db.SaveChanges();
            }
        }

        public void UpdateField(IChange e, Guid journalSetupFieldIdentifier, Action<QJournalSetupField> action)
        {
            using (var db = CreateContext())
            {
                if (action != null)
                {
                    var entity = db.QJournalSetupFields
                        .Where(x => x.JournalSetupFieldIdentifier == journalSetupFieldIdentifier)
                        .FirstOrDefault();

                    if (entity == null)
                        return;

                    action(entity);

                    db.Entry(entity).State = EntityState.Modified;
                }

                UpdateJournalSetup(e, null, db);

                db.SaveChanges();
            }
        }

        public void DeleteField(JournalSetupFieldDeleted e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QJournalSetupFields
                    .Where(x => x.JournalSetupFieldIdentifier == e.Field)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                db.QJournalSetupFields.Remove(entity);

                UpdateJournalSetup(e, null, db);

                db.SaveChanges();
            }
        }

        public void ReorderFields(JournalSetupFieldsReordered e)
        {
            using (var db = CreateContext())
            {
                var fields = db.QJournalSetupFields.Where(x => x.JournalSetupIdentifier == e.AggregateIdentifier).ToList();
                foreach (var (fieldId, sequence) in e.Fields)
                {
                    var field = fields.Find(x => x.JournalSetupFieldIdentifier == fieldId);
                    if (field != null)
                        field.Sequence = sequence;
                }
                db.SaveChanges();
            }
        }

        public void InsertUser(JournalSetupUserAdded e)
        {
            using (var db = CreateContext())
            {
                var entity = new QJournalSetupUser
                {
                    EnrollmentIdentifier = UniqueIdentifier.Create(),
                    JournalSetupIdentifier = e.AggregateIdentifier,
                    UserIdentifier = e.User,
                    UserRole = e.Role.ToString()
                };

                db.QJournalSetupUsers.Add(entity);

                UpdateJournalSetup(e, null, db);

                db.SaveChanges();
            }
        }

        public void DeleteUser(JournalSetupUserDeleted e)
        {
            var roleText = e.Role.ToString();

            using (var db = CreateContext())
            {
                var entity = db.QJournalSetupUsers
                    .Where(x => x.JournalSetupIdentifier == e.AggregateIdentifier && x.UserIdentifier == e.User && x.UserRole == roleText)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                db.QJournalSetupUsers.Remove(entity);

                UpdateJournalSetup(e, null, db);

                db.SaveChanges();
            }
        }

        public void InsertGroup(JournalSetupGroupCreated e)
        {
            using (var db = CreateContext())
            {
                var entity = new QJournalSetupGroup
                {
                    JournalSetupIdentifier = e.AggregateIdentifier,
                    GroupIdentifier = e.Group,
                    Created = DateTimeOffset.UtcNow
                };

                db.QJournalSetupGroups.Add(entity);

                UpdateJournalSetup(e, null, db);

                db.SaveChanges();
            }
        }

        public void DeleteGroup(JournalSetupGroupRemoved e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QJournalSetupGroups
                    .Where(x => x.JournalSetupIdentifier == e.AggregateIdentifier && x.GroupIdentifier == e.Group)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                db.QJournalSetupGroups.Remove(entity);

                UpdateJournalSetup(e, null, db);

                db.SaveChanges();
            }
        }

        public void InsertJournal(JournalCreated e)
        {
            using (var db = CreateContext())
            {
                var entity = new QJournal
                {
                    JournalIdentifier = e.AggregateIdentifier,
                    JournalSetupIdentifier = e.JournalSetup,
                    UserIdentifier = e.User,
                    JournalCreated = e.ChangeTime
                };

                db.QJournals.Add(entity);
                db.SaveChanges();
            }
        }

        public void DeleteJournal(JournalDeleted e)
        {
            using (var db = CreateContext())
            {
                var journal = db.QJournals
                    .Include(x => x.Experiences.Select(y => y.Competencies))
                    .Where(x => x.JournalIdentifier == e.AggregateIdentifier)
                    .FirstOrDefault();

                if (journal == null)
                    return;

                var competencies = journal.Experiences.SelectMany(x => x.Competencies).ToList();

                db.QExperienceCompetencies.RemoveRange(competencies);
                db.QExperiences.RemoveRange(journal.Experiences);
                db.QJournals.Remove(journal);

                db.SaveChanges();
            }
        }

        public void InsertComment(CommentAdded e)
        {
            using (var db = CreateContext())
            {
                var entity = new QComment
                {
                    CommentIdentifier = e.Comment,
                    LogbookIdentifier = e.AggregateIdentifier,
                    AuthorUserIdentifier = e.Author,
                    ContainerIdentifier = e.Subject,
                    CommentText = e.Text,
                    CommentPosted = e.Posted,
                    CommentIsPrivate = e.IsPrivate
                };

                entity.ContainerType = "Logbook";

                if (e.SubjectType == "Journal")
                {
                    entity.LogbookIdentifier = e.Subject;
                    entity.ContainerSubtype = "Journal";
                }
                else if (e.SubjectType == "Experience")
                {
                    entity.LogbookExperienceIdentifier = e.Subject;
                    entity.ContainerSubtype = "Experience";
                }

                db.QComments.Add(entity);
                db.SaveChanges();
            }
        }

        public void UpdateComment(CommentChanged e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QComments
                    .Where(x => x.CommentIdentifier == e.Comment)
                    .FirstOrDefault();

                if (entity != null)
                {
                    entity.CommentText = e.Text;
                    entity.CommentRevised = e.Revised;
                    entity.CommentIsPrivate = e.IsPrivate;

                    db.SaveChanges();
                }
            }
        }

        public void DeleteComment(CommentDeleted e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QComments
                    .Where(x => x.CommentIdentifier == e.Comment)
                    .FirstOrDefault();

                if (entity != null)
                {
                    db.QComments.Remove(entity);
                    db.SaveChanges();
                }
            }
        }

        public void InsertExperience(ExperienceAdded e)
        {
            using (var db = CreateContext())
            {
                var sequence = db.QExperiences
                    .Where(x => x.JournalIdentifier == e.AggregateIdentifier)
                    .Max(x => (int?)x.Sequence);

                var entity = new QExperience
                {
                    ExperienceIdentifier = e.Experience,
                    JournalIdentifier = e.AggregateIdentifier,
                    ExperienceCreated = e.ChangeTime,
                    Sequence = sequence.HasValue ? sequence.Value + 1 : 1
                };

                db.QExperiences.Add(entity);
                db.SaveChanges();
            }
        }

        public void UpdateExperience(Guid experienceIdentifier, Action<QExperience> action)
        {
            using (var db = CreateContext())
            {
                var entity = db.QExperiences
                    .Where(x => x.ExperienceIdentifier == experienceIdentifier)
                    .FirstOrDefault();

                if (entity != null)
                {
                    action(entity);

                    db.SaveChanges();
                }
            }
        }

        public void DeleteExperience(ExperienceDeleted e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QExperiences
                    .Include(x => x.Competencies)
                    .Where(x => x.ExperienceIdentifier == e.Experience)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                var comments = db.QComments
                    .Where(x => x.LogbookExperienceIdentifier == e.Experience)
                    .ToList();

                db.QComments.RemoveRange(comments);
                db.QExperienceCompetencies.RemoveRange(entity.Competencies);
                db.QExperiences.Remove(entity);

                db.SaveChanges();
            }
        }

        public void InsertExperienceCompetency(ExperienceCompetencyAdded e)
        {
            using (var db = CreateContext())
            {
                var entity = new QExperienceCompetency
                {
                    ExperienceIdentifier = e.Experience,
                    CompetencyStandardIdentifier = e.Competency,
                    CompetencyHours = e.Hours
                };

                db.QExperienceCompetencies.Add(entity);

                db.SaveChanges();
            }
        }

        public void UpdateExperienceCompetency(Guid experienceIdentifier, Guid competencyIdentifier, Action<QExperienceCompetency> action)
        {
            using (var db = CreateContext())
            {
                var entity = db.QExperienceCompetencies
                    .Where(x => x.ExperienceIdentifier == experienceIdentifier && x.CompetencyStandardIdentifier == competencyIdentifier)
                    .FirstOrDefault();

                if (entity != null)
                {
                    action(entity);

                    db.SaveChanges();
                }
            }
        }

        public void DeleteExperienceCompetency(ExperienceCompetencyDeleted e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QExperienceCompetencies
                    .Where(x => x.ExperienceIdentifier == e.Experience && x.CompetencyStandardIdentifier == e.Competency)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                db.QExperienceCompetencies.Remove(entity);

                db.SaveChanges();
            }
        }

        private void UpdateJournalSetup(IChange e, Action<QJournalSetup> change, InternalDbContext db)
        {
            var entity = db.QJournalSetups
                .FirstOrDefault(x => x.JournalSetupIdentifier == e.AggregateIdentifier);

            if (entity == null)
                return;

            change?.Invoke(entity);

            SetLastChange(entity, e);
        }

        private void SetLastChange(QJournalSetup entity, IChange e)
        {
            var user = _contactSearch.GetUser(e.OriginUser);

            entity.LastChangeTime = e.ChangeTime;
            entity.LastChangeType = e.GetType().Name;
            entity.LastChangeUser = user != null ? user.UserFullName : UserNames.Someone;
        }

        internal InternalDbContext CreateContext() => new InternalDbContext(true) { EnablePrepareToSaveChanges = false };
    }
}
