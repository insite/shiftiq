using System;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Domain.Messages;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Employers.Candidates
{
    public partial class RequestContact : PortalBasePage
    {
        private class DataItem
        {
            public DateTimeOffset? Assigned { get; set; }
            public string Requested => Assigned.HasValue ? TimeZones.FormatDateOnly(Assigned.Value, TimeZones.Pacific) : "-";
            public Guid CandidateContactIdentifier { get; set; }
            public string CandidateFirstName { get; set; }
            public string CandidateLastName { get; set; }
        }

        private Guid CandidateId => Guid.TryParse(Request["candidate"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var candidate = PersonSearch.Select(Organization.Identifier, CandidateId, x => x.User);

                if (candidate == null)
                    HttpResponseHelper.Redirect("/");

                LoadData(candidate);
            }
        }

        private void LoadData(Person candidate)
        {
            var requester = CurrentSessionState.Identity.User;

            CandidateFullName.Text = candidate.User.FullName;
            RequesterName.Text = requester.FullName;
            RequesterEmail.Text = requester.Email;

            var requesterPerson = PersonSearch.Select(CurrentSessionState.Identity.Organization.Identifier, CurrentSessionState.Identity.User.Identifier, x => x.User);
            if (requesterPerson != null)
                LoadPrevData(requesterPerson.EmployerGroupIdentifier);
        }

        private void LoadPrevData(Guid? EmployerIdentifier)
        {
            if (!EmployerIdentifier.HasValue)
                return;

            var data = MembershipSearch.Bind(x => new DataItem
            {
                Assigned = x.Assigned,
                CandidateContactIdentifier = x.UserIdentifier,
                CandidateFirstName = x.User.FirstName,
                CandidateLastName = x.User.LastName,
            },
                x => x.GroupIdentifier == EmployerIdentifier.Value && x.MembershipType == "Jobs Candidate" && x.UserIdentifier == CandidateId
            );

            Repeater.DataSource = data;
            Repeater.DataBind();
            RequestHistory.Visible = data.Length > 0;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            if (CandidateId == Guid.Empty)
            {
                StatusAlert.AddMessage(AlertType.Warning, "Your account is not approved yet.");
                return;
            }

            var organization = CurrentSessionState.Identity.Organization;
            var requester = PersonSearch.Select(organization.Identifier, CurrentSessionState.Identity.User.Identifier, x => x.User);
            var contact = PersonSearch.Select(organization.Identifier, CandidateId, x => x.User);

            if (contact == null)
            {
                StatusAlert.AddMessage(AlertType.Warning, "Record not found or insufficient permissions.");
                return;
            }

            if (!contact.JobsApproved.HasValue)
            {
                StatusAlert.AddMessage(AlertType.Warning, "Candidate is not approved yet.");
                return;
            }

            SendRequest(organization, requester, contact);

            SaveButton.Visible = false;
        }

        private void SendRequest(OrganizationState organization, Person requester, Person contact)
        {
            ServiceLocator.AlertMailer.Send(organization.Identifier, CandidateId, new AlertJobsCandidateContactRequested
            {
                CandidateFirstName = contact.User.FirstName,
                CandidateLastName = contact.User.LastName,
                CompanyName = RequesterOrganization.Text,
                Message = RequesterMessage.Text,
                EmailAddress = RequesterEmail.Text,
                EmployerName = RequesterName.Text,
            });

            if (requester.EmployerGroupIdentifier.HasValue)
                MembershipStore.Save(MembershipFactory.Create(CandidateId, requester.EmployerGroupIdentifier.Value, organization.Identifier, "Jobs Candidate"));

            StatusAlert.AddMessage(AlertType.Success, "Request for Candidate Sent.");
        }
    }
}