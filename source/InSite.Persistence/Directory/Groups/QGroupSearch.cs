using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class QGroupSearch : IGroupSearch
    {
        public QGroup GetGroup(Guid groupIdentifier, params Expression<Func<QGroup, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QGroups
                    .Where(x => x.GroupIdentifier == groupIdentifier)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public bool GroupExists(QGroupFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQueryByQGroupFilter(filter, db).Any();
            }
        }

        public int CountGroups(QGroupFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQueryByQGroupFilter(filter, db).Count();
            }
        }

        public List<QGroup> GetGroups(QGroupFilter filter, params Expression<Func<QGroup, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQueryByQGroupFilter(filter, db).ApplyIncludes(includes);

                query = filter.OrderBy.HasValue()
                    ? query.OrderBy(filter.OrderBy)
                    : query.OrderBy(x => x.GroupName);

                return query.ApplyPaging(filter).ToList();
            }
        }

        public List<GroupSearchResult> SearchGroups(QGroupFilter filter)
        {
            var orderBy = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "GroupName";

            using (var db = CreateContext())
            {
                var query = CreateQueryByQGroupFilter(filter, db);

                if (orderBy.Equals("GroupSize"))
                    query = query.OrderBy(x => x.VMemberships.Count);
                else
                    query = query.OrderBy(orderBy);

                var list = query.ApplyPaging(filter);

                var statuses = filter.Statuses.IsEmpty() ? new[] { Guid.Empty } : filter.Statuses;

                return list
                    .Select(g => new GroupSearchResult
                    {
                        GroupIdentifier = g.GroupIdentifier,
                        GroupName = g.GroupName,
                        GroupDescription = g.GroupDescription,
                        GroupType = g.GroupType,
                        GroupLabel = g.GroupLabel,
                        GroupCode = g.GroupCode,
                        GroupCategory = g.GroupCategory,
                        GroupStatus = db.TCollectionItems.FirstOrDefault(i => i.ItemIdentifier == g.GroupStatusItemIdentifier).ItemName,
                        GroupOffice = g.GroupOffice,
                        GroupPhone = g.GroupPhone,
                        GroupRegion = g.GroupRegion,
                        GroupCapacity = g.GroupCapacity,
                        ShippingAddress = g.Addresses.Where(y => y.AddressType == "Shipping").FirstOrDefault(),
                        BillingAddress = g.Addresses.Where(y => y.AddressType == "Billing").FirstOrDefault(),
                        PhysicalAddress = g.Addresses.Where(y => y.AddressType == "Physical").FirstOrDefault(),
                        NumberOfEmployees = g.GroupSize,
                        GroupSize = g.VMemberships.Count,
                        GroupExpiry = g.GroupExpiry,
                        ChildCount = g.Children.Count + g.ConnectionChildren.Count,
                        SurveyFormIdentifier = g.SurveyFormIdentifier,
                        SurveyFormName = g.SurveyForm.SurveyFormName,
                        MembershipProductIdentifier = g.MembershipProductIdentifier,
                        MembershipProductName = g.MembershipProduct.ProductName,
                        MembershipStatusSize = g.VMemberships
                            .Where(m =>
                                db.Persons.Any(z =>
                                    z.UserIdentifier == m.UserIdentifier
                                    && z.OrganizationIdentifier == g.OrganizationIdentifier
                                    && z.MembershipStatusItemIdentifier != null
                                    && statuses.Contains(z.MembershipStatusItemIdentifier.Value)
                                )
                            ).Count(),
                        HierarchyParent = new GroupSearchResult.ParentInfo
                        {
                            GroupIdentifier = g.Parent.GroupIdentifier,
                            GroupName = g.Parent.GroupName
                        },
                        FunctionalParents = g.ConnectionParents.Select(y => new GroupSearchResult.ParentInfo
                        {
                            GroupIdentifier = y.ParentGroup.GroupIdentifier,
                            GroupName = y.ParentGroup.GroupName
                        }).OrderBy(y => y.GroupName)
                    })
                    .ToList();
            }
        }

        public List<GroupExportResult> ExportGroups(QGroupFilter filter, bool empty)
        {
            using (var db = CreateContext())
            {
                var query = !empty
                    ? CreateQueryByQGroupFilter(filter, db)
                    : db.QGroups.Where(x => 1 == 0);

                query = query.OrderBy(x => x.GroupName);

                var statuses = filter.Statuses.IsEmpty() ? new[] { Guid.Empty } : filter.Statuses;

                return query
                    .Select(x => new GroupExportResult
                    {
                        Group = x,
                        GroupSize = x.VMemberships.Count,
                        Subgroups = x.Children.Count + x.ConnectionChildren.Count,
                        ShippingAddress = x.Addresses.Where(y => y.AddressType == "Shipping").FirstOrDefault(),
                        BillingAddress = x.Addresses.Where(y => y.AddressType == "Billing").FirstOrDefault(),
                        PhysicalAddress = x.Addresses.Where(y => y.AddressType == "Physical").FirstOrDefault(),
                        Parent = x.Parent,
                        Ancestors = x.ConnectionParents,
                        MembershipStatusSize = x.VMemberships
                            .Where(m =>
                                db.Persons.Any(z =>
                                    z.UserIdentifier == m.UserIdentifier
                                    && z.OrganizationIdentifier == x.OrganizationIdentifier
                                    && z.MembershipStatusItemIdentifier != null
                                    && statuses.Contains(z.MembershipStatusItemIdentifier.Value)
                                )
                            ).Count(),
                        SurveyFormName = x.SurveyForm.SurveyFormName,
                        MembershipProductName = x.MembershipProduct.ProductName
                    })
                    .ToList();
            }
        }

        public List<GroupDetail> SearchGroupDetails(QGroupFilter filter)
        {
            var orderBy = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "GroupName";

            using (var db = CreateContext())
            {
                var query = CreateQueryByQGroupFilter(filter, db);

                query = query
                    .OrderBy(orderBy)
                    .ApplyPaging(filter);

                return query
                    .Select(x => new GroupDetail
                    {
                        GroupIdentifier = x.GroupIdentifier,
                        GroupName = x.GroupName,
                        GroupCode = x.GroupCode,
                        MembershipCount = x.VMemberships.Count
                    })
                    .ToList();
            }
        }

        private static IQueryable<QGroup> CreateQueryByQGroupFilter(QGroupFilter filter, InternalDbContext db)
        {
            var query = db.QGroups.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.GroupIdentifier.HasValue)
                query = query.Where(x => x.GroupIdentifier == filter.GroupIdentifier);

            if (filter.ParentGroupIdentifier.HasValue)
                query = query.Where(x => x.ParentGroupIdentifier == filter.ParentGroupIdentifier);

            if (filter.ConnectParentGroupIdentifier.HasValue)
            {
                query = query.Join(db.QGroupConnections.Where(x => x.ParentGroupIdentifier == filter.ConnectParentGroupIdentifier),
                        a => a.GroupIdentifier,
                        b => b.ChildGroupIdentifier,
                        (a, b) => a
                    );
            }

            if (filter.GroupType.HasValue())
                query = query.Where(x => x.GroupType == filter.GroupType);

            if (filter.GroupName.HasValue())
                query = query.Where(x => x.GroupName == filter.GroupName);

            if (filter.GroupNameLike.HasValue())
                query = query.Where(x => x.GroupName.Contains(filter.GroupNameLike));

            if (filter.GroupCode.HasValue())
                query = query.Where(x => x.GroupCode == filter.GroupCode);

            if (filter.GroupCategory.HasValue())
                query = query.Where(x => x.GroupCategory.Contains(filter.GroupCategory));

            if (filter.GroupLabel.HasValue())
                query = query.Where(x => x.GroupLabel == filter.GroupLabel);

            if (filter.GroupStatusIdentifier.HasValue)
                query = query.Where(x => x.GroupStatusItemIdentifier == filter.GroupStatusIdentifier);

            if (filter.GroupRegion.HasValue())
                query = query.Where(x => x.GroupRegion == filter.GroupRegion);

            if (filter.Address.HasValue())
            {
                query = query.Where(x =>
                    x.Addresses.Any(y => y.City.Contains(filter.Address)
                        || y.Province.Contains(filter.Address)
                        || y.Country.Contains(filter.Address)
                        || y.PostalCode.Contains(filter.Address)
                        || y.Street1.Contains(filter.Address)
                    )
                );
            }

            if (filter.AnyParentGroupIdentifier.HasValue)
                query = query.Where(x => x.ParentGroupIdentifier == filter.AnyParentGroupIdentifier.Value || x.ConnectionParents.Any(y => y.ParentGroupIdentifier == filter.AnyParentGroupIdentifier.Value));

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.VMemberships.FirstOrDefault(y => y.UserIdentifier == filter.UserIdentifier) != null);

            if (filter.MembershipUserIdentifier.HasValue)
                query = query.Where(x => x.VMemberships.Any(y => y.UserIdentifier == filter.MembershipUserIdentifier));

            if (filter.UtcCreatedSince.HasValue)
                query = query.Where(x => x.GroupCreated >= filter.UtcCreatedSince.Value);

            if (filter.UtcCreatedBefore.HasValue)
                query = query.Where(x => x.GroupCreated < filter.UtcCreatedBefore.Value);

            if (filter.GroupExpirySince.HasValue)
                query = query.Where(x => x.GroupExpiry >= filter.GroupExpirySince.Value);

            if (filter.GroupExpiryBefore.HasValue)
                query = query.Where(x => x.GroupExpiry < filter.GroupExpiryBefore.Value);

            if (filter.ExcludeContainerIdentifier.HasValue)
                query = query.Where(x => !db.TGroupPermissions.Any(y => y.ObjectIdentifier == filter.ExcludeContainerIdentifier && y.GroupIdentifier == x.GroupIdentifier));

            if (filter.Statuses.IsNotEmpty())
            {
                query = query.Where(x =>
                    x.VMemberships.Any(y =>
                        db.Persons.Any(z =>
                            z.UserIdentifier == y.UserIdentifier
                            && z.OrganizationIdentifier == x.OrganizationIdentifier
                            && z.MembershipStatusItemIdentifier != null
                            && filter.Statuses.Contains(z.MembershipStatusItemIdentifier.Value)
                        )
                    )
                );
            }

            if (filter.Cities.IsNotEmpty() || filter.Provinces.IsNotEmpty() || filter.Country.IsNotEmpty())
            {
                var addressTypes = filter.AddressTypes != null && filter.AddressTypes.Length > 0
                    ? filter.AddressTypes
                    : new[] { AddressType.Billing.ToString(), AddressType.Physical.ToString(), AddressType.Shipping.ToString() };

                var addressQuery = db.QGroupAddresses.Where(x => addressTypes.Contains(x.AddressType));

                if (filter.Cities.IsNotEmpty())
                    addressQuery = addressQuery.Where(x => filter.Cities.Contains(x.City));

                if (filter.Provinces.IsNotEmpty())
                    addressQuery = addressQuery.Where(x => filter.Provinces.Contains(x.Province));

                if (filter.Country.IsNotEmpty())
                    addressQuery = addressQuery.Where(x => filter.Country == x.Country);

                query = query.Where(x => addressQuery.Any(y => y.GroupIdentifier == x.GroupIdentifier));
            }

            if (filter.OrganizationIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.OrganizationIdentifiers.Contains(x.OrganizationIdentifier));

            if (filter.Keyword.HasValue())
                query = query.Where(x => x.GroupName.Contains(filter.Keyword) || x.GroupCode.Contains(filter.Keyword));

            if (filter.GroupCodes.IsNotEmpty())
                query = query.Where(x => filter.GroupCodes.Contains(x.GroupCode));

            if (filter.AllowSelfSubscription.HasValue)
                query = query.Where(x => x.AllowSelfSubscription == filter.AllowSelfSubscription);

            if (filter.AddNewUsersAutomatically.HasValue)
                query = query.Where(x => x.AddNewUsersAutomatically == filter.AddNewUsersAutomatically);

            if (filter.SurveyNecessity != null)
                query = query.Where(x => x.SurveyNecessity == filter.SurveyNecessity);

            if (filter.SurveyFormIdentifier.HasValue)
                query = query.Where(x => x.SurveyFormIdentifier == filter.SurveyFormIdentifier.Value);

            if (filter.ExcludeJournalSetupIdentifier.HasValue)
            {
                query = query.Where(x => !db.QJournalSetupGroups.Any(y =>
                    y.JournalSetupIdentifier == filter.ExcludeJournalSetupIdentifier
                    && y.GroupIdentifier == x.GroupIdentifier
                ));
            }

            if (filter.HasLifetime.HasValue)
            {
                if (filter.HasLifetime.Value)
                    query = query.Where(x => x.LifetimeQuantity != null && x.LifetimeUnit != null);
                else
                    query = query.Where(x => x.LifetimeQuantity == null || x.LifetimeUnit == null);
            }

            if (filter.MembershipProductIdentifier.HasValue)
                query = query.Where(x => x.MembershipProductIdentifier == filter.MembershipProductIdentifier.Value);

            if (filter.OnlyOperatorCanAddUser.HasValue)
                query = query.Where(x => x.OnlyOperatorCanAddUser == filter.OnlyOperatorCanAddUser);

            return query;
        }

        public int CountSelectorGroups(QGroupSelectorFilter filter)
        {
            using (var db = CreateContext())
                return CreateQueryByQGroupSelectorFilter(filter, db).Count();
        }

        public List<GroupSelectorItem> GetSelectorGroups(QGroupSelectorFilter filter, bool includeShippingAddress)
        {
            using (var db = CreateContext())
            {
                var query = CreateQueryByQGroupSelectorFilter(filter, db);

                query = query
                    .OrderBy(x => x.GroupName)
                    .ApplyPaging(filter);

                if (includeShippingAddress)
                {
                    var addressType = AddressType.Shipping.ToString();

                    return query
                        .Select(x => new GroupSelectorItem
                        {
                            GroupIdentifier = x.GroupIdentifier,
                            GroupName = x.GroupName,
                            GroupCode = x.GroupCode,
                            ShippingAddress = x.Addresses.Where(y => y.AddressType == addressType).FirstOrDefault()
                        })
                        .ToList();
                }

                return query
                    .Select(x => new GroupSelectorItem
                    {
                        GroupIdentifier = x.GroupIdentifier,
                        GroupName = x.GroupName
                    })
                    .ToList();
            }
        }

        private static IQueryable<QGroup> CreateQueryByQGroupSelectorFilter(QGroupSelectorFilter filter, InternalDbContext db)
        {
            var predicate = PredicateBuilder.True<QGroup>();

            if (filter.AncestorName.HasValue())
                predicate = predicate.And(x => x.ConnectionParents.Any(y => y.ParentGroup.GroupName == filter.AncestorName));

            if (filter.IsEmployer.HasValue)
            {
                if (filter.IsEmployer.Value)
                {
                    predicate = predicate.And(x =>
                        x.VMemberships.Any(y =>
                            db.Persons.Any(z =>
                                z.UserIdentifier == y.UserIdentifier
                                && z.OrganizationIdentifier == x.OrganizationIdentifier
                                && z.EmployerGroupIdentifier == x.GroupIdentifier
                            )
                        )
                    );
                }
                else
                {
                    predicate = predicate.And(x =>
                        !x.VMemberships.Any(y =>
                            db.Persons.Any(z =>
                                z.UserIdentifier == y.UserIdentifier
                                && z.OrganizationIdentifier == x.OrganizationIdentifier
                                && z.EmployerGroupIdentifier == x.GroupIdentifier
                            )
                        )
                    );
                }
            }

            if (filter.GrandparentName.HasValue())
                predicate = predicate.And(x => x.Parent.Parent.GroupName == filter.GrandparentName);

            if (filter.GroupNameStartsWith.HasValue())
                predicate = predicate.And(x => x.GroupName.StartsWith(filter.GroupNameStartsWith));

            if (filter.GroupNameEndsWithAny != null && filter.GroupNameEndsWithAny.Length > 0)
                predicate = predicate.And(x => filter.GroupNameEndsWithAny.Any(y => x.GroupName.EndsWith(y)));

            if (filter.ParentName.HasValue())
                predicate = predicate.And(x => x.Parent.GroupName == filter.ParentName);

            if (filter.Keyword.HasValue())
                predicate = predicate.And(x => x.GroupCode.StartsWith(filter.Keyword) || x.GroupName.Contains(filter.Keyword));

            if (filter.MembershipUserIdentifier.HasValue)
                predicate = predicate.And(x => x.VMemberships.FirstOrDefault(y => y.UserIdentifier == filter.MembershipUserIdentifier.Value) != null);

            if (filter.OrganizationIdentifier.HasValue)
                predicate = predicate.And(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.GroupTypes.IsNotEmpty())
                predicate = predicate.And(x => filter.GroupTypes.Contains(x.GroupType));

            if (filter.GroupLabel.HasValue())
                predicate = predicate.And(x => x.GroupLabel == filter.GroupLabel);

            if (filter.MustHavePermissions)
                predicate = predicate.And(x => db.TGroupPermissions.Any(y => y.GroupIdentifier == x.GroupIdentifier));

            if (filter.ExcludeAdministrators)
                predicate = predicate.And(x => !x.GroupName.Contains("Administrator"));

            if (filter.ParentGroupIdentifier.HasValue && filter.AncestorGroupIdentifier.HasValue)
                predicate = predicate.And(x => x.ParentGroupIdentifier == filter.ParentGroupIdentifier.Value || x.ConnectionParents.Any(y => y.ParentGroupIdentifier == filter.AncestorGroupIdentifier));
            else if (filter.ParentGroupIdentifier.HasValue)
                predicate = predicate.And(x => x.ParentGroupIdentifier == filter.ParentGroupIdentifier.Value);
            else if (filter.AncestorGroupIdentifier.HasValue)
                predicate = predicate.And(x => x.ConnectionParents.Any(y => y.ParentGroupIdentifier == filter.AncestorGroupIdentifier));

            if (filter.DownstreamUserIdentifiers.IsNotEmpty())
            {
                predicate = filter.DownstreamContactRelationshipType.HasValue()
                    ? predicate.And(x => x.VMemberships.Any(y => filter.DownstreamUserIdentifiers.Contains(y.UserIdentifier) && y.MembershipType == filter.DownstreamContactRelationshipType))
                    : predicate.And(x => x.VMemberships.Any(y => filter.DownstreamUserIdentifiers.Contains(y.UserIdentifier)));
            }

            if (filter.IncludeGroupIdentifiers.IsNotEmpty())
                predicate = predicate.And(x => filter.IncludeGroupIdentifiers.Contains(x.GroupIdentifier));

            if (filter.ExcludeGroupIdentifiers.IsNotEmpty())
                predicate = predicate.And(x => !filter.ExcludeGroupIdentifiers.Contains(x.GroupIdentifier));

            if (filter.HasChildren.HasValue)
                predicate = filter.HasChildren.Value
                    ? predicate.And(x => x.Children.Any())
                    : predicate.And(x => !x.Children.Any());

            if (filter.IsEventLocation.HasValue)
                predicate = filter.IsEventLocation.Value
                    ? predicate.And(x => db.Events.Any(y => y.VenueLocationIdentifier == x.GroupIdentifier))
                    : predicate.And(x => !db.Events.Any(y => y.VenueLocationIdentifier == x.GroupIdentifier));

            if (filter.IsRegistrationEventLocation.HasValue)
                predicate = filter.IsRegistrationEventLocation.Value
                    ? predicate.And(x => db.Registrations.Any(y => y.Event.VenueLocationIdentifier == x.GroupIdentifier))
                    : predicate.And(x => !db.Registrations.Any(y => y.Event.VenueLocationIdentifier == x.GroupIdentifier));

            if (filter.EmployerContactUserIdentifier.HasValue)
                predicate = predicate.And(x => x.VMemberships.Any(y => y.UserIdentifier == filter.EmployerContactUserIdentifier && y.MembershipType == MembershipType.EmployerContact));

            if (filter.OnlyOperatorCanAddUser.HasValue)
                predicate = predicate.And(x => x.OnlyOperatorCanAddUser == filter.OnlyOperatorCanAddUser);

            var query = db.QGroups.AsNoTracking().AsExpandable();

            if (filter.AlwaysIncludeGroupIdentifiers.IsNotEmpty())
            {
                var orPredicate = PredicateBuilder.False<QGroup>();

                orPredicate = orPredicate.Or(x => filter.AlwaysIncludeGroupIdentifiers.Contains(x.GroupIdentifier));
                orPredicate = orPredicate.Or(predicate.Expand());

                query = query.Where(orPredicate);
            }
            else
                query = query.Where(predicate);

            return query;
        }

        public List<SubgroupDataItem> GetSubgroups(Guid group)
        {
            using (var db = new InternalDbContext())
            {
                return db.QGroups
                    .Where(x => x.ParentGroupIdentifier == group)
                    .OrderBy(x => x.GroupType)
                    .ThenBy(x => x.GroupName)
                    .Select(x => new SubgroupDataItem
                    {
                        Identifier = x.GroupIdentifier,
                        Type = x.GroupType,
                        Name = x.GroupName
                    })
                    .ToList();
            }
        }

        public List<SupergroupDataItem> GetSupergroups(Guid organization, Guid group)
        {
            using (var db = new InternalDbContext())
            {
                return db.QGroups
                    .Where(x => x.OrganizationIdentifier == organization && x.GroupIdentifier != group)
                    .OrderBy(x => x.GroupType)
                    .ThenBy(x => x.GroupName)
                    .Select(x => new SupergroupDataItem
                    {
                        GroupIdentifier = x.GroupIdentifier,
                        GroupType = x.GroupType,
                        Name = x.GroupName,
                        Selected = x.ConnectionChildren.Any(y => y.ChildGroupIdentifier == group),
                        Enabled = true
                    })
                    .ToList();
            }
        }

        public List<GroupOutlineItem> GetGroupOutlineItems(Guid organization, string keyword)
        {
            using (var db = CreateContext())
            {
                return db.QGroups.Where(x => x.OrganizationIdentifier == organization
                        && (string.IsNullOrEmpty(keyword) || (!string.IsNullOrEmpty(keyword) && x.GroupName.Contains(keyword)))
                    )
                    .Select(x => new GroupOutlineItem
                    {
                        GroupIdentifier = x.GroupIdentifier,
                        ParentGroupIdentifier = x.ParentGroupIdentifier,
                        GroupName = x.GroupName,
                        GroupType = x.GroupType,
                        GroupCode = x.GroupCode,
                        MemberCount = x.VMemberships.Count(),
                        GroupActionCount = db.TGroupPermissions.Where(y => y.GroupIdentifier == x.GroupIdentifier).Count()
                    })
                    .ToList();
            }
        }

        public GroupRelationshipInfo[] GetGroupDescendentRelationships(Guid root)
        {
            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<GroupRelationshipInfo>("SELECT * FROM contacts.GetGroupDescendentRelationships(@RootGroupIdentifier)", new SqlParameter("@RootGroupIdentifier", root))
                    .ToArray();
            }
        }

        public bool CausesCycle(Guid child, Guid parent)
        {
            if (child == parent)
                return true;

            var relationships = GetGroupDescendentRelationships(child);

            foreach (var r in relationships)
            {
                if (!r.ParentGroupIdentifier.HasValue)
                    continue;

                if (r.ChildGroupIdentifier == parent)
                    return true;
            }

            return false;
        }

        public ICollection<CountModel> CountPerLabel(Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                var groups = db.QGroups
                    .Where(x => x.OrganizationIdentifier == organizationId && x.GroupLabel != null)
                    .GroupBy(x => new { x.GroupLabel })
                    .Select(x => new { x.Key.GroupLabel, Count = x.Count(), Icon = x })
                    .ToList();

                return groups.Select(x => new CountModel
                    (
                        null,
                        x.GroupLabel,
                        GroupTypes.GetIcon(x.GroupLabel),
                        x.Count
                    ))
                    .OrderBy(x => x.Name)
                    .ToArray();
            }
        }

        public ICollection<CountModel> CountPerType(Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                var groups = db.QGroups
                    .Where(x => x.OrganizationIdentifier == organizationId)
                    .GroupBy(x => new { x.GroupType })
                    .Select(x => new { x.Key.GroupType, Count = x.Count() })
                    .ToList();

                return groups.Select(x => new CountModel
                    (
                        x.GroupType,
                        null,
                        GroupTypes.GetIcon(x.GroupType),
                        x.Count
                    ))
                    .OrderBy(x => x.Name)
                    .ToArray();
            }
        }

        public List<UserRoleItem> GetUserRoles(Guid organization, Guid[] groups, bool isPlatformAdministrator, Guid user)
        {
            using (var db = CreateContext())
            {
                return db.QGroups.Where(x => x.OrganizationIdentifier == organization
                        && (isPlatformAdministrator || groups.Any(g => g == x.GroupIdentifier))
                        && x.GroupType == GroupTypes.Role
                    )
                    .Select(x => new UserRoleItem
                    {
                        GroupIdentifier = x.GroupIdentifier,
                        GroupName = x.GroupName,
                        OnlyOperatorCanAddUser = x.OnlyOperatorCanAddUser,
                        Selected = x.VMemberships.Any(y => y.UserIdentifier == user)
                    })
                    .OrderBy(x => x.GroupName)
                    .ToList();
            }
        }

        public List<UserRoleItem> GetUserRoles(Guid organization, Guid user)
        {
            using (var db = CreateContext())
            {
                return db.QGroups.Where(x => x.OrganizationIdentifier == organization
                        && x.GroupType == GroupTypes.Role
                    )
                    .Select(x => new UserRoleItem
                    {
                        GroupIdentifier = x.GroupIdentifier,
                        GroupName = x.GroupName,
                        OnlyOperatorCanAddUser = x.OnlyOperatorCanAddUser,
                        Selected = x.VMemberships.Any(y => y.UserIdentifier == user)
                    })
                    .OrderBy(x => x.GroupName)
                    .ToList();
            }
        }

        public List<string> GetGroupLabels(Guid organization, string groupType)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.QGroups.Where(x => x.OrganizationIdentifier == organization);
                if (!string.IsNullOrEmpty(groupType))
                    query = query.Where(x => x.GroupType == groupType);

                return query
                    .Where(x => x.GroupLabel != null)
                    .Select(x => x.GroupLabel)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }

        public List<string> GetGroupTags(Guid group)
        {
            using (var db = new InternalDbContext())
                return db.QGroupTags.Where(x => x.GroupIdentifier == group).OrderBy(x => x.GroupTag).Select(x => x.GroupTag).ToList();
        }

        public QGroupAddress GetAddress(Guid groupIdentifier, AddressType type)
        {
            var typeText = type.ToString();

            using (var db = CreateContext())
            {
                return db.QGroupAddresses
                    .Where(x => x.GroupIdentifier == groupIdentifier && x.AddressType == typeText)
                    .FirstOrDefault();
            }
        }

        public List<QGroupAddress> GetAddresses(Guid groupIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QGroupAddresses
                    .Where(x => x.GroupIdentifier == groupIdentifier)
                    .ToList();
            }
        }

        public int CountParentConnections(Guid childGroupIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QGroupConnections
                    .Where(x => x.ChildGroupIdentifier == childGroupIdentifier)
                    .Count();
            }
        }

        public List<QGroupConnection> GetParentConnections(Guid childGroupIdentifier, params Expression<Func<QGroupConnection, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QGroupConnections
                    .Where(x => x.ChildGroupIdentifier == childGroupIdentifier)
                    .ApplyIncludes(includes);

                return query.ToList();
            }
        }

        public int CountChildConnections(Guid parentGroupIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QGroupConnections
                    .Where(x => x.ParentGroupIdentifier == parentGroupIdentifier)
                    .Count();
            }
        }

        public List<QGroupConnection> GetChildConnections(Guid parentGroupIdentifier, params Expression<Func<QGroupConnection, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QGroupConnections
                    .Where(x => x.ParentGroupIdentifier == parentGroupIdentifier)
                    .ApplyIncludes(includes);

                return query.ToList();
            }
        }

        private InternalDbContext CreateContext()
            => new InternalDbContext(true) { EnablePrepareToSaveChanges = false };

        public string GetOrganizationCode(Guid organization)
        {
            using (var db = CreateContext())
            {
                return db.Organizations.AsNoTracking().AsQueryable()
                    .Where(x => x.OrganizationIdentifier == organization)
                    .Select(x => x.OrganizationCode)
                    .FirstOrDefault();
            }
        }

        public List<T> BindGroups<T>(
            Expression<Func<QGroup, T>> binder,
            Expression<Func<QGroup, bool>> filter,
            string orderBy = null)
        {
            using (var db = CreateContext())
            {
                var query = db.QGroups.AsNoTracking().AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                if (orderBy.IsNotEmpty())
                    query = query.OrderBy(orderBy);

                return query.Select(binder).ToList();
            }
        }
    }
}
