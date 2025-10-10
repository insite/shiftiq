using System;

namespace Shift.Contract.Responses
{
    public class ObjectResponse
    {
        public Guid Identifier { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
