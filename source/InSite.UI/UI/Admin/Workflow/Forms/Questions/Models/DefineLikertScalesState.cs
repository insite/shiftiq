using InSite.Domain.Surveys.Forms;

namespace InSite.Admin.Workflow.Forms.Questions.Models
{
    public class DefineLikertScalesState
    {
        public SurveyState Survey { get; set; }
        public SurveyQuestion Question { get; set; }
    }
}