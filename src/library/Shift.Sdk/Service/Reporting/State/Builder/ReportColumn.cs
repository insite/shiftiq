using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace InSite.Domain.Reports
{
    [Serializable]
    public class ReportColumn
    {
        private static readonly HashSet<string> NumberDataTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { 
            "decimal", "float", "int", "money", "numeric", "bigint", "smallmoney" 
        };

        public string Name { get; set; }
        public string DataType { get; set; }

        [JsonIgnore]
        public string ViewAlias
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return null;

                var index = Name.IndexOf('.');

                return index >= 0
                    ? Name.Substring(0, index)
                    : null;
            }
        }

        [JsonIgnore]
        public string NameWithoutView
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return null;

                var index = Name.IndexOf('.');

                return index >= 0
                    ? Name.Substring(index + 1)
                    : Name;
            }
        }

        [JsonIgnore]
        public bool IsNumeric => NumberDataTypes.Contains(DataType);

        [JsonIgnore]
        public bool IsBit => string.Equals(DataType, "bit", StringComparison.OrdinalIgnoreCase);

        [JsonIgnore]
        public bool IsText => string.Equals(DataType, "varchar", StringComparison.OrdinalIgnoreCase) 
            || string.Equals(DataType, "char", StringComparison.OrdinalIgnoreCase)
            || string.Equals(DataType, "text", StringComparison.OrdinalIgnoreCase);

        [JsonIgnore]
        public bool IsStatistic => Name.StartsWith(ReportAggregate.ViewAlias + ".");
    }
}
