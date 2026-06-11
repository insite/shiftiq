using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveFileClaim : Query<FileClaimModel>
    {
        public Guid ClaimId { get; set; }
    }
}