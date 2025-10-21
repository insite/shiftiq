using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Portal.Contacts.People.Controls
{
    public partial class PersonRegistrations : BaseUserControl
    {
        public int LoadData(Guid organizationId, Guid userId)
        {
            var filter = new QRegistrationFilter
            {
                CandidateIdentifier = userId,
                OrganizationIdentifier = organizationId
            };

            var registrations = ServiceLocator.RegistrationSearch
                .GetRegistrations(filter, x => x.Event)
                .Select(x => new
                {
                    EventTitle = x.Event.EventTitle,
                    RegistrationRequestedOn = x.RegistrationRequestedOn,
                    ApprovalStatus = x.ApprovalStatus,
                    RegistrationFee = x.RegistrationFee,
                    AttendanceStatus = x.AttendanceStatus,
                    Score = x.Score,
                    RegistrationComment =x.RegistrationComment
                })
                .OrderByDescending(x => x.RegistrationRequestedOn)
                .ThenBy(x => x.EventTitle)
                .ToList();

            Registrations.DataSource = registrations;
            Registrations.DataBind();
            Registrations.Visible = registrations.Count > 0;

            NoRegistrations.Visible = registrations.Count == 0;

            return registrations.Count;
        }

        protected string LocalizeDate(object date)
        {
            return date != null
                ? TimeZones.FormatDateOnly((DateTimeOffset)date, User.TimeZone)
                : string.Empty;
        }
    }
}