using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Organizations;

using Newtonsoft.Json;

using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class OrganizationHelper
    {
        public static bool EnableDivisions(CompanySize companySize)
        {
            return companySize == CompanySize.Large;
        }

        public static IReadOnlyCollection<string> GetStandardOrganizationTags(Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                var tagItems = db.Standards
                    .Where(x => x.OrganizationIdentifier == organization && x.Tags != null)
                    .Select(x => x.Tags)
                    .Distinct()
                    .ToList();

                var tags = new HashSet<string>();

                foreach (var tagItem in tagItems)
                {
                    var collections = JsonConvert.DeserializeObject<List<Tuple<string, List<string>>>>(tagItem);

                    foreach (var collection in collections)
                    {
                        foreach (var collectionTag in collection.Item2)
                            tags.Add(collectionTag);
                    }
                }

                var sortedTags = tags.ToList();
                sortedTags.Sort();

                return sortedTags;
            }
        }

        public static TUserSessionCacheSummary GetRecentSession(Guid userId, string[] includeOrganizations, string[] excludeOrganizations)
        {
            using (var db = new InternalDbContext())
            {
                return db.SessionSummaries
                    .Where(x => x.UserIdentifier == userId && includeOrganizations.Contains(x.OrganizationCode) &&
                                !excludeOrganizations.Contains(x.OrganizationCode))
                    .OrderByDescending(x => x.SessionStarted)
                    .FirstOrDefault();
            }
        }

        public static List<TUserSessionCacheSummary> GetRecentSessions(Guid user, int count)
        {
            using (var db = new InternalDbContext())
            {
                var summaries = db.SessionSummaries
                    .Where(x => x.UserIdentifier == user)
                    .OrderByDescending(x => x.SessionStarted)
                    .Take(count)
                    .ToList();

                var closedAccounts = db.QOrganizations
                    .Where(x => x.AccountStatus == "Closed")
                    .Select(x => x.OrganizationIdentifier)
                    .ToList();

                return summaries
                    .Where(x => !closedAccounts.Any(y => y == x.OrganizationIdentifier))
                    .ToList();
            }
        }

        public static OrganizationState GetUserOrganizationCmds(Guid user, Guid? activeOrganizationIdentifier, OrganizationState organization, out bool isMultiOrganization)
        {
            var identifiers = SelectCmdsOrganizationIdentifiers(user);

            isMultiOrganization = identifiers.Count > 1;

            if (identifiers.Count > 0)
            {
                if (isMultiOrganization)
                {
                    if (activeOrganizationIdentifier.HasValue)
                    {
                        var activeOrganization = OrganizationSearch.Select(activeOrganizationIdentifier.Value);
                        if (activeOrganization != null && identifiers.Any(x => x == activeOrganization.OrganizationIdentifier))
                            organization = activeOrganization;
                    }
                    else if (!identifiers.Any(x => x == organization.OrganizationIdentifier))
                    {
                        // Determine if the user has access to the organization in any one of their latest 5 sessions.
                        // If yes, then use it as the default organization for this new session.

                        var mostRecentSession = GetRecentSessions(user, 5)
                            .FirstOrDefault(session => identifiers.Any(id => id == session.OrganizationIdentifier));

                        if (mostRecentSession != null)
                            organization = OrganizationSearch.Select(mostRecentSession.OrganizationIdentifier);
                    }
                }

                // If the user does not have access to the organization that is now selected as their default, then
                // simply use the first item in the list of organizations to which the user does have access.

                if (identifiers.All(x => x != organization.OrganizationIdentifier))
                    organization = OrganizationSearch.Select(identifiers.First());
            }

            return organization;
        }

        public static OrganizationState GetUserOrganizationCore(Guid user, Guid? activeOrganizationIdentifier, OrganizationState organization, out bool isMultiOrganization)
        {
            var identifiers = SelectCoreOrganizationIdentifiers(user);

            isMultiOrganization = identifiers.Count > 1;

            if (identifiers.Count > 0)
            {
                if (isMultiOrganization)
                {
                    if (activeOrganizationIdentifier.HasValue)
                    {
                        var activeOrganization = OrganizationSearch.Select(activeOrganizationIdentifier.Value);
                        if (activeOrganization != null && identifiers.Any(x => x == activeOrganization.OrganizationIdentifier))
                            organization = activeOrganization;
                    }
                }

                if (identifiers.All(x => x != organization.OrganizationIdentifier))
                    organization = OrganizationSearch.Select(identifiers[0]);
            }

            return organization;
        }

        public static bool IsCmdsOrganization(Guid organization)
        {
            return OrganizationSearch.SelectEntity(organization).ParentOrganizationIdentifier == OrganizationIdentifiers.CMDS;
        }

        public static List<Guid> SelectCmdsOrganizationIdentifiers(Guid user)
        {
            using (var db = new InternalDbContext())
            {
                return db.Persons
                    .Where(x => x.UserIdentifier == user 
                        && x.Organization.ParentOrganizationIdentifier == OrganizationIdentifiers.CMDS
                        && x.Organization.AccountClosed == null
                        )
                    .Select(x => new { x.OrganizationIdentifier, x.Organization.OrganizationCode })
                    .OrderBy(x => x.OrganizationCode)
                    .Select(x => x.OrganizationIdentifier)
                    .Distinct()
                    .ToList();
            }
        }

        public static List<Guid> SelectCoreOrganizationIdentifiers(Guid user)
        {
            using (var db = new InternalDbContext())
            {
                return db.Persons
                    .Where(x => x.UserIdentifier == user)
                    .Select(x => x.OrganizationIdentifier)
                    .ToList();
            }
        }

        public static OrganizationList SelectOrganizationsAccessibleToUser(Guid user)
        {
            var list = new OrganizationList();
            SelectUserAccessibleOrganizations(user).ForEach(x => { list.Add(OrganizationAdapter.CreatePacket(x)); });
            return list;
        }

        private static List<VOrganization> SelectUserAccessibleOrganizations(Guid user)
        {
            using (var db = new InternalDbContext())
            {
                return db.Persons
                    .Where(x => x.UserIdentifier == user && (x.IsAdministrator || x.IsLearner))
                    .Select(x => x.Organization)
                    .OrderBy(x => x.CompanyName)
                    .ToList();
            }
        }
    }
}
