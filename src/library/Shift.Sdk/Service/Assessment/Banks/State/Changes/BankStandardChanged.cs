using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class BankStandardChanged : Change
    {
        public Guid Standard { get; set; }

        public BankStandardChanged(Guid standard)
        {
            Standard = standard;
        }
    }
}
