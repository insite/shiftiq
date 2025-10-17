using System;

namespace InSite.Domain.Standards
{
    public class StandardContainment
    {
        public Guid ChildStandardId { get; set; }
        public int ChildSequence { get; set; }
        public decimal? CreditHours { get; set; }
        public string CreditType { get; set; }

        public StandardContainment()
        {

        }

        public StandardContainment(Guid childStandardId, int childSequence, decimal? creditHours, string creditType)
        {
            ChildStandardId = childStandardId;
            ChildSequence = childSequence;
            CreditHours = creditHours;
            CreditType = creditType;
        }

        public StandardContainment Clone()
        {
            return (StandardContainment)MemberwiseClone();
        }
    }
}
