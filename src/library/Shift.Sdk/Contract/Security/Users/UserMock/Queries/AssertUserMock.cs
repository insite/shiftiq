using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertUserMock : Query<bool>
    {
        public Guid MockIdentifier { get; set; }
    }
}