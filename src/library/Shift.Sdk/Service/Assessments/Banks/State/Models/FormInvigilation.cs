using System;
using System.ComponentModel;

using Shift.Common;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// Invigilation properties determine the rules applied to an online exam submission.
    /// </summary>
    [Serializable]
    public class FormInvigilation
    {
        public bool IsSafeExamBrowserRequired { get; set; }
        public bool IsKioskModeRequired { get; set; }
        public DateTimeOffset? Opened { get; set; }
        public DateTimeOffset? Closed { get; set; }
        public int TimeLimit { get; set; }

        [DefaultValue(true)]
        public bool IsTimerVisible { get; set; } = true;

        public int AttemptLimit { get; set; }

        public int AttemptLimitPerSession { get; set; }
        public int TimeLimitPerSession { get; set; }
        public int TimeLimitPerLockout { get; set; }

        public FormInvigilation Clone()
        {
            var clone = new FormInvigilation();

            this.ShallowCopyTo(clone);

            return clone;
        }

        #region Methods (comparison)

        public bool Equals(FormInvigilation invigilation)
        {
            return invigilation != null
                && this.IsSafeExamBrowserRequired == invigilation.IsSafeExamBrowserRequired
                && this.IsKioskModeRequired == invigilation.IsKioskModeRequired
                && this.Opened == invigilation.Opened
                && this.Closed == invigilation.Closed
                && this.TimeLimit == invigilation.TimeLimit
                && this.IsTimerVisible == invigilation.IsTimerVisible
                && this.AttemptLimit == invigilation.AttemptLimit
                && this.AttemptLimitPerSession == invigilation.AttemptLimitPerSession
                && this.TimeLimitPerSession == invigilation.TimeLimitPerSession
                && this.TimeLimitPerLockout == invigilation.TimeLimitPerLockout;
        }

        #endregion
    }
}
