using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;

namespace InSite.Application.Standards.Read
{
    public class VStandard
    {
        public Guid? ParentStandardIdentifier { get; set; }

        public Int32? StandardAsset { get; set; }
        public String StandardCode { get; set; }
        public Guid StandardIdentifier { get; set; }
        public String StandardLabel { get; set; }
        public String StandardTitle { get; set; }
        public String StandardType { get; set; }
        public string CompetencyScoreSummarizationMethod { get; set; }
        public string CompetencyScoreCalculationMethod { get; set; }
        public int? CalculationArgument { get; set; }
        public decimal? MasteryPoints { get; set; }
        public decimal? PointsPossible { get; set; }

        public virtual VStandard Parent { get; set; }
        public virtual ICollection<VStandard> Children { get; set; }
        public virtual ICollection<QGradeItemCompetency> GradeItemCompetencies { get; set; }
        public virtual ICollection<Records.Read.QGradebookCompetencyValidation> Validations { get; set; }

        public VStandard()
        {
            Children = new HashSet<VStandard>();
            GradeItemCompetencies = new HashSet<QGradeItemCompetency>();
            Validations = new HashSet<Records.Read.QGradebookCompetencyValidation>();
        }
    }
}
