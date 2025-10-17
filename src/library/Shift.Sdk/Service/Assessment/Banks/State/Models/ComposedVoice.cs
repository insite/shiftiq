using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class ComposedVoice
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int TimeLimit
        {
            get => _timeLimit;
            set => _timeLimit = Number.CheckRange(value, 0);
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int AttemptLimit
        {
            get => _attemptLimit;
            set => _attemptLimit = Number.CheckRange(value, 0);
        }

        public bool IsEmpty => _attemptLimit == 0 && _timeLimit == 0;

        private int _timeLimit;
        private int _attemptLimit;

        public ComposedVoice Clone()
        {
            var copy = new ComposedVoice();

            this.CopyTo(copy);

            return copy;
        }

        public void CopyTo(ComposedVoice other)
        {
            other._timeLimit = this._timeLimit;
            other._attemptLimit = this._attemptLimit;
        }

        public bool IsEqual(ComposedVoice other)
        {
            return this._timeLimit == other._timeLimit
                && this._attemptLimit == other._attemptLimit;
        }
    }
}
