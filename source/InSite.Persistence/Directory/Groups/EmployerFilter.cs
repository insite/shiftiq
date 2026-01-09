using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class EmployerFilter : Filter
    {
        public string SortByColumn { get; set; }

        public Guid? OrganizationIdentifier { get; set; }
        public string EmployerName { get; set; }
    }
}