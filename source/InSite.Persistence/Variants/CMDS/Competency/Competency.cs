using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class Competency
    {
        public Guid StandardIdentifier { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid CreatedBy { get; set; }
        public Boolean IsDeleted { get; set; }
        public String Knowledge { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }
        public String Number { get; set; }
        public String NumberOld { get; set; }
        public Decimal? ProgramHours { get; set; }
        public String Skills { get; set; }
        public String Summary { get; set; }
        public String Title { get; set; }
    }
}
