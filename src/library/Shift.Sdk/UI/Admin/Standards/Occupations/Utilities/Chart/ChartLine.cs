using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class ChartLine
    {
        public ChartLine(string letter, string name)
        {
            Letter = letter;
            Name = name;
            Competencies = new SortedList<int, ChartCompetency>();
        }

        public string Letter { get; set; }
        public string Name { get; set; }
        public SortedList<int, ChartCompetency> Competencies { get; set; }
    }
}