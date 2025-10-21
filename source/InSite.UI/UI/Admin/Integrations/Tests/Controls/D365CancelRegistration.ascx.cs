using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.UI.Admin.Integrations.Tests.Utilities;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Integrations.Tests.Controls
{
    public partial class D365CancelRegistration : BaseUserControl, ID365Method
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
        }

        public void InitMethod()
        {
            EventIdentifier.Filter.EventType = "Exam";

            OnEventChanged();

            Reason.Text = "Cancel request";
        }

        public string GetUrl()
        {
            var regId = GetRedistrationId();

            return regId.HasValue ? $"/api/registrations/{regId.Value}/commands/cancel" : null;
        }

        private Guid? GetRedistrationId()
        {
            if (!CandidateIdentifier.Value.HasValue)
                return null;

            var registrationId = CandidateRegistrationMapping.GetOrDefault(CandidateIdentifier.Value.Value);
            return registrationId == Guid.Empty ? (Guid?)null : registrationId;
        }

        public string GetBody(Formatting jsonFormatting)
        {
            var data = new CancelRegistration
            {
                Reason = Reason.Text
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

        public D365Response SendRequest()
        {
            return D365Response.Get(GetUrl(), "POST", "application/json", GetBody(Formatting.None));
        }
    }
}