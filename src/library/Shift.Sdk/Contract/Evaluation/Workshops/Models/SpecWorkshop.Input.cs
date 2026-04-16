using System;

namespace Shift.Contract
{
    public partial class SpecWorkshop
    {
        public class Input
        {
            public class InputCompetency
            {
                public Guid StandardId { get; set; }
                public int? Tax1Count { get; set; }
                public int? Tax2Count { get; set; }
                public int? Tax3Count { get; set; }
            }
            
            public class InputCriterion
            {
                public Guid CriterionId { get; set; }
                public int Weight { get; set; }
                public InputCompetency[] Competencies { get; set; }
            }

            public int FormLimit { get; set; }
            public int QuestionLimit { get; set; }
            public InputCriterion[] Criteria { get; set; }
        }
    }
}
