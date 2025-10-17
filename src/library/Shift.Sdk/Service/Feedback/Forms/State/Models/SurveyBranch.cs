namespace InSite.Domain.Surveys.Forms
{
    public class SurveyBranch
    {
        public SurveyOptionItem FromOptionItem { get; set; }
        public SurveyQuestion ToQuestion { get; set; }
    }
}