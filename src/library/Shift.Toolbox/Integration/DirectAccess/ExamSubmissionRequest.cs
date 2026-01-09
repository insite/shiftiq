using System.Collections.Generic;

using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class ExamSubmissionRequest
    {
        public List<ExamSubmissionSession> Sessions { get; set; }

        public ExamSubmissionRequest()
        {
            Sessions = new List<ExamSubmissionSession>();
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(Sessions);
        }
    }
}