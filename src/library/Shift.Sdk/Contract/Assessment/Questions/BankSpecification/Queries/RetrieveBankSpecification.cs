using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveBankSpecification : Query<BankSpecificationModel>
    {
        public Guid SpecIdentifier { get; set; }
    }
}