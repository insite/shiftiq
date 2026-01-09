using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveUserMock : Query<UserMockModel>
    {
        public Guid MockIdentifier { get; set; }
    }
}