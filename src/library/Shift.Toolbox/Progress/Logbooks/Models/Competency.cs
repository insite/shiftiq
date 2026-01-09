namespace Shift.Toolbox.Progress
{
    public class Competency
    {
        public int Sequence { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
        public string Hours { get; set; }
        public decimal? RequiredHours { get; set; }
        public decimal? CompletedHours { get; set; }
        public string JournalItems { get; set; }
        public int? RequiredJournalItems { get; set; }
        public int? CompletedJournalItems { get; set; }
        public string SkillRating { get; set; }
        public string SatisfactionLevel { get; set; }
        public bool IncludeHoursToArea { get; set; }
    }
}
