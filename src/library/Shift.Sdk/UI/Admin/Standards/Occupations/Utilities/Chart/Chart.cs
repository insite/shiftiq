using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class Chart
    {
        public Chart()
        {
            Lines = new SortedList<string, ChartLine>();
        }

        public SortedList<string, ChartLine> Lines { get; set; }

        public ChartCompetency AddCompetency(string letter, int number, string name)
        {
            var line = Lines[letter];

            if (line.Competencies.ContainsKey(number))
                return line.Competencies[number];

            var competency = new ChartCompetency(letter, number, name);
            line.Competencies.Add(number, competency);
            return competency;
        }

        public void AddLine(string letter, string name)
        {
            if (!Lines.ContainsKey(letter))
                Lines.Add(letter, new ChartLine(letter, name));
        }
    }
}