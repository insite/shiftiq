using System;

namespace InSite.Persistence
{
    [Serializable]
    public class VTaskEnrollment
    {
        public Guid ProgramIdentifier { get; set; }
        public Guid ObjectIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid TaskIdentifier { get; set; }
        public int CompletionCounter { get; set; }
        public string ObjectType { get; set; }
    }
}
