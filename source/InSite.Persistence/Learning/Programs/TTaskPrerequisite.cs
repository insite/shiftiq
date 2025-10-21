using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TTaskPrerequisite
    {
        public Guid ObjectIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid PrerequisiteIdentifier { get; set; }
        public Guid TriggerIdentifier { get; set; }

        public string ObjectType { get; set; }
        public string TriggerChange { get; set; }
        public string TriggerType { get; set; }

        public int? TriggerConditionScoreFrom { get; set; }
        public int? TriggerConditionScoreThru { get; set; }

        public TTaskPrerequisite Clone()
        {
            var clone = new TTaskPrerequisite();
            this.ShallowCopyTo(clone);
            return clone;
        }
    }
}
