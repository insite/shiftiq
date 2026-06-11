using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class MembershipFunctionModified : Change
    {
        public MembershipFunctionModified(string function)
        {
            Function = function;
        }

        public string Function { get; set; }
    }
}