using System;

namespace Shift.Common
{
    public class TimeZoneAbbreviation
    {
        #region Properties

        public string Generic { get; }

        public string Standard { get; }

        public string Daylight { get; }

        public string Moment { get; }

        #endregion

        #region Fields

        private readonly TimeZoneInfo _zone;

        #endregion

        #region Construction

        public TimeZoneAbbreviation(TimeZoneInfo zone, string generic, string standard, string daylight, string moment)
        {
            _zone = zone;

            Generic = generic;
            Standard = standard;
            Daylight = daylight;
            Moment = moment;
        }

        #endregion

        #region Methods

        public string GetAbbreviation(DateTimeOffset time) =>
            _zone.IsDaylightSavingTime(time) ? Daylight : Standard;

        #endregion
    }
}