using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookWarningAdded : Change
    {
        public GradebookWarningAdded(string warning)
        {
            Warning = warning;
        }

        public string Warning { get; set; }
    }
}