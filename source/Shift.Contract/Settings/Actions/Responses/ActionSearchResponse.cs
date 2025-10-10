using System;

namespace Shift.Contract.Responses
{
    public class ActionSearchResponse
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
    }
}