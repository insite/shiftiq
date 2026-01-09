using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveSubmission : Query<SubmissionModel>
    {
        public Guid ResponseSessionIdentifier { get; set; }
    }
}