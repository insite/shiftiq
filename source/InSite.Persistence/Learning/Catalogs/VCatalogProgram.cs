using System;

namespace InSite.Persistence
{
    public class VCatalogProgram
    {
        public Guid CatalogIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid ProgramCategoryItemIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string CatalogName { get; set; }
        public string ProgramCategory { get; set; }
        public string ProgramImage { get; set; }
        public string ProgramName { get; set; }
        public string ProgramSlug { get; set; }
        public string ProgramDescription { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }

        public bool CatalogIsHidden { get; set; }

        public DateTimeOffset ProgramCreated { get; set; }
        public DateTimeOffset ProgramModified { get; set; }
    }
}
