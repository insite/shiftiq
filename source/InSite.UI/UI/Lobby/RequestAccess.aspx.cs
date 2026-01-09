using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Lobby
{
    public partial class RequestAccess : Layout.Lobby.LobbyBasePage
    {
        #region Classes

        [Serializable]
        private sealed class Ticket
        {
            public string ID { get; }
            public string Email { get; }

            public Ticket(string id, string name)
            {
                ID = id;
                Email = name;
            }
        }

        #endregion

        #region Properties

        private static LinkedList<Ticket> Tickets => (LinkedList<Ticket>)(HttpContext.Current.Session[_ticketsKey]
            ?? (HttpContext.Current.Session[_ticketsKey] = new LinkedList<Ticket>()));

        private Ticket CurrentTicket
        {
            get => (Ticket)ViewState[nameof(CurrentTicket)];
            set => ViewState[nameof(CurrentTicket)] = value;
        }

        #endregion

        #region Fields

        private static readonly string _ticketsKey = typeof(Lobby.RequestAccess).FullName + "." + nameof(Tickets);
        private static readonly RandomStringGenerator _ticketIdGenerator = new RandomStringGenerator(RandomStringType.AlphanumericCaseSensitive, 4);

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RequestButton.Click += RequestButton_Click;
            CloseButton.Click += CloseButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Title = LabelHelper.GetTranslation(ActionModel.ActionName);

            RequestOrganizationSelector.Filter.IsClosed = false;

            if (CurrentSessionState.Identity.IsAuthenticated)
            {
                RequestOrganizationField.Visible = true;
                RequestUserName.Text = Identity.User.Email;
                RequestUserName.Enabled = false;

                ScreenViews.SetActiveView(ViewRequest);
            }
            else
            {
                RequestOrganizationField.Visible = false;

                var ticketId = Request.QueryString["ticket"];
                if (ticketId.IsEmpty())
                    RedirectBack();

                CurrentTicket = Tickets.FirstOrDefault(x => x.ID == ticketId);
                if (CurrentTicket == null)
                    RedirectBack();

                RequestUserName.Text = CurrentTicket.Email;

                ScreenViews.SetActiveView(ViewRequest);

                var lockedUntil = AccountHelper.SignInLockedUntil;
                if (lockedUntil.HasValue)
                    OnBruteForceDetected(lockedUntil.Value);
            }
        }

        #endregion

        #region Event handlers

        private void RequestButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Guid organizationId;
            User user = null;

            if (CurrentTicket != null)
            {
                var isValid = false;
                user = UserSearch.SelectByEmail(CurrentTicket.Email);

                if (user == null)
                    ScreenStatus.AddMessage(AlertType.Error, LabelHelper.GetTranslation("The user account is not found."));
                else if (string.IsNullOrEmpty(user.Email))
                    ScreenStatus.AddMessage(AlertType.Error, LabelHelper.GetTranslation("The user account has no email address."));
                else if (!ServiceLocator.PersonSearch.IsPersonExist(user.UserIdentifier, Organization.Key))
                    ScreenStatus.AddMessage(AlertType.Error, LabelHelper.GetTranslation("The user account is not authorized to access this organization."));
                else
                    isValid = true;

                if (!isValid)
                    return;

                organizationId = Organization.Identifier;
            }
            else
                organizationId = RequestOrganizationSelector.Value.Value;

            if (user != null)
            {
                var organization = OrganizationSearch.Select(organizationId);
                ServiceLocator.AlertMailer.Send(organizationId, user.UserIdentifier, new AlertApplicationAccessRequested
                {
                    AppUrl = ServiceLocator.Urls.GetApplicationUrl(organization.Code),
                    Organization = organization.LegalName,
                    SourceUrl = Request.Url.Host + Request.RawUrl,
                    UserEmail = user.Email,
                    UserIdentifier = user.UserIdentifier,
                    UserName = user.FullName
                });
            }

            ScreenStatus.AddMessage(
                AlertType.Success,
                "far fa-check fs-xl", LabelHelper.GetTranslation("Your access request has been received and is pending approval."));

            RequestButton.Visible = false;

            Tickets.Clear();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Tickets.Clear();
            RedirectBack();
        }

        private void OnBruteForceDetected(DateTime lockedUntil)
        {
            RequestButton.Visible = false;

            ScreenStatus.AddMessage(AlertType.Error, AccountHelper.GetBruteForceError("Submit", lockedUntil));
        }

        #endregion

        #region Method (helpers)

        private static void RedirectBack()
        {
            var url = new ReturnUrl().GetReturnUrl()
                ?? "/ui/lobby/signin";

            HttpResponseHelper.Redirect(url);
        }

        public static string CreateTicket(string login)
        {
            string id;
            do { id = _ticketIdGenerator.Next(); } while (Tickets.Any(x => x.ID == id));

            Tickets.AddLast(new Ticket(id, login));

            if (Tickets.Count > 5)
                Tickets.RemoveFirst();

            return id;
        }

        #endregion
    }
}