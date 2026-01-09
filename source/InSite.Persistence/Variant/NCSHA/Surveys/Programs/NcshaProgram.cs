using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class NcshaProgram
    {
        public Guid AgencyGroupIdentifier { get; set; }
        public int ProgramYear { get; set; }
        public string StateName { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramCode { get; set; }
    }
}