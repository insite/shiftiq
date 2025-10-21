using System;

using Shift.Common;

namespace InSite.Persistence
{
    public class TMeasurement
    {
        public Guid? ContainerIdentifier { get; set; }
        public Guid MeasurementIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string ContainerName { get; set; }
        public string ContainerType { get; set; }
        public string IntervalType { get; set; }
        public string QuantityDeltaText { get; set; }
        public string QuantityFunction { get; set; }
        public string QuantityType { get; set; }
        public string QuantityUnit { get; set; }
        public string QuantityValueText { get; set; }
        public string UniquePath { get; set; }
        public string VariableItem { get; set; }
        public string VariableList { get; set; }
        public string VariableRoot { get; set; }

        public int AsAtDay { get; set; }
        public int AsAtMonth { get; set; }
        public int AsAtQuarter { get; set; }
        public int AsAtWeek { get; set; }
        public int AsAtYear { get; set; }
        public int UniquePathHash { get; set; }

        public decimal? QuantityDelta { get; set; }
        public decimal QuantityValue { get; set; }

        public DateTimeOffset AsAt { get; set; }
        public DateTime AsAtDate { get; set; }

        private const string Hyphen = "-";

        public void PrepareToSave(Guid organization, string containerType = null, Guid? containerId = null, string containerName = null)
        {
            if (MeasurementIdentifier == Guid.Empty)
                MeasurementIdentifier = UniqueIdentifier.Create();

            OrganizationIdentifier = organization;
            ContainerType = containerType;
            ContainerIdentifier = containerId;
            ContainerName = containerName;

            AsAtDate = new DateTime(AsAt.Year, AsAt.Month, AsAt.Day);
            AsAtYear = AsAt.Year;
            AsAtMonth = AsAt.Month;
            AsAtDay = AsAt.Day;

            var calendar = new System.Globalization.CultureInfo("en-US").Calendar;
            AsAtWeek = calendar.GetWeekOfYear(AsAt.Date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            AsAtQuarter = (AsAt.Month + 2) / 3;

            UniquePath = CalculateUniquePath();
            UniquePathHash = CalculateUniquePathHash();
        }

        public string CalculateUniquePath()
            => $"{VariableRoot ?? Hyphen}/{VariableList ?? Hyphen}/{VariableItem ?? Hyphen}";

        public int CalculateUniquePathHash()
            => CalculateUniquePath().GetHashCode();
    }
}
