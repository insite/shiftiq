using System;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class TTaskFilter : Filter
    {
        public Guid? ObjectIdentifier { get; set; }
        public List<Guid> OrganizationIdentifiers { get; set; } = new List<Guid>();
        public Guid? ProgramIdentifier { get; set; }
        public Guid? ExcludedTask { get; set; }
        public Guid? ExcludedObject { get; set; }
        public string[] ExcludeObjectTypes { get; set; }
    }
}
