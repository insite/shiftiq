using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationIdentificationModified : Change
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }

        public OrganizationIdentificationModified(string code, string name, string domain)
        {
            Code = code;
            Name = name;
            Domain = domain;
        }
    }
}
