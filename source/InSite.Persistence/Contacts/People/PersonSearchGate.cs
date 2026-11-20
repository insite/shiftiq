using System;

namespace InSite.Persistence
{
    /// <summary>
    /// Resolves the permission-driven scope and feature visibility for the consolidated person
    /// search page, which serves both the contact directory (Portal/Contacts) and user account
    /// administration (design/users). The logic has no Web Forms dependency so the page and its
    /// controls stay lightweight and the rules can be unit tested directly.
    /// </summary>
    public static class PersonSearchGate
    {
        /// <summary>Scope value that restricts results to the caller's upstream contact subtree.</summary>
        public const string ScopeMyContacts = "MyContacts";

        /// <summary>Scope value that returns the full organization.</summary>
        public const string ScopeAllUsers = "AllUsers";

        /// <summary>
        /// The user-administration resource, spelled exactly as settings.TAction stores its URL.
        /// The action lookup behind Identity.IsGranted(string) is case-sensitive, and a URL that
        /// resolves to no action is granted to everyone, so "design/users" silently passes for
        /// every caller. Pass this constant rather than a literal.
        /// </summary>
        public const string UserAdministrationResource = "Design/Users";

        /// <summary>
        /// The scope the page starts on. An explicit request from the URL is honoured when the
        /// caller may have it. With no request, a caller who has contacts of their own starts on
        /// them, and a caller with none starts on the full organization rather than an empty list.
        /// Either way the result passes through <see cref="ConfineScope"/>, so the default never
        /// widens the search for a caller who may not see the full organization.
        /// </summary>
        public static string ResolveScope(bool allowAllUsers, bool hasUsers, bool hasOwnContacts, string requestedScope)
        {
            var isMyContacts = string.Equals(requestedScope, ScopeMyContacts, StringComparison.OrdinalIgnoreCase);
            var isAllUsers = string.Equals(requestedScope, ScopeAllUsers, StringComparison.OrdinalIgnoreCase);

            string scope;

            if (isMyContacts)
                scope = ScopeMyContacts;
            else if (isAllUsers)
                scope = ScopeAllUsers;
            else
                scope = hasOwnContacts ? ScopeMyContacts : ScopeAllUsers;

            return ConfineScope(allowAllUsers, hasUsers, scope);
        }

        /// <summary>
        /// The scope a search is allowed to run under, given the scope selected. The
        /// full-organization scope exists only where allowAllUsers is set (the E03 partition);
        /// everywhere else every caller is confined to their own contacts, because a role holding
        /// the user-administration permission there (BCWS grants it to 1,210 supervisors) must not
        /// expose the whole organization. Where the scope is allowed, a caller without the
        /// user-administration permission is still confined to their own contacts. The page calls
        /// this on every search rather than trusting the selector's posted value.
        /// </summary>
        public static string ConfineScope(bool allowAllUsers, bool hasUsers, string selectedScope)
        {
            // Outside a partition that allows the full-organization scope, no permission widens the
            // search beyond the caller's own contacts.
            if (!allowAllUsers)
                return ScopeMyContacts;

            // A caller without the user-administration permission can only ever see their own
            // contacts; they cannot widen the scope to the whole organization.
            if (!hasUsers)
                return ScopeMyContacts;

            // A user-administrator gets the full organization only when it is selected.
            if (string.Equals(selectedScope, ScopeAllUsers, StringComparison.OrdinalIgnoreCase))
                return ScopeAllUsers;

            return ScopeMyContacts;
        }

        /// <summary>
        /// The scope selector is offered to any user-administrator in a partition that allows the
        /// full-organization scope, since only they can switch between their own contacts and the
        /// full organization. Any other caller is confined to their own contacts and has no choice
        /// to make, so the selector stays hidden. The hasContacts argument is unused.
        /// </summary>
        public static bool ShowScopeSelector(bool allowAllUsers, bool hasContacts, bool hasUsers) => allowAllUsers && hasUsers;

        /// <summary>
        /// The user-administration features - the name-match, session-count and approval filters,
        /// the row action column, and the downloads tab - belong to the design/users permission.
        /// </summary>
        public static bool ShowUserFeatures(bool hasUsers) => hasUsers;

        /// <summary>The page is reachable only by a caller who holds at least one of the two permissions.</summary>
        public static bool IsAccessAllowed(bool hasContacts, bool hasUsers) => hasContacts || hasUsers;
    }
}
