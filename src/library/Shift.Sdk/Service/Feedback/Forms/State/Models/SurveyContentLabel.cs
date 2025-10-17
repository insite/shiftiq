namespace InSite.Domain.Surveys.Forms
{
    public class SurveyContentLabel
    {
        public string Name { get; }
        public string Description { get; }

        public SurveyContentLabel(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
