using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveCase : Query<CaseModel>
    {
        public Guid CaseIdentifier { get; set; }
    }
}