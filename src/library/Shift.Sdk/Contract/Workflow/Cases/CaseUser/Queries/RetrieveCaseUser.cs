using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveCaseUser : Query<CaseUserModel>
    {
        public Guid JoinIdentifier { get; set; }
    }
}