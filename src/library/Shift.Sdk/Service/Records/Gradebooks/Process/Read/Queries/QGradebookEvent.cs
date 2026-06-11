using System;

using InSite.Application.Events.Read;

namespace InSite.Application.Records.Read
{
    public class QGradebookEvent
    {
        public Guid GradebookIdentifier { get; set; }
        public Guid EventIdentifier { get; set; }

        public virtual QGradebook Gradebook { get; set; }
        public virtual QEvent Event { get; set; }
    }
}
