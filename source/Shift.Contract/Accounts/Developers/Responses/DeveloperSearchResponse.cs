using System;

namespace Shift.Contract.Responses
{
    public class DeveloperSearchResponse
    {
        public Guid Identifier { get; set; }
        
        public string Name { get; set; }

        public DateTimeOffset? Issued { get; set; }
        public DateTimeOffset? Expiry { get; set; }
    }
}