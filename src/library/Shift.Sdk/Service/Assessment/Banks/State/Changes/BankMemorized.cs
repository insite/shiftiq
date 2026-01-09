using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class BankMemorized : Change
    {
        public BankState Data { get; set; }

        public BankMemorized(BankState data)
        {
            Data = data;
        }
    }
}