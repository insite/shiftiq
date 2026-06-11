using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardCategoryAdded : Change
    {
        public StandardCategory[] Categories { get; }

        public StandardCategoryAdded(StandardCategory[] categories)
        {
            Categories = categories;
        }
    }
}
