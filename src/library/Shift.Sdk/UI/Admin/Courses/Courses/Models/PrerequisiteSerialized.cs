using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class PrerequisiteSerialized
    {
        public string TriggerChange { get; set; }
        public string TriggerType { get; set; }
        public string Trigger { get; set; }
        public int? TriggerConditionScoreFrom { get; set; }
        public int? TriggerConditionScoreThru { get; set; }
    }
}