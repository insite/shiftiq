using Shift.Common.Timeline.Changes;

namespace InSite.Persistence.Plugin.CMDS
{
    public class CmdsCompetencyChanged : Change
    {
        public string Author { get; set; }
        public string Change { get; set; }

        public CmdsCompetencyChanged(string author, string change)
        {
            Author = author;
            Change = change;
        }
    }
}