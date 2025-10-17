using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrievePersonField : Query<PersonFieldModel>
    {
        public Guid FieldIdentifier { get; set; }
    }
}