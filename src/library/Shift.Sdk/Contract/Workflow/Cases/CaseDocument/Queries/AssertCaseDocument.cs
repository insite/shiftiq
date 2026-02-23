using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertCaseDocument : Query<bool>
    {
        public Guid AttachmentId { get; set; }
    }
}