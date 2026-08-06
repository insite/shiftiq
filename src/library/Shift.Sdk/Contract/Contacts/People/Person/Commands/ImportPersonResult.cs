using System;

using Shift.Common;

namespace Shift.Contract
{
    public class ImportPersonResult
    {
        public enum StatusEnum { Modified, NotChanged, Created, Pending, Error }

        public ImportPerson Input { get; }
        public StatusEnum Status { get; set; }
        public Guid? PendingPersonId { get; set; }
        public Guid? UserId { get; set; }
        public ValidationFailure Failure { get; set; }

        public ImportPerson Original { get; set; }

        public ImportPersonResult(ImportPerson import)
        {
            Input = import;
        }
    }
}
