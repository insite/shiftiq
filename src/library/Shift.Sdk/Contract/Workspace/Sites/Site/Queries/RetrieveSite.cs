using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveSite : Query<SiteModel>
    {
        public Guid SiteIdentifier { get; set; }
    }
}