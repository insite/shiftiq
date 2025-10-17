using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertPersonSecret : Query<bool>
    {
        public Guid SecretIdentifier { get; set; }
    }
}