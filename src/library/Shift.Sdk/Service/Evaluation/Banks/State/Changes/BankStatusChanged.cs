using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class BankStatusChanged : Change
    {
        public bool IsActive { get; set; }

        public BankStatusChanged(bool isActive)
        {
            IsActive = isActive;
        }
    }
}