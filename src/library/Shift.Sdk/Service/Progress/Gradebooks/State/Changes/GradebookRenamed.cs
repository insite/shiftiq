using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookRenamed : Change
    {
        public string Name { get; set; }

        public GradebookRenamed(string name)
        {
            Name = name;
        }
    }
}
