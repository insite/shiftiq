using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Credentials.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Common.Timeline.Commands;
using Shift.Constant;

namespace InSite.Persistence
{
    /// <summary>
    /// Pushes program task changes down to the credentials of every learner enrolled in the
    /// program. Without this, adding a task to a program can leave enrolled learners on a stale
    /// training plan - until an administrator reassigns the program by hand, and progress recorded
    /// against the missing task can never add up to 100 percent.
    ///
    /// The recompute is idempotent: re-running it always converges on the same state, so it also
    /// serves as a repair tool if the data falls out of sync.
    /// </summary>
    public static class ProgramTaskCascade
    {
        private const string Mandatory = "Mandatory";
        private const string Optional = "Optional";
        private const string Planned = "Planned";
        private const string Unplanned = "Unplanned";

        private const string Month = "Month";
        private const string Year = "Year";

        /// <summary>
        /// The only task object type backed by a credential. Courses, logbooks, surveys, and
        /// assessments have their own "enrollment" records.
        /// </summary>
        public const string Achievement = "Achievement";

        /// <summary>
        /// The settings a learner's credential should have after every program that claims the
        /// achievement has had its say.
        /// </summary>
        private class DesiredState
        {
            public bool IsRequired { get; set; }
            public bool IsPlanned { get; set; }
            public int? LifetimeMonths { get; set; }
        }

        /// <summary>
        /// Recomputes learner credentials for the given achievements. Call after any change to a
        /// program's tasks. Fast no-op when the program has no enrollments.
        /// </summary>
        /// <param name="objectIdentifiers">
        /// The achievements whose task rows were added, removed, or modified. Only achievements are
        /// cascaded; courses, logbooks, and surveys carry their own "enrollment" records rather
        /// than credentials. (For example, survey forms have response/submission records, and these
        /// servie the purpose of an enrollment.)
        /// </param>
        public static void Synchronize(Guid organizationIdentifier, Guid programIdentifier, IEnumerable<Guid> objectIdentifiers)
        {
            var objects = objectIdentifiers?.Distinct().ToArray() ?? new Guid[0];
            if (objects.Length == 0)
                return;

            Guid[] learners;

            using (var db = new InternalDbContext())
            {
                learners = db.TProgramEnrollments.AsNoTracking()
                    .Where(e => e.ProgramIdentifier == programIdentifier)
                    .Select(e => e.LearnerUserIdentifier)
                    .Distinct()
                    .ToArray();
            }

            if (learners.Length == 0)
                return;

            var desired = MergeDemands(TaskSearch.SelectLearnerDemands(organizationIdentifier, programIdentifier, objects));
            var credentials = SelectCredentials(organizationIdentifier, programIdentifier, objects);
            var labels = SelectAchievementLabels(objects);

            var commands = new List<Command>();

            foreach (var learner in learners)
                foreach (var achievement in objects)
                {
                    var key = new Tuple<Guid, Guid>(learner, achievement);

                    desired.TryGetValue(key, out var target);
                    credentials.TryGetValue(key, out var credential);

                    if (credential == null)
                        AddCreateCommands(commands, organizationIdentifier, learner, achievement, target, labels);
                    else if (credential.OrganizationIdentifier == organizationIdentifier)
                        AddUpdateCommands(commands, credential, target);
                }

            foreach (var command in commands)
            {
                try
                {
                    ServiceLocator.SendCommand(command);
                }
                catch (Exception ex) when (ex.Find<DuplicateCredentialException>() != null)
                {
                    // Ignore if the credential already exists.
                }
            }
        }

        /// <summary>
        /// Convenience overload for a program whose whole task set may have shifted.
        /// </summary>
        public static void SynchronizeProgram(Guid organizationIdentifier, Guid programIdentifier)
        {
            var objects = TaskSearch
                .Select(x => x.ProgramIdentifier == programIdentifier && x.ObjectType == Achievement)
                .Select(x => x.ObjectIdentifier)
                .ToArray();

            Synchronize(organizationIdentifier, programIdentifier, objects);
        }

        #region Merge

        /// <summary>
        /// Most-restrictive merge per learner and achievement: Required beats Optional,
        /// Planned beats Unplanned, shortest non-null lifetime wins. A learner enrolled in
        /// two programs that disagree ends up with the stricter of the two, which is why
        /// removing a task from one program cannot quietly relax a requirement another
        /// program still imposes.
        /// </summary>
        private static Dictionary<Tuple<Guid, Guid>, DesiredState> MergeDemands(List<LearnerTaskDemand> demands)
        {
            var result = new Dictionary<Tuple<Guid, Guid>, DesiredState>();

            foreach (var demand in demands)
            {
                var key = new Tuple<Guid, Guid>(demand.LearnerUserIdentifier, demand.ObjectIdentifier);

                if (!result.TryGetValue(key, out var state))
                {
                    result.Add(key, new DesiredState
                    {
                        IsRequired = demand.IsRequired,
                        IsPlanned = demand.IsPlanned,
                        LifetimeMonths = demand.LifetimeMonths
                    });

                    continue;
                }

                state.IsRequired = state.IsRequired || demand.IsRequired;
                state.IsPlanned = state.IsPlanned || demand.IsPlanned;
                state.LifetimeMonths = MinLifetime(state.LifetimeMonths, demand.LifetimeMonths);
            }

            return result;
        }

        private static int? MinLifetime(int? a, int? b)
        {
            if (a == null)
                return b;

            if (b == null)
                return a;

            return Math.Min(a.Value, b.Value);
        }

