using InSite.Domain.Surveys.Forms;

namespace InSite.Admin.Surveys.Questions.Forms
{
    public class DefineLikertScalesState
    {
        public SurveyState Survey { get; set; }
        public SurveyQuestion Question { get; set; }
    }
}