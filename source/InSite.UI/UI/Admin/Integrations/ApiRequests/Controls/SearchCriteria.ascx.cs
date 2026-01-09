using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Integrations.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<ApiRequestFilter>
    {
        public override ApiRequestFilter Filter
        {
            get
            {
                var filter = new ApiRequestFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    RequestStartedSince = StartedSince.Value,
                    RequestStartedBefore = StartedBefore.Value,
                    RequestUri = RequestUri.Text,
                    RequestData = RequestData.Text,
                    RequestIsIncoming = RequestDirection.ValueAsBoolean
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                StartedSince.Value = Filter.RequestStartedSince;
                StartedBefore.Value = Filter.RequestStartedBefore;
                RequestData.Text = Filter.RequestData;
                RequestUri.Text = Filter.RequestUri;
                RequestDirection.ValueAsBoolean = Filter.RequestIsIncoming;
            }
        }

        public override void Clear()
        {
            StartedSince.Value = DateTimeOffset.Now.AddDays(-28); // since 4 weeks ago
            StartedBefore.Value = null;
            RequestUri.Text = null;
            RequestData.Text = null;
            RequestDirection.ClearSelection();
        }
    }
}