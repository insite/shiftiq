using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ProgressCompleted2 : Change
    {
        private class ObsoleteProgressCompleted : Change
        {
            public DateTimeOffset? Completed { get; set; }
            public decimal? Percent { get; set; }
            public bool? Pass { get; set; }
            public int? ElapsedMinutes { get; set; }
        }

        public const string ObsoleteChangeType = "ProgressCompleted";

        public const string Status = "Completed";

        public ProgressCompleted2(DateTimeOffset? completed, decimal? percent, bool? pass, int? elapsedSeconds)
        {
            Completed = completed;
            Percent = percent;
            Pass = pass;
            ElapsedSeconds = elapsedSeconds;
        }

        public DateTimeOffset? Completed { get; set; }
        public decimal? Percent { get; set; }
        public bool? Pass { get; set; }
        public int? ElapsedSeconds { get; set; }

        public static ProgressCompleted2 Upgrade(SerializedChange serializedChange)
        {
            var v1 = serializedChange.Deserialize<ObsoleteProgressCompleted>();

            var v2 = new ProgressCompleted2(v1.Completed, v1.Percent, v1.Pass, v1.ElapsedMinutes * 60)
            {
                AggregateIdentifier = v1.AggregateIdentifier,
                AggregateVersion = v1.AggregateVersion,
                OriginOrganization = v1.OriginOrganization,
                OriginUser = v1.OriginUser,
                ChangeTime = v1.ChangeTime
            };

            return v2;
        }
    }
}
