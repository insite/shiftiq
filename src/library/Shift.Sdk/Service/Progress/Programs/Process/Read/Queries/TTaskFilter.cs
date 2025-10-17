using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class TTaskFilter : Filter
    {
        public Guid? ObjectIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? ParentOrganizationIdentifier { get; set; }
        public Guid? ProgramIdentifier { get; set; }
        public Guid? ExcludedTask { get; set; }
        public Guid? ExcludedObject { get; set; }
        public string[] ExcludeObjectTypes { get; set; }
    }
}
