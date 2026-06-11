using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;

namespace InSite.Application.Standards.Read
{
    public class VFramework
    {
        public Int32? FrameworkAsset { get; set; }
        public String FrameworkCode { get; set; }
        public Guid FrameworkIdentifier { get; set; }
        public String FrameworkLabel { get; set; }
        public Int32? FrameworkSize { get; set; }
        public String FrameworkTitle { get; set; }
        public Int32? OccupationAsset { get; set; }
        public String OccupationCode { get; set; }
        public Guid? OccupationIdentifier { get; set; }
        public String OccupationLabel { get; set; }
        public Int32? OccupationSize { get; set; }
        public String OccupationTitle { get; set; }

        public virtual ICollection<QGradebook> Gradebooks { get; set; } = new HashSet<QGradebook>();
        public virtual ICollection<QJournalSetup> JournalSetups { get; set; } = new HashSet<QJournalSetup>();
    }
}
