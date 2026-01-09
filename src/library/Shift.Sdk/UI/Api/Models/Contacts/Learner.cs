using System;

namespace Shift.Sdk.UI
{
    public class Learner
    {
        public Guid Identifier { get; set; }
        public string Email { get; set; }
        public PersonName Name { get; set; }

        public Learner()
        {
            Name = new PersonName();
        }
    }
}