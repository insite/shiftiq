using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Glossaries
{
    public class GlossaryInitialized : Change
    {
        public Guid Tenant { get; set; }

        public GlossaryInitialized(Guid tenant)
        {
            Tenant = tenant;
        }
    }

    public class GlossaryInitialized2 : Change
    {
        public GlossaryInitialized2() { }
    }
}