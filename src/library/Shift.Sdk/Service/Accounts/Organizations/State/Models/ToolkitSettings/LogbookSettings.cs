using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class LogbookSettings
    {
        public bool DisplayTotalLogbookHours { get; set; }
        public bool LogbookBulkEntry { get; set; }

        public bool IsEqual(LogbookSettings other)
        {
            return DisplayTotalLogbookHours == other.DisplayTotalLogbookHours
                && LogbookBulkEntry == other.LogbookBulkEntry
                ;
        }
    }
}
