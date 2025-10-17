using System.Collections.Generic;

using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class ExamSubmissionSession
    {
        [JsonProperty("sessionID")]
        public string SessionId { get; set; }

        [JsonProperty("completedDate")]
        public string ExamCompletedDate { get; set; }

        [JsonProperty("location")]
        public string ExamLocation { get; set; }

        [JsonProperty("examResults")]
        public List<ExamSubmissionPerson> Persons { get; set; }

        public ExamSubmissionSession()
        {
            Persons = new List<ExamSubmissionPerson>();
        }

        private readonly Dictionary<string, string> _variables = new Dictionary<string, string>();

        public void SetVariable(string name, string value)
        {
            _variables[name] = value;
        }

        public string GetVariable(string name)
        {
            if (_variables.TryGetValue(name, out string value))
                return value;

            return null;
        }
    }
}