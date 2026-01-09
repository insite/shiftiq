using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertGradebook : Query<bool>
    {
        public Guid GradebookIdentifier { get; set; }
    }
}