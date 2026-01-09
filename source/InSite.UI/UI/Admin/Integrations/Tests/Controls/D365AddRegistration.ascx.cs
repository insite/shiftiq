using System;

using InSite.Common.Web.UI;
using InSite.UI.Admin.Integrations.Tests.Utilities;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Integrations.Tests.Controls
{
    public partial class D365AddRegistration : BaseUserControl, ID365Method
    {
        public void InitMethod()
        {
            RegistrationId.Text = UniqueIdentifier.Create().ToString();

            EventVenue.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            EventVenue.Filter.GroupType = GroupTypes.Venue;

            EventBillingCode.Settings.CollectionName = CollectionName.Activities_Exams_Billing_Type;
            EventBillingCode.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;
            EventBillingCode.RefreshData();

            LearnerRegistrantType.Settings.CollectionName = CollectionName.Registrations_Exams_Candidate_Type;
            LearnerRegistrantType.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;
            LearnerRegistrantType.RefreshData();

            Accommodations.InitControl();
        }

        public string GetUrl()
        {
            return $"/api/registrations/{Guid.Parse(RegistrationId.Text)}/commands/register-lax";
        }

        public string GetBody(Formatting jsonFormatting)
        {
            var data = new AddRegistrationLax
            {
                EventVenue = EventVenue.Value ?? Guid.Empty,
                EventStart = EventStart.Value ?? DateTimeOffset.MinValue,
                EventExamType = EventExamType.Value,
                EventExamFormat = EventExamFormat.Value,
                EventBillingCode = EventBillingCode.Value,
                Learner = LearnerIdentifier.Value ?? Guid.Empty,
                LearnerRegistrantType = LearnerRegistrantType.Value,
                Assessment = Assessment.Text,
                Accommodations = Accommodations.GetItems().ToArray()
            };

            return JsonConvert.SerializeObject(data, jsonFormatting);
        }

        public D365Response SendRequest()
        {
            return D365Response.Get(GetUrl(), "POST", "application/json", GetBody(Formatting.None));
        }
    }
}