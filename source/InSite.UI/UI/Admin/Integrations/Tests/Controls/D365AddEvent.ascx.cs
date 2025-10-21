using System;

using InSite.Common.Web.UI;
using InSite.UI.Admin.Integrations.Tests.Utilities;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Integrations.Tests.Controls
{
    public partial class D365AddEvent : BaseUserControl, ID365Method
    {
        public void InitMethod()
        {
            EventId.Text = UniqueIdentifier.Create().ToString();

            EventVenue.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            EventVenue.Filter.GroupType = GroupTypes.Venue;

            EventBillingCode.Settings.CollectionName = CollectionName.Activities_Exams_Billing_Type;
            EventBillingCode.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;
            EventBillingCode.RefreshData();
        }

        public string GetUrl()
        {
            return $"/api/events/{Guid.Parse(EventId.Text)}";
        }

        public string GetBody(Formatting jsonFormatting)
        {
            var data = new Shift.Sdk.UI.AddEvent
            {
                EventVenue = EventVenue.Value ?? Guid.Empty,
                EventStart = EventStart.Value ?? DateTimeOffset.MinValue,
                EventExamType = EventExamType.Value,
                EventExamFormat = EventExamFormat.Value,
                EventBillingCode = EventBillingCode.Value,
                RegistrationLimit = RegistrationLimit.ValueAsInt
            };

            return JsonConvert.SerializeObject(data, jsonFormatting);
        }

        public D365Response SendRequest()
        {
            return D365Response.Get(GetUrl(), "POST", "application/json", GetBody(Formatting.None));
        }
    }
}