using System.Collections.Generic;

namespace Shift.Toolbox.Records
{
    public class Experience
    {
        public int Sequence { get; set; }
        public string ExperienceCreated { get; set; }
        public string Status { get; set; }
        public List<ExperienceField> ExperienceFields { get; set; }
    }
}
