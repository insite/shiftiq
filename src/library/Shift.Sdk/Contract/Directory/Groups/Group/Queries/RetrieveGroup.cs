using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveGroup : Query<GroupModel>
    {
        public Guid GroupIdentifier { get; set; }
    }
}