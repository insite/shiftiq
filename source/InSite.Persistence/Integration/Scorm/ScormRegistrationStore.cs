using System;
using System.Data.Entity;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class ScormRegistrationStore
    {
        public static void Insert(TScormRegistration registration)
        {
            using (var db = new InternalDbContext())
            {
                SnipStrings(registration);
                db.TScormRegistrations.Add(registration);
                db.SaveChanges();
            }
        }

        public static void Insert(TScormRegistrationActivity activity)
        {
            using (var db = new InternalDbContext())
            {
                var courseActivity = db.QActivities
                    .Include(x => x.Module.Unit.Course)
                    .FirstOrDefault(x => x.ActivityIdentifier == activity.ActivityIdentifier);

                var courseId = courseActivity?.Module?.Unit?.Course?.CourseIdentifier;

                if (courseId == null)
                    return;

                activity.CourseIdentifier = courseId.Value;

                db.TScormRegistrationActivities.Add(activity);
                db.SaveChanges();
            }
        }

        public static void Update(TScormRegistration registration)
        {
            using (var db = new InternalDbContext())
            {
                SnipStrings(registration);
                db.Entry(registration).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        private static void SnipStrings(TScormRegistration registration)
        {
            var max = InternalDbContext.GetMaxLength<TScormRegistration>(x => x.ScormPackageHook);
            if (max.HasValue)
                registration.ScormPackageHook = registration.ScormPackageHook.MaxLength(max.Value, true);

            max = InternalDbContext.GetMaxLength<TScormRegistration>(x => x.ScormRegistrationSuccess);
            if (max.HasValue)
                registration.ScormRegistrationSuccess = registration.ScormRegistrationSuccess.MaxLength(max.Value, true);

            max = InternalDbContext.GetMaxLength<TScormRegistration>(x => x.ScormRegistrationCompletion);
            if (max.HasValue)
                registration.ScormRegistrationCompletion = registration.ScormRegistrationCompletion.MaxLength(max.Value, true);
        }

        public static void Delete(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var registration = db.TScormRegistrations.FirstOrDefault(x => x.ScormRegistrationIdentifier == id);
                if (registration == null)
                    return;
                db.TScormRegistrations.Remove(registration);
                db.SaveChanges();
            }
        }
    }
}