using System.Linq;

namespace Shift.Contract
{
    public partial class FormWorkshop
    {
        public class QuestionStatistics
        {
            public class QuestionPerTagAndTaxonomy
            {
                public string Tag { get; set; }
                public int Taxonomy { get; set; }
                public int Count { get; set; }
            }

            public class QuestionPerIntItem
            {
                public int Item { get; set; }
                public int Count { get; set; }
            }

            public class QuestionPerStringItem
            {
                public string Item { get; set; }
                public int Count { get; set; }
            }

            public class AssessmentStandard
            {
                public string SetStandardCode { get; set; }
                public string QuestionStandardCode { get; set; }
                public int Questions { get; set; }
                public int?[] Taxonomies { get; set; }
            }

            public class SubCompetency
            {
                public string SetStandardCode { get; set; }
                public string QuestionStandardCode { get; set; }
                public string QuestionSubCode { get; set; }
                public int Questions { get; set; }
            }

            public class TagAndTaxonomy
            {
                public string Tag { get; set; }
                public int[] CountPerTaxonomy { get; set; }
            }

            public QuestionPerTagAndTaxonomy[] QuestionPerTagAndTaxonomyArray { get; set; }
            public QuestionPerIntItem[] QuestionPerTaxonomyArray { get; set; }
            public QuestionPerIntItem[] QuestionPerDifficultyArray { get; set; }
            public QuestionPerStringItem[] QuestionPerGACArray { get; set; }
            public QuestionPerStringItem[] QuestionPerCodeArray { get; set; }
            public QuestionPerStringItem[] QuestionPerLIGArray { get; set; }
            public int[] Taxonomies { get; set; }
            public AssessmentStandard[] Standards { get; set; }
            public SubCompetency[] SubCompetencies { get; set; }
            public TagAndTaxonomy[] TagAndTaxonomyArray { get; set; }
        }
    }
}
