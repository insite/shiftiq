using System;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class TProgramCategory
    {
        public Guid ItemIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public virtual TCollectionItem Category { get; set; }

        public virtual TProgram Program { get; set; }
    }
}
