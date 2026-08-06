using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    /// <summary>
    /// Baseline settings applied to an achievement task when it is added to a program. A program
    /// specifies baseline training requirements, so a new task is planned and required until an
    /// administrator says otherwise, and a time-sensitive achievement carries its own expiration
    /// lifetime to the task.
    /// </summary>
    public static class ProgramTaskDefaults
    {
        public const string AchievementObjectType = "Achievement";

        public const bool IsPlanned = true;
        public const bool IsRequired = true;

        private const string MonthUnit = "Month";
        private const string YearUnit = "Year";

        private const int MonthsPerYear = 12;

        /// <summary>
        /// The achievement-level time-sensitivity default, expressed in months. Returns null when
        /// the achievement never expires or expires on a fixed date, because neither carries a
        /// lifetime the task can inherit.
        /// </summary>
        public static int? GetLifetimeMonths(string expirationType, string expirationLifetimeUnit, int? expirationLifetimeQuantity)
        {
            var isRelative = StringHelper.Equals(expirationType, ExpirationType.Relative.ToString());
            if (!isRelative)
                return null;

            if (!expirationLifetimeQuantity.HasValue || expirationLifetimeQuantity.Value <= 0)
                return null;

            var quantity = expirationLifetimeQuantity.Value;

            var isMonths = StringHelper.Equals(expirationLifetimeUnit, MonthUnit);
            if (isMonths)
                return quantity;

            var isYears = StringHelper.Equals(expirationLifetimeUnit, YearUnit);
            if (isYears)
                return quantity * MonthsPerYear;

            return null;
        }

        /// <summary>
        /// Achievement-level lifetime defaults for a set of achievements, keyed by achievement.
        /// Achievements without a relative expiration are omitted.
        /// </summary>
        public static Dictionary<Guid, int> GetLifetimeMonths(IEnumerable<Guid> achievementIdentifiers)
        {
            var result = new Dictionary<Guid, int>();

            if (achievementIdentifiers == null)
                return result;

            var identifiers = achievementIdentifiers.Distinct().ToArray();
            if (identifiers.Length == 0)
                return result;

            using (var db = new InternalDbContext(false))
            {
                var achievements = db.QAchievements
                    .AsNoTracking()
                    .Where(x => identifiers.Contains(x.AchievementIdentifier))
                    .Select(x => new
                    {
                        x.AchievementIdentifier,
                        x.ExpirationType,
                        x.ExpirationLifetimeUnit,
                        x.ExpirationLifetimeQuantity
                    })
                    .ToList();

                foreach (var achievement in achievements)
                {
                    var months = GetLifetimeMonths(achievement.ExpirationType, achievement.ExpirationLifetimeUnit, achievement.ExpirationLifetimeQuantity);
                    if (months.HasValue)
                        result[achievement.AchievementIdentifier] = months.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// The achievement-level lifetime default for a single achievement.
        /// </summary>
        public static int? GetLifetimeMonths(Guid achievementIdentifier)
        {
            var lifetimes = GetLifetimeMonths(new[] { achievementIdentifier });

            return GetLifetimeMonths(achievementIdentifier, lifetimes);
        }

        /// <summary>
        /// Reads one achievement out of a lifetime lookup produced by
        /// <see cref="GetLifetimeMonths(IEnumerable{Guid})" />.
        /// </summary>
        public static int? GetLifetimeMonths(Guid achievementIdentifier, IDictionary<Guid, int> lifetimes)
        {
            if (lifetimes == null)
                return null;

            var found = lifetimes.TryGetValue(achievementIdentifier, out var months);

            return found ? months : (int?)null;
        }

        /// <summary>
        /// Applies the baseline settings to a task that is about to be inserted, looking up the
        /// achievement lifetime on the way. A task for anything other than an achievement keeps the
        /// settings the caller supplied, and costs no query.
        /// </summary>
        public static void Apply(TTask task)
        {
            if (task == null)
                return;

            var isAchievement = StringHelper.Equals(task.ObjectType, AchievementObjectType);
            if (!isAchievement)
                return;

            var lifetimes = GetLifetimeMonths(new[] { task.ObjectIdentifier });

            Apply(task, lifetimes);
        }

        /// <summary>
        /// Applies the baseline settings to a task that is about to be inserted, reading the
        /// achievement lifetime from a lookup shared by every task in the same batch. A task for
        /// anything other than an achievement keeps the settings the caller supplied.
        /// </summary>
        public static void Apply(TTask task, IDictionary<Guid, int> lifetimes)
        {
            if (task == null)
                return;

            var isAchievement = StringHelper.Equals(task.ObjectType, AchievementObjectType);
            if (!isAchievement)
                return;

            task.TaskIsPlanned = IsPlanned;
            task.TaskIsRequired = IsRequired;
            task.TaskLifetimeMonths = GetLifetimeMonths(task.ObjectIdentifier, lifetimes);
        }
    }
}
