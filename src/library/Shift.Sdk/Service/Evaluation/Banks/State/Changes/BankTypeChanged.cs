using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class BankTypeChanged : Change
    {
        public string Type { get; set; }

        public BankTypeChanged(string type)
        {
            Type = type;
        }
    }
}
