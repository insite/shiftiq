using System;

using InSite.Application.Standards.Read;

namespace InSite.Application.Records.Read
{
    public class QAreaRequirement
    {
        public Guid JournalSetupIdentifier { get; set; }
        public Guid AreaStandardIdentifier { get; set; }
        public decimal? AreaHours { get; set; }

        public virtual QJournalSetup JournalSetup { get; set; }
        public virtual QStandard AreaStandard { get; set; }
    }
}
