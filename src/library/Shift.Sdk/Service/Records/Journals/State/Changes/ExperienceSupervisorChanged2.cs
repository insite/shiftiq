using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceSupervisorChanged2 : Change
    {
        public Guid Experience { get; set; }
        public Guid? Supervisor { get; set; }

        public ExperienceSupervisorChanged2(Guid experience, Guid? supervisor)
        {
            Experience = experience;
            Supervisor = supervisor;
        }

        #region Obsolete

        private class ExperienceSupervisorChanged1 : Change
        {
            public Guid Experience { get; set; }
            public string Supervisor { get; set; }
        }

        public static ExperienceSupervisorChanged2 Upgrade(SerializedChange serializedChange)
        {
            var v1 = serializedChange.Deserialize<ExperienceSupervisorChanged1>();

            var v2 = new ExperienceSupervisorChanged2(v1.Experience, null)
            {
                AggregateState = serializedChange.AggregateState,
                AggregateIdentifier = v1.AggregateIdentifier,
                AggregateVersion = v1.AggregateVersion,
                OriginOrganization = v1.OriginOrganization,
                OriginUser = v1.OriginUser,
                ChangeTime = v1.ChangeTime
            };

            return v2;
        }

        #endregion
    }
}
