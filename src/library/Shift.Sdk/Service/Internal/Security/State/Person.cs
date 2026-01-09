using System;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class Person
    {
        public Guid Organization { get; set; }
        public Guid User { get; set; }

        public bool IsAdministrator { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsLearner { get; set; }
        public bool IsOperator { get; set; }
    }
}
