using System;

namespace Shift.Common.File
{
    public class AttemptUploadFileLine
    {
        public string LearnerCode { get; set; }
        public string LearnerName { get; set; }
        
        public string Instructor { get; set; }

        public DateTime? AttemptDate { get; set; }
        public string[] AttemptAnswers { get; set; }
    }
}
