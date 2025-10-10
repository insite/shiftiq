using System;

namespace Shift.Contract.Responses
{
    public class OrganizationSearchResponse
    {
        public Guid Identifier { get; set; }

        public string Domain { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Status { get; set; }

        public DateTimeOffset? Opened { get; set; }
        public DateTimeOffset? Closed { get; set; }
    }
}