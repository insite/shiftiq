using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveSite : Query<SiteModel>
    {
        public Guid SiteId { get; set; }
    }
}