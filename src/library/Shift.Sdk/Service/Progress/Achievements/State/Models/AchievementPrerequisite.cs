using System;
using System.Collections.Generic;

namespace InSite.Domain.Records
{
    public class AchievementPrerequisite
    {
        public Guid Identifier { get; set; }

        /// <summary>
        /// This is a list of Achievement identifiers. A prerequisite is understood to be satisfied if the learner has been granted ANY ONE of the
        /// achievements in this list. In other words, each "condition" is an achievement.
        /// </summary>
        /// <example>
        /// Mathematics 271 OR Mathematics 273
        /// </example>
        public List<Guid> Conditions { get; set; }

        public AchievementPrerequisite()
        {
            Conditions = new List<Guid>();
        }
    }
}
