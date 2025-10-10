using System;

namespace Shift.Contract.Requests
{
    public class GenerateTokenRequest
    {
        public string Secret { get; set; }
        public int? Lifetime { get; set; }
        public Guid? Organization { get; set; }
    }
}