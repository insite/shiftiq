using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveGroupTag : Query<GroupTagModel>
    {
        public Guid TagIdentifier { get; set; }
    }
}