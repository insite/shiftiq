using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class BankRenamed : Change
    {
        public string Name { get; set; }

        public BankRenamed(string name)
        {
            Name = name;
        }
    }
}
