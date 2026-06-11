using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ConnectCourseCatalog : Command, IHasRun
    {
        public Guid? CatalogId { get; set; }

        public ConnectCourseCatalog(Guid courseId, Guid? catalogId)
        {
            AggregateIdentifier = courseId;
            CatalogId = catalogId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (course.Data.GetGuidValue(CourseField.CatalogIdentifier) == CatalogId)
                return false;

            course.Apply(new CourseCatalogConnected(CatalogId));
            return true;
        }
    }
}
