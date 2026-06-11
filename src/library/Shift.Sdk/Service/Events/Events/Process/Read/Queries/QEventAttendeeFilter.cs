using System;

using Shift.Common;

namespace InSite.Application.Events.Read
{
    [Serializable]
    public class QEventAttendeeFilter : Filter
    {
        public Guid? EventIdentifier { get; set; }
        public Guid? GradebookIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? ContactIdentifier { get; set; }
        public Guid[] ContactIdentifiers { get; set; }

        public string ContactKeyword { get; set; }
        public string ContactRole { get; set; }
    }
}
