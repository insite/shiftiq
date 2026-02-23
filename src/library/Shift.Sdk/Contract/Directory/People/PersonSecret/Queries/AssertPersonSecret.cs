using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertPersonSecret : Query<bool>
    {
        public Guid SecretId { get; set; }
    }
}