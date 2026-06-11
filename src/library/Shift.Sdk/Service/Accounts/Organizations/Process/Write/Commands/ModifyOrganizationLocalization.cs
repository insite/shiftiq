using System;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationLocalization : Command, IHasRun
    {
        public string[] Languages { get; set; }
        public string TimeZone { get; set; }

        public ModifyOrganizationLocalization(Guid organizationId, string[] languages, string timeZone)
        {
            AggregateIdentifier = organizationId;
            Languages = languages;
            TimeZone = timeZone;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (Languages.IsEmpty() || TimeZone.IsEmpty())
                return true;

            var isSame = state.TimeZone?.Id == TimeZone.NullIfEmpty()
                && state.Languages.Length == Languages.Length
                && state.Languages
                    .Select(x => x.TwoLetterISOLanguageName)
                    .Zip(Languages, (a, b) => a.Equals(b, StringComparison.OrdinalIgnoreCase))
                    .All(x => x);

            if (isSame)
                return true;

            aggregate.Apply(new OrganizationLocalizationModified(Languages, TimeZone));

            return true;
        }
    }
}
