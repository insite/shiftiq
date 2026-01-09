using QuestPDF.Fluent;
using Shift.Utility.Assessments.Models;


namespace Shift.Toolbox.Assessments
{
    public class AssessmentResultCreator
    {
        public byte[] CreatePdf(PersonAssessment personAssessmentModel)
        {
            return new AssessmentResultDocument(personAssessmentModel).GeneratePdf();
        }
    }
}