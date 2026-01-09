using System.Collections.Generic;

using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class ExamSubmissionPerson
    {
        [JsonProperty("individualID")]
        public int IndividualId { get; set; }

        [JsonProperty("isNoShow")]
        public bool IsNoShow { get; set; }

        [JsonProperty("examID")]
        public string ExamId { get; set; }

        [JsonProperty("mark")]
        public int Mark { get; set; }

        [JsonProperty("topicResults")]
        public List<ExamSubmissionTopic> Topics { get; set; }

        public void AddTopic(string name, int mark)
        {
            Topics.Add(new ExamSubmissionTopic(name, mark));
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

        public ExamSubmissionPerson()
        {
            Topics = new List<ExamSubmissionTopic>();
        }
    }
}