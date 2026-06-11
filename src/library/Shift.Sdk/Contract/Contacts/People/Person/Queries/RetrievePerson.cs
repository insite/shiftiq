using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrievePerson : Query<PersonModel>
    {
        public Guid PersonId { get; set; }
    }
}