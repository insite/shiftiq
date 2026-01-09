using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class BankEditionChanged : Change
    {
        public string Major { get; set; }
        public string Minor { get; set; }

        public BankEditionChanged(string major, string minor)
        {
            Major = major;
            Minor = minor;
        }
    }
}
