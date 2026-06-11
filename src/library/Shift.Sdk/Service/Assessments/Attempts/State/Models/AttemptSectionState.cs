using System;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Attempts
{
    public class AttemptSectionState
    {
        [JsonProperty]
        public int Index { get; private set; }

        [JsonProperty]
        public Guid? Identifier { get; private set; }

        [JsonProperty]
        public bool ShowWarningNextTab { get; private set; }

        [JsonProperty]
        public bool IsBreakTimer { get; private set; }

        [JsonProperty]
        public int TimeLimit { get; private set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public FormSectionTimeType TimerType { get; private set; }

        public DateTimeOffset? Started { get; set; }
        public DateTimeOffset? Completed { get; set; }
        public int? Duration { get; set; }

        [JsonConstructor]
        private AttemptSectionState()
        {

        }

        public AttemptSectionState(int index)
        {
            Index = index;
            ShowWarningNextTab = true;
            IsBreakTimer = false;
            TimeLimit = 0;
            TimerType = FormSectionTimeType.Optional;
        }

        public AttemptSectionState(AttemptSection model, int index)
        {
            Index = index;
            Identifier = model.Identifier;
            ShowWarningNextTab = model.ShowWarningNextTab;
            IsBreakTimer = model.IsBreakTimer;
            TimeLimit = model.TimeLimit;
            TimerType = model.TimerType;
        }
    }
}
