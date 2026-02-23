using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class BankOpened : Change
    {
        public BankState Bank { get; set; }

        public BankOpened(BankState bank)
        {
            Bank = bank;
        }
    }
}
