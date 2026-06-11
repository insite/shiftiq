using System;

namespace Shift.Contract
{
    public partial class SpecWorkshop
    {
        public class SpecDetails
        {
            public class DetailsCompetency
            {
                public Guid StandardId { get; set; }
                public int? Tax1Count { get; set; }
                public int? Tax2Count { get; set; }
                public int? Tax3Count { get; set; }
                public int? QuestionCount { get; set; }
                public int Tax1CountActual { get; set; }
                public int Tax2CountActual { get; set; }
                public int Tax3CountActual { get; set; }
                public int? UnassignedCount { get; set; }
            }

            public class DetailsCriterion
            {
                public Guid CriterionId { get; set; }
                public string Title { get; set; }
                public int Weight { get; set; }
                public Guid[] StandardIds { get; set; }
                public DetailsCompetency[] Competencies { get; set; }
            }

            public string SpecName { get; set; }
            public int AssetNumber { get; set; }
            public Guid? FrameworkId { get; set; }
            public int FormLimit { get; set; }
            public int QuestionLimit { get; set; }
            public DetailsCriterion[] Criteria { get; set; }
        }
    }
}
