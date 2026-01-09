using System;
using System.Data.Entity;
using System.Linq;

using InSite.Domain.Integrations.Scorm;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Persistence.Integration.Moodle
{
    public class MoodleStore
    {
        public void InsertMoodleEvent(Guid activityId, MoodleEvent e, DateTimeOffset when)
        {
            using (var db = CreateContext())
            {
                var entity = new TMoodleEvent
                {
                    Action = e.Action,
                    Anonymous = Convert.ToBoolean(e.Anonymous),
                    CallbackUrl = e.CallbackUrl,
                    Component = e.Component,
                    ContextId = e.ContextId,
                    ContextInstanceId = e.ContextInstanceId,
                    ContextLevel = e.ContextLevel,
                    CourseId = e.CourseId,
                    Crud = e.Crud,
                    EduLevel = e.EduLevel,
                    EventName = e.EventName,
                    IdNumber = e.IdNumber,
                    ObjectId = e.ObjectId,
                    ObjectTable = e.ObjectTable,
                    RelatedUserId = e.RelatedUserId,
                    ShortName = e.ShortName,
                    Target = e.Target,
                    TimeCreated = e.TimeCreated,
                    Token = e.Token,
                    UserId = e.UserId
                };

                entity.OtherInstanceId = e.Other?.InstanceId;
                entity.OtherLoadedContent = e.Other?.LoadedContent;

                entity.OtherAttemptId = e.Other?.AttemptId;
                entity.OtherCmiElement = e.Other?.CmiElement;
                entity.OtherCmiValue = e.Other?.CmiValue;

                entity.OtherFinalGrade = e.Other?.FinalGrade;
                entity.OtherItemId = e.Other?.ItemId;
                entity.OtherOverridden = e.Other?.Overridden;

                entity.EventIdentifier = UniqueIdentifier.Create();
                entity.EventData = JsonConvert.SerializeObject(e);
                entity.EventWhen = when;

                GetIdentifiers(activityId, e.UserGuid, entity, db);

                db.TMoodleEvents.Add(entity);
                db.SaveChanges();
            }
        }

        private void GetIdentifiers(Guid activityId, string userId, TMoodleEvent entity, InternalDbContext db)
        {
            var activity = db.QActivities
                .Include(x => x.Module.Unit.Course)
                .FirstOrDefault(x => x.ActivityIdentifier == activityId);

            entity.ActivityIdentifier = activityId;
            entity.CourseIdentifier = activity?.Module?.Unit?.CourseIdentifier ?? Guid.Empty;
            entity.OrganizationIdentifier = activity?.Module?.Unit?.Course?.OrganizationIdentifier ?? Guid.Empty;

            if (Guid.TryParse(userId, out var user))
                entity.UserGuid = user;
        }

        private InternalDbContext CreateContext()
            => new InternalDbContext(true);
    }
}