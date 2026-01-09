using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class ExamTypeChanged : Change
    {
        public string Type { get; set; }

        public ExamTypeChanged(string type)
        {
            Type = type;
        }
    }
}