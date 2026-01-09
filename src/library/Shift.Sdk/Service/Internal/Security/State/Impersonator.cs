using System;

using InSite.Domain.Organizations;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class Impersonator
    {
        /// <summary>
        /// If the current user is impersonating another user then this is a reference back to the
        /// impersonating administrator's account.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// If the current user is impersonating another user then this is a reference back to the
        /// organization that was selected when the administrator started impersonating.
        /// </summary>
        public OrganizationState Organization { get; set; }

        /// <summary>
        /// If the current user is impersonating another user then this is a reference back to the
        /// list of organizations accessible to the administrator who is impersonating another user.
        /// </summary>
        public OrganizationList Organizations { get; set; }
    }
}
