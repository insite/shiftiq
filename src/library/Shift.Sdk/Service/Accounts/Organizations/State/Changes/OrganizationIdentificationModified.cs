using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationIdentificationModified : Change
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerCode { get; set; }

        public OrganizationIdentificationModified(string code, string name, string customerCode, string customerNumber)
        {
            Code = code;
            Name = name;
            CustomerCode = customerCode;
            CustomerNumber = customerNumber;
        }
    }
}
