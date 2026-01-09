using System;

namespace InSite.Application.Files.Read
{
    public class TFileActivity
    {
        public Guid FileIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid ActivityIdentifier { get; set; }
        public DateTimeOffset ActivityTime { get; set; }
        public string ActivityChanges { get; set; }

        public TFile File { get; set; }
    }
}
