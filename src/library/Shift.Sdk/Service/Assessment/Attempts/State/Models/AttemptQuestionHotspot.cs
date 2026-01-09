namespace InSite.Domain.Attempts
{
    public class AttemptQuestionHotspot : AttemptQuestion
    {
        public int PinLimit { get; set; }
        public bool ShowShapes { get; set; }
        public string Image { get; set; }
        public AttemptOptionHotspot[] Options { get; set; }
    }
}
