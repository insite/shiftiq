using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveGradebook : Query<GradebookModel>
    {
        public Guid GradebookIdentifier { get; set; }
    }
}