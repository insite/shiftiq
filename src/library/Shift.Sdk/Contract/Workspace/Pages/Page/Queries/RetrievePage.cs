using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrievePage : Query<PageModel>
    {
        public Guid PageIdentifier { get; set; }
    }
}