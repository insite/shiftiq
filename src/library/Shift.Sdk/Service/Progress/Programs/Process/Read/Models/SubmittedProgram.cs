using System;
using System.Collections.Generic;

namespace InSite.Application.Records.Read
{
    public class SubmittedProgram
    {
        public Guid ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramSummary { get; set; }
        public List<string> CategoryNames { get; set; }
    }
}
