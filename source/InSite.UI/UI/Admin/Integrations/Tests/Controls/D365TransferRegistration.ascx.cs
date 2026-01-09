using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.UI.Admin.Integrations.Tests.Utilities;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Integrations.Tests.Controls
{
    public partial class D365TransferRegistration : BaseUserControl, ID365Method
    {
        private Dictionary<Guid, Guid> CandidateRegistrationMapping
        {
            get => (Dictionary<Guid, Guid>)ViewState[nameof(CandidateRegistrationMapping)];
            set => ViewState[nameof(CandidateRegistrationMapping)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EventIdentifier.AutoPostBack = true;
            EventIdentifier.ValueChanged += (x, y) => OnEventChanged();

            CandidateIdentifier.AutoPostBack = true;
            CandidateIdentifier.ValueChanged += CandidateIdentifier_ValueChanged;
        }

        public void InitMethod()
        {
            EventIdentifier.Filter.EventType = "Exam";

            OnEventChanged();

            EventVenue.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            EventVenue.Filter.GroupType = GroupTypes.Venue;

            Accommodations.InitControl();
        }

        public string GetUrl()
        {
            var regId = GetRedistrationId();

            return regId.HasValue ? $"/api/registrations/{regId.Value}/commands/transfer-lax" : null;
        }

        private Guid? GetRedistrationId()
        {
            if (!CandidateIdentifier.Value.HasValue)
                return null;

            return CandidateRegistrationMapping.GetOrDefault(CandidateIdentifier.Value.Value).NullIfEmpty();
        }

        public string GetBody(Formatting jsonFormatting)
        {
            var data = new TransferRegistrationLax
            {
                EventVenue = EventVenue.Value ?? Guid.Empty,
                EventStart = EventStart.Value ?? DateTimeOffset.MinValue,
                EventExamType = EventExamType.Value,
                Learner = LearnerIdentifier.Value ?? Guid.Empty,
                Reason = Reason.Text,
                Accommodations = Accommodations.GetItems().ToArray()
            };

            return JsonConvert.SerializeObject(data, jsonFormatting);
        }

        private void OnEventChanged()
        {
            var eventId = EventIdentifier.Value;

            CandidateIdentifier.Value = null;
            CandidateIdentifier.Enabled = false;

            CandidateRegistrationMapping = null;

            if (!eventId.HasValue)
                return;

            var regs = ServiceLocator.RegistrationSearch.GetRegistrations(
                new InSite.Application.Registrations.Read.QRegistrationFilter
                {
                    EventIdentifier = eventId,
                });

            if (regs.IsEmpty())
                return;

            CandidateRegistrationMapping = regs.GroupBy(x => x.CandidateIdentifier).ToDictionary(x => x.Key, x => x.First().RegistrationIdentifier);

            CandidateIdentifier.Enabled = true;
            CandidateIdentifier.Filter.IncludeUserIdentifiers = CandidateRegistrationMapping.Keys.ToArray();
        }

        private void CandidateIdentifier_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            if (CandidateIdentifier.HasValue)
                LearnerIdentifier.Value = CandidateIdentifier.Value;
        }

        public D365Response SendRequest()
        {
            return D365Response.Get(GetUrl(), "POST", "application/json", GetBody(Formatting.None));
        }
    }
}