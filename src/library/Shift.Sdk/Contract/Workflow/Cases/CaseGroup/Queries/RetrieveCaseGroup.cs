using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveCaseGroup : Query<CaseGroupModel>
    {
        public Guid JoinIdentifier { get; set; }
    }
}