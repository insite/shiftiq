using System;

namespace Shift.Sdk.UI
{
    public class ProgramTaskItem
    {
        public Guid ObjectIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid TaskIdentifier { get; set; }

        public bool IsSelected { get; set; }

        public string ObjectType { get; set; }
        public string TaskName { get; set; }
        public string TaskCompletionRequirement { get; set; }
    }
}