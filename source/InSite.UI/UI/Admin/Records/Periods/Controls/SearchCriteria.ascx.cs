using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Records.Periods.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QPeriodFilter>
    {
        public override QPeriodFilter Filter
        {
            get
            {
                var filter = new QPeriodFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    PeriodName = PeriodName.Text,
                    PeriodSince = PeriodSince.Value,
                    PeriodBefore = PeriodBefore.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                PeriodName.Text = value.PeriodName;
                PeriodSince.Value = value.PeriodSince.HasValue ? value.PeriodSince.Value.UtcDateTime : (DateTime?)null;
                PeriodBefore.Value = value.PeriodBefore.HasValue ? value.PeriodBefore.Value.UtcDateTime : (DateTime?)null;
            }
        }

        public override void Clear()
        {
            PeriodName.Text = null;
            PeriodSince.Value = null;
            PeriodBefore.Value = null;
        }
    }
}