using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class StatementData
    {
        public Guid ActivityIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public List<Statement> Statements { get; set; }
    }
}
