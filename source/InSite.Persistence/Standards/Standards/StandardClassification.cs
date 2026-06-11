using System;

namespace InSite.Persistence
{
    public class StandardClassification
    {
        public Guid CategoryIdentifier { get; set; }
        public Guid StandardIdentifier { get; set; }
        public Int32? ClassificationSequence { get; set; }
    }
}
