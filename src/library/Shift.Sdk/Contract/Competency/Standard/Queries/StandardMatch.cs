using System;

namespace Shift.Contract
{
    public partial class StandardMatch
    {
        public Guid Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
    }
}