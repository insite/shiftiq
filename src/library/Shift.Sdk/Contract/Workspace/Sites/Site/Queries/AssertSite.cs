using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertSite : Query<bool>
    {
        public Guid SiteId { get; set; }
    }
}