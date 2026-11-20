using System;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Portal.Contacts.People.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<PersonFilter>
    {
        public override PersonFilter Filter
        {
            get
            {
                // Resolve the scope from the partition and the caller's permissions on every search
                // instead of trusting the selector's value, so a caller who may not see the full
                // organization stays confined to their own contacts even if the hidden selector's
                // value is tampered with.
                var allowAllUsers = ServiceLocator.Partition.IsE03();
                var hasUsers = Identity.IsGranted(PersonSearchGate.UserAdministrationResource);
                var scope = PersonSearchGate.ConfineScope(allowAllUsers, hasUsers, Scope.Value);

                var filter = new PersonFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    EmailContains = Email.Text,
                    FullName = FullName.Text,
                    CodeContains = PersonCode.Text,
                    UtcCreatedSince = CreatedSince.Value?.UtcDateTime,
                    UtcCreatedBefore = CreatedBefore.Value?.UtcDateTime,
                    LastAuthenticatedSince = LastAuthenticatedSince.Value?.UtcDateTime,
                    LastAuthenticatedBefore = LastAuthenticatedBefore.Value?.UtcDateTime,
                    NameFilterType = MatchNamesWith.Visible ? MatchNamesWith.Value : "Exact",
                    SessionCount = SessionCount.Visible ? SessionCount.ValueAsInt : null,
                    IsApproved = (ApprovalFilter.Visible && ApprovalFilter.Value == "Approved") ? true : (bool?)null,
                    UpstreamUserIdentifiers = scope == PersonSearchGate.ScopeMyContacts
                        ? new Guid[] { User.Identifier }
                        : null
                };
                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Email.Text = value.EmailContains;
                FullName.Text = value.FullName;
                PersonCode.Text = value.CodeContains;
                CreatedSince.Value = value.UtcCreatedSince;
                CreatedBefore.Value = value.UtcCreatedBefore;
                LastAuthenticatedSince.Value = value.LastAuthenticatedSince;
                LastAuthenticatedBefore.Value = value.LastAuthenticatedBefore;

                if (MatchNamesWith.Visible)
                    MatchNamesWith.Value = value.NameFilterType;

                if (SessionCount.Visible)
                    SessionCount.ValueAsInt = value.SessionCount;

                if (ApprovalFilter.Visible)
                    ApprovalFilter.Value = value.IsApproved == true ? "Approved" : "All";

                // A saved filter carries its scope as the upstream-user restriction, so restore the
                // selector from it rather than keeping the route's default. The getter still confines
                // the scope on every search.
                if (Scope.Visible)
                    Scope.Value = value.UpstreamUserIdentifiers == null
                        ? PersonSearchGate.ScopeAllUsers
                        : PersonSearchGate.ScopeMyContacts;
            }
        }

        /// <summary>
        /// Sets the scope and restricts the user-administration filters for the current request.
        /// Called by the page once, on the initial (non-postback) load, with the scope requested
        /// on the query string. Both permission decisions persist in ViewState across postbacks.
        /// </summary>
        public void ApplyRouteScope(string requestedScope)
        {
            var allowAllUsers = ServiceLocator.Partition.IsE03();
            var hasContacts = Identity.IsGranted("Portal/Contacts");
            var hasUsers = Identity.IsGranted(PersonSearchGate.UserAdministrationResource);
            var hasOwnContacts = HasOwnContacts();

            var showUserFilters = PersonSearchGate.ShowUserFeatures(hasUsers);

            MatchNamesWith.Visible = showUserFilters;
            SessionCount.Visible = showUserFilters;
            ApprovalFilter.Visible = showUserFilters;

            Scope.Visible = PersonSearchGate.ShowScopeSelector(allowAllUsers, hasContacts, hasUsers);
            Scope.Value = PersonSearchGate.ResolveScope(allowAllUsers, hasUsers, hasOwnContacts, requestedScope);
        }

        /// <summary>
        /// Whether the caller has at least one contact in this organization, using the same
        /// upstream-connection rule the MyContacts scope searches with.
        /// </summary>
        private bool HasOwnContacts()
        {
            var filter = new PersonFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                UpstreamUserIdentifiers = new Guid[] { User.Identifier }
            };

            return PersonCriteria.Exists(filter);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PersonCode.EmptyMessage = LabelHelper.GetLabelContentText("Person Code");

            if (MatchNamesWith.Visible)
                Translate(MatchNamesWith);
        }

        public override void Clear()
        {
            FullName.Text = null;
            Email.Text = null;
            PersonCode.Text = null;
            CreatedSince.Value = null;
            CreatedBefore.Value = null;
            LastAuthenticatedSince.Value = null;
            LastAuthenticatedBefore.Value = null;

            if (MatchNamesWith.Visible)
                MatchNamesWith.ClearSelection();

            if (SessionCount.Visible)
                SessionCount.ValueAsInt = null;

            if (ApprovalFilter.Visible)
                ApprovalFilter.Value = "Approved";
        }
    }
}
