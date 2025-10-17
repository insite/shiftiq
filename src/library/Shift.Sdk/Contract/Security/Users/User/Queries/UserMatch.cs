using System;

namespace Shift.Contract
{
    public partial class UserMatch
    {
        public Guid UserIdentifier { get; set; }
        public string FullName { get; set; }
    }
}