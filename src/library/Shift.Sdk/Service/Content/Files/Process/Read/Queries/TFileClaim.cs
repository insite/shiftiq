using System;

namespace InSite.Application.Files.Read
{
    public class TFileClaim
    {
        public Guid FileIdentifier { get; set; }
        public Guid ClaimIdentifier { get; set; }
        public string ObjectType { get; set; }
        public Guid ObjectIdentifier { get; set; }

        public DateTimeOffset? ClaimGranted { get; set; }

        public TFile File { get; set; }
    }
}
