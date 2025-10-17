using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveFileClaim : Query<FileClaimModel>
    {
        public Guid ClaimIdentifier { get; set; }
    }
}