using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertSubmission : Query<bool>
    {
        public Guid ResponseSessionIdentifier { get; set; }
    }
}