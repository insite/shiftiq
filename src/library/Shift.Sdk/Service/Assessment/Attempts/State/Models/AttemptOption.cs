namespace InSite.Domain.Attempts
{
    public class AttemptOption
    {
        public int Key { get; set; }
        public decimal Points { get; set; }
        public decimal? CutScore { get; set; }
        public string Text { get; set; }
        public bool? IsTrue { get; set; }
    }

    public class AttemptOptionHotspot : AttemptOption
    {
        public string Shape { get; set; }
    }
}
