using System.Collections.Generic;

namespace Shift.Toolbox.Progress
{
    public class LogbookModel
    {
        public string OrganizationLogoPath { get; set; }
        public string LogbookTitle { get; set; }
        public string PersonFullName { get; set; }
        public string PersonEmail { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<Area> Areas { get; set; }
        public List<Comment> Comments { get; set; }
        public bool ShowSkillRating { get; set; }
        public string NumberOfHoursText { get; set; }
    }
}
