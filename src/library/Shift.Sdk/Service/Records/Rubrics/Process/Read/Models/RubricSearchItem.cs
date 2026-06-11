using System;

namespace InSite.Application.Records.Read
{
    public class RubricSearchItem
    {
        public Guid RubricIdentifier { get; set; }
        public string RubricTitle { get; set; }
        public decimal RubricPoints { get; set; }
        public int CriteriaCount { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
