using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseCatalogConnected : Change
    {
        public Guid? CatalogId { get; set; }

        public CourseCatalogConnected(Guid? catalogId)
        {
            CatalogId = catalogId;
        }
    }
}
