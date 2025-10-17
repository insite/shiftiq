using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace Shift.Toolbox.Progress
{
    public class Area
    {
        public int Sequence { get; set; }
        public string Name { get; set; }
        public List<Competency> Competencies { get; set; }
        public decimal? RequiredHours { get; set; }

        public bool HasRequiredHours => RequiredHours.HasValue
            || Competencies.Any(x => x.IncludeHoursToArea && x.RequiredHours.HasValue && x.RequiredHours.Value > 0);

        public decimal HoursCompleted
        {
            get
            {
                if (Competencies.Count == 0)
                    return 0;

                decimal required = 0, completed = 0;

                if (!RequiredHours.HasValue)
                {
                    foreach (var competency in Competencies)
                    {
                        if (!competency.IncludeHoursToArea || (competency.RequiredHours ?? 0) <= 0)
                            continue;

                        required += competency.RequiredHours.Value;
                        completed += competency.CompletedHours.Value;
                    }
                }
                else
                {
                    required = RequiredHours.Value;
                    completed = (decimal)Competencies.Where(x => x.IncludeHoursToArea).Sum(x => x.CompletedHours);
                }

                if (completed > required)
                    completed = required;

                return completed > 0 ? Calculator.GetPercentDecimal(completed, required) : 0;
            }
        }
    }
}
