using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveFileActivity : Query<FileActivityModel>
    {
        public Guid ActivityId { get; set; }
    }
}