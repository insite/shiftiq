using System;

namespace InSite.Domain.Reports
{
    [Serializable]
    public class ReportAggregate
    {
        public const string ViewAlias = "ST";
        public const string ViewName = "statistic.Aggregate";

        public enum FunctionType 
        { 
            Count,
            Sum,
            Min,
            Max,
            Mean,
            Median,
            Mode
        }

        public string Alias { get; set; }
        public FunctionType Function { get; set; }
        public ReportColumn Column { get; set; }

        public override string ToString()
        {
            switch (Function)
            {
                case FunctionType.Count: return $"COUNT(*)";
                case FunctionType.Sum: return $"SUM({Column.Name})";
                case FunctionType.Min: return $"MIN({Column.Name})";
                case FunctionType.Max: return $"MAX({Column.Name})";
                case FunctionType.Mean: return $"MEAN({Column.Name})";
                case FunctionType.Median: return $"MEDIAN({Column.Name})";
                case FunctionType.Mode: return $"MODE({Column.Name})";
                default: return "NA";
            }
        }
    }
}
