using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class ExamSubmissionTopic
    {
        private const int MaximumTopicNameLength = 100;

        private string _name;

        [JsonProperty("topicName")]
        public string Name
        {
            get => _name;
            set
            {
                if (value != null && value.Length > MaximumTopicNameLength)
                    _name = value.Substring(0, MaximumTopicNameLength - 3) + "...";
                else
                    _name = value;
            }
        }

        [JsonProperty("topicMark")]
        public int Mark { get; set; }

        public ExamSubmissionTopic() { }

        public ExamSubmissionTopic(string name, int mark)
        {
            Name = name;
            Mark = mark;
        }
    }
}