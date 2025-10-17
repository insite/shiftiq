using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace Shift.Common
{
    public class Lockout : Model
    {
        /// <summary>
        /// If it is present iin appsettings and set o true - Lockout will be disabled
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// The partitions to which this lockout applies.
        /// </summary>
        public string[] Partitions { get; set; }

        /// <summary>
        /// The environments to which this lockout applies.
        /// </summary>
        public string[] Environments { get; set; }

        /// <summary>
        /// The interfaces to which this lockout applies.
        /// </summary>
        public string[] Interfaces { get; set; }

        /// <summary>
        /// The interval of time during which the lockout is in effect.
        /// </summary>
        public Interval Interval { get; set; }

        public Lockout()
        {
            Interval = new Interval();
            Partitions = Environments = Interfaces = new string[0];
        }

        public bool FilterPartitions()
            => Partitions != null && Partitions.Any();

        public bool FilterEnvironments()
            => Environments != null && Environments.Any();

        public bool FilterInterfaces()
            => Interfaces != null && Interfaces.Any();

        public int? MinutesBeforeOpenTime(DateTimeOffset current, string partition, string environment)
        {
            var next = NextOpenTime(current, partition, environment);

            if (next == null)
                return null;

            return Interval.MinutesBeforeOpenTime(current);
        }

        public DateTimeOffset? NextOpenTime(DateTimeOffset current, string partition, string environment)
        {
            var next = Interval.NextOpenTime(current);

            if (next < current)
                return null;

            if (FilterPartitions() && partition.MatchesNone(Partitions))
                return null;

            if (FilterEnvironments() && environment.MatchesNone(Environments))
                return null;

            return next;
        }

        public bool IsActive(DateTimeOffset current, string partition, string environment)
        {
            if (Disabled)
                return false;

            if (!Interval.Contains(current))
                return false;

            if (FilterPartitions() && partition.MatchesNone(Partitions))
                return false;

            if (FilterEnvironments() && environment.MatchesNone(Environments))
                return false;

            return true;
        }

        public bool IsValid()
            => !Validate().Any();

        public IEnumerable<ValidationError> Validate()
        {
            var errors = Interval.Validate().ToList();

            foreach (var i in Environments)
                if (i.MatchesNone(Shift.Common.Environments.Names))
                    errors.Add(new ValidationError { Property = nameof(Environments), Summary = $"Environments can contain only items in this list: {string.Join("; ", Shift.Common.Environments.Names)}" });

            foreach (var i in Interfaces)
                if (i.MatchesNone(new[] { "api", "ui" }))
                    errors.Add(new ValidationError { Property = nameof(Environments), Summary = "Interfaces can contain only items in this list: API; UI" });

            return errors;
        }
    }
}