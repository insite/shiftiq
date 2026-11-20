using System;

namespace Shift.Common
{
    public class TimeZoneAbbreviation
    {
        #region Properties

        public string TimeZoneId { get; }

        public string DisplayName { get; }

        public string DisplayNameWithoutOffset { get; }

        public string Generic { get; }

        public string Standard { get; }

        public string Daylight { get; }

        public bool IsIANACompatible { get; }

        #endregion

        #region Construction

        public TimeZoneAbbreviation(string timeZoneId, string offset, string name, string generic, string standard, string daylight, bool isIANACompatible)
        {
            TimeZoneId = timeZoneId;
            DisplayName = $"({offset}) {name}";
            DisplayNameWithoutOffset = name;
            Generic = generic;
            Standard = standard;
            Daylight = daylight;
            IsIANACompatible = isIANACompatible;
        }

        #endregion

        #region Methods

        public string GetAbbreviation(DateTimeOffset time)
        {
            return IsIANACompatible
                ? TimeZones.IsDaylightSavingTime(time, TimeZoneId) ? Daylight : Standard
                : Generic;
        }

        #endregion
    }
}