        #endregion

        #region Commands

        private static void AddCreateCommands(
            List<Command> commands,
            Guid organizationIdentifier,
            Guid learner,
            Guid achievement,
            DesiredState target,
            Dictionary<Guid, string> labels)
        {
            // No program claims the achievement, so there is nothing to hand the learner.
            // This happens on a removal where the learner never held the credential.
            if (target == null)
                return;

            var id = ServiceLocator.AchievementSearch.GetCredentialIdentifier(null, achievement, learner);

            commands.Add(new CreateCredential(id, organizationIdentifier, achievement, learner, DateTimeOffset.Now));

            if (target.LifetimeMonths.HasValue)
                commands.Add(new ChangeCredentialExpiration(id, RelativeExpiration(target.LifetimeMonths.Value)));

            commands.Add(new TagCredential(id, Necessity(target), Priority(target)));

            var authority = AllowSignOff(labels.GetOrDefault(achievement)) ? "Self" : null;

            commands.Add(new ChangeCredentialAuthority(id, null, null, authority, null, null, null));
        }

        private static void AddUpdateCommands(List<Command> commands, QCredential credential, DesiredState target)
        {
            // Orphaned: no program the learner is enrolled in still claims the achievement.
            // Drop it out of the training plan but leave the expiry alone. A credential can
            // be granted outside any program, where unplanned-and-optional-but-expiring is
            // the normal state, so clearing the lifetime here would destroy real data.
            if (target == null)
            {
                if (!StringHelper.Equals(credential.CredentialNecessity, Optional)
                    || !StringHelper.Equals(credential.CredentialPriority, Unplanned))
                    commands.Add(new TagCredential(credential.CredentialIdentifier, Optional, Unplanned));

                return;
            }

            var necessity = Necessity(target);
            var priority = Priority(target);

            if (!StringHelper.Equals(credential.CredentialNecessity, necessity)
                || !StringHelper.Equals(credential.CredentialPriority, priority))
                commands.Add(new TagCredential(credential.CredentialIdentifier, necessity, priority));

            // Only ever set a lifetime, never clear one. A program saying "renew this every
            // twelve months" is a claim; a program with no lifetime is silent on the subject,
            // not an instruction that the credential never expires.
            if (target.LifetimeMonths.HasValue && GetLifetimeMonths(credential) != target.LifetimeMonths)
                commands.Add(new ChangeCredentialExpiration(credential.CredentialIdentifier, RelativeExpiration(target.LifetimeMonths.Value)));
        }

        private static Expiration RelativeExpiration(int months) => new Expiration
        {
            Type = ExpirationType.Relative,
            Lifetime = new Lifetime { Quantity = months, Unit = Month }
        };

        private static string Necessity(DesiredState target) => target.IsRequired ? Mandatory : Optional;

        private static string Priority(DesiredState target) => target.IsPlanned ? Planned : Unplanned;

        /// <summary>
        /// Mirrors EmployeeAchievementHelper.AllowSignOff, which lives in the UI assembly
        /// and cannot be referenced from here.
        /// </summary>
        private static bool AllowSignOff(string achievementType)
        {
            return StringHelper.EqualsAny(achievementType, new[]
            {
                AchievementTypes.AdditionalComplianceRequirement,
                AchievementTypes.CodeOfPractice,
                AchievementTypes.HumanResourcesDocument,
                AchievementTypes.SafeOperatingPractice,
                AchievementTypes.SiteSpecificOperatingProcedure,
                AchievementTypes.TrainingGuide
            });
        }

        #endregion

        #region Queries

        private static Dictionary<Tuple<Guid, Guid>, QCredential> SelectCredentials(Guid organizationIdentifier, Guid programIdentifier, Guid[] objects)
        {
            using (var db = new InternalDbContext())
            {
                var rows = db.QCredentials.AsNoTracking()
                    .Where(c => objects.Contains(c.AchievementIdentifier)
                        && db.TProgramEnrollments.Any(e =>
                            e.ProgramIdentifier == programIdentifier
                            && e.LearnerUserIdentifier == c.UserIdentifier
                            && e.LearnerUser.Persons.Any(p => p.OrganizationIdentifier == organizationIdentifier)))
                    .ToList();

                var result = new Dictionary<Tuple<Guid, Guid>, QCredential>();

                foreach (var row in rows)
                {
                    var key = new Tuple<Guid, Guid>(row.UserIdentifier, row.AchievementIdentifier);

                    if (!result.ContainsKey(key))
                        result.Add(key, row);
                }

                return result;
            }
        }

        private static Dictionary<Guid, string> SelectAchievementLabels(Guid[] objects)
        {
            using (var db = new InternalDbContext())
            {
                return db.QAchievements.AsNoTracking()
                    .Where(a => objects.Contains(a.AchievementIdentifier))
                    .Select(a => new { a.AchievementIdentifier, a.AchievementLabel })
                    .ToDictionary(a => a.AchievementIdentifier, a => a.AchievementLabel);
            }
        }

        private static int? GetLifetimeMonths(QCredential credential)
        {
            if (credential.ExpirationLifetimeQuantity == null)
                return null;

            if (StringHelper.Equals(credential.ExpirationLifetimeUnit, Year))
                return credential.ExpirationLifetimeQuantity * 12;

            if (StringHelper.Equals(credential.ExpirationLifetimeUnit, Month))
                return credential.ExpirationLifetimeQuantity;

            return null;
        }

        #endregion
    }
}
