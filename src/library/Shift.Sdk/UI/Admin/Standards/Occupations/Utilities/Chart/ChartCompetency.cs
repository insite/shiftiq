namespace Shift.Sdk.UI
{
    public class ChartCompetency
    {
        public ChartCompetency(string letter, int number, string name)
        {
            Letter = letter;
            Number = number;
            Name = name;
        }

        public string Letter { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }

        public bool HasLevel1 { get; set; }
        public bool HasLevel2 { get; set; }
        public bool HasLevel3 { get; set; }
        public bool HasLevel4 { get; set; }

        public string Level1Url { get; set; }
        public string Level2Url { get; set; }
        public string Level3Url { get; set; }
        public string Level4Url { get; set; }
    }
}