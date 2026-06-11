using System;

namespace InSite.Domain.Standards
{
    public class StandardCategory
    {
        public Guid CategoryId { get; set; }
        public int? Sequence { get; set; }

        public StandardCategory()
        {

        }

        public StandardCategory(Guid categoryId, int? sequence)
        {
            CategoryId = categoryId;
            Sequence = sequence;
        }

        public StandardCategory Clone()
        {
            return (StandardCategory)MemberwiseClone();
        }
    }
}
