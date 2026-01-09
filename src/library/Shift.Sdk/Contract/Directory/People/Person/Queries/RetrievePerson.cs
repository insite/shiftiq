using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrievePerson : Query<PersonModel>
    {
        public Guid PersonIdentifier { get; set; }
    }
}