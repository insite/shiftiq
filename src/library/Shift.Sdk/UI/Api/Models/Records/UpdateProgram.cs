using System;

namespace Shift.Sdk.UI
{
    public class UpdateProgram
    {
        public string ProgramName { get; set; }
        public string ProgramType { get; set; }

        public string AssessmentFormCode { get; set; }
        public string AssessmentFormName { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}