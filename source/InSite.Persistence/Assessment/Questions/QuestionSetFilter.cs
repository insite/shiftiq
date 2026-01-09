using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class QuestionSetFilter: Filter
    {
        public string Title { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
    }
}
