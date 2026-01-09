using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class TaskInfo
    {
        public Guid ObjectIdentifier { get; set; }
        public Guid? TaskIdentifier { get; set; }
        public string Type { get; set; }
        public string TaskTitle { get; set; }
        public string DisplayTitle { get; set; }
        public int Sequence { get; set; }
    }
}