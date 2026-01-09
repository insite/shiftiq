using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class EmployeeRepository
    {
        #region SQL

        private static string GroupContainmentQuery = @"
SELECT  
      CAST(0 AS BIT) AS IsHierarchy
    , CAST(0 AS BIT) AS IsIndirect
    , containments.ChildGroupIdentifier
    , containments.ParentGroupIdentifier 
    , parent.ParentGroupIdentifier AS GrandparentGroupIdentifier
    , child.GroupName AS ChildGroupName
    , parent.GroupName AS ParentGroupName
    , grandparent.GroupName AS GrandparentGroupName
FROM contacts.QGroupConnection AS containments
INNER JOIN contacts.QGroup AS child ON containments.ChildGroupIdentifier = child.GroupIdentifier
INNER JOIN contacts.QGroup AS parent ON containments.ParentGroupIdentifier = parent.GroupIdentifier
LEFT JOIN contacts.QGroup AS grandparent ON parent.ParentGroupIdentifier = grandparent.GroupIdentifier
WHERE child.OrganizationIdentifier = @OrganizationIdentifier";

        #endregion

        #region Classes

        public class GroupItem
        {
            public Guid Identifier { get; }
            public string Name { get; }

            public GroupItem(Guid identifier, string name)
            {
                Identifier = identifier;
                Name = name;
            }
        }

        public class EmployeeRow
        {
            public Guid EmployeeUserIdentifier { get; set; }
            public string EmployeeFullName { get; set; }
            public string EmployeeFirstName { get; set; }
            public string EmployeeLastName { get; set; }
            public string EmployeeEmail { get; set; }
            public string EmployeeJobTitle { get; set; }
            public string EmployeeProcessStep { get; set; }
            public string EmployeeGender { get; set; }

            public Guid EmployerOrganizationIdentifier { get; set; }
            public Guid EmployerGroupIdentifier { get; set; }
            public string EmployerGroupName { get; set; }
            public string EmployerGroupNumber { get; set; }
            public string EmployerContactLabel { get; set; }

            public Guid? EmployerDistrictIdentifier { get; set; }
            public string EmployerDistrictName { get; set; }
            public string EmployerDistrictRegion { get; set; }

            public string EmployerShippingAddressStreet1 { get; set; }
            public string EmployerShippingAddressStreet2 { get; set; }
            public string EmployerShippingAddressCity { get; set; }
            public string EmployerShippingAddressProvince { get; set; }
            public string EmployerShippingAddressCountry { get; set; }
            public string EmployerShippingAddressPostalCode { get; set; }
            public string EmployerPhone { get; set; }
            public string EmployerPhoneFax { get; set; }

            public string EmployeeHonorific { get; set; }
            public string EmployeePhone { get; set; }
            public string EmployeePhoneHome { get; set; }
            public string EmployeePhoneMobile { get; set; }
            public DateTime? EmployeeMemberStartDate { get; set; }
            public DateTime? EmployeeMemberEndDate { get; set; }
            public string EmployeeShippingAddressStreet1 { get; set; }
            public string EmployeeShippingAddressCity { get; set; }
            public string EmployeeShippingAddressProvince { get; set; }
            public string EmployeeShippingAddressPostalCode { get; set; }
            public string EmployeeShippingAddressCountry { get; set; }
            public string EmployeeShippingPreference { get; set; }
            public string EmployeeAccountNumber { get; set; }
            public string EmployeeHomeAddressStreet1 { get; set; }
            public string EmployeeHomeAddressCity { get; set; }
            public string EmployeeHomeAddressProvince { get; set; }
            public string EmployeeHomeAddressPostalCode { get; set; }
            public string EmployeeHomeAddressCountry { get; set; }

            public GroupItem[] EmployerParentFunctionalList { get; set; }
            public string EmployerParentFunctional =>
                EmployerParentFunctionalList.IsNotEmpty()
                ? string.Join(", ", EmployerParentFunctionalList.Select(x => x.Name))
                : null;

            public Guid? RolesParticipationGroupIdentifier { get; set; }
            public string RolesParticipationGroupName { get; set; }
        }

        private class GroupContainmentBufferItem
        {
            public bool IsHierarchy { get; set; }
            public bool IsIndirect { get; set; }

            public Guid ChildGroupIdentifier { get; set; }
            public Guid ParentGroupIdentifier { get; set; }
            public Guid? GrandparentGroupIdentifier { get; set; }

            public string ChildGroupName { get; set; }
            public string ParentGroupName { get; set; }
            public string GrandparentGroupName { get; set; }
        }

        private class RoleMembershipBufferItem
        {
            public Guid UserIdentifier { get; set; }
            public Guid GroupIdentifier { get; set; }
            public string GroupName { get; set; }
        }

        #endregion

        #region SELECT (EmployeeFilter)

        public static int CountByEmployeeFilter(EmployeeFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQueryByEmployeeFilter(filter, db).Count();
            }
        }

        public static IListSource SelectByEmployeeFilter(EmployeeFilter filter)
        {
            var sortExpression = filter.SortByColumn.IfNullOrEmpty("EmployeeFirstName, EmployeeLastName");

            using (var db = new InternalDbContext())
            {
                var groupContainmentBuffer = CreateGroupContainmentBuffer(db, filter.OrganizationIdentifier.Value);
                var roleMembershipBuffer = CreateRoleMembershipBuffer(filter.OrganizationIdentifier.Value);

                var query = CreateQueryByEmployeeFilter(filter, db);

                List<Employee> employees = null;

                if (sortExpression.Contains("RolesParticipationGroupName"))
                    employees = query.ToList();
                else
                {
                    employees = query
                        .OrderBy(sortExpression)
                        .ApplyPaging(filter)
                        .ToList();
                }

                var result = new List<EmployeeRow>();

                foreach (var employee in employees)
                {
                    var row = new EmployeeRow
                    {
                        EmployeeUserIdentifier = employee.EmployeeUserIdentifier,
                        EmployeeFullName = employee.EmployeeFullName,
                        EmployeeFirstName = employee.EmployeeFirstName,
                        EmployeeLastName = employee.EmployeeLastName,
                        EmployeeEmail = employee.EmployeeEmail,
                        EmployeeJobTitle = employee.EmployeeJobTitle,
                        EmployeeProcessStep = employee.EmployeeProcessStep,
                        EmployeeGender = employee.EmployeeGender,
                        EmployerOrganizationIdentifier = employee.EmployerOrganizationIdentifier,
                        EmployerGroupIdentifier = employee.EmployerGroupIdentifier,
                        EmployerGroupName = employee.EmployerGroupName,
                        EmployerGroupNumber = employee.EmployerGroupNumber,
                        EmployerContactLabel = employee.EmployerContactLabel,
                        EmployerDistrictIdentifier = employee.EmployerDistrictIdentifier,
                        EmployerDistrictName = employee.EmployerDistrictName,
                        EmployerDistrictRegion = employee.EmployerDistrictRegion,
                        EmployerShippingAddressStreet1 = employee.EmployerShippingAddressStreet1,
                        EmployerShippingAddressStreet2 = employee.EmployerShippingAddressStreet2,
                        EmployerShippingAddressCity = employee.EmployerShippingAddressCity,
                        EmployerShippingAddressProvince = employee.EmployerShippingAddressProvince,
                        EmployerShippingAddressCountry = employee.EmployerShippingAddressCountry,
                        EmployerShippingAddressPostalCode = employee.EmployerShippingAddressPostalCode,
                        EmployerPhone = employee.EmployerPhone,
                        EmployerPhoneFax = employee.EmployerPhoneFax,
                        EmployeeHonorific = employee.EmployeeHonorific,
                        EmployeePhone = employee.EmployeePhone,
                        EmployeePhoneHome = employee.EmployeePhoneHome,
                        EmployeePhoneMobile = employee.EmployeePhoneMobile,
                        EmployeeMemberStartDate = employee.EmployeeMemberStartDate,
                        EmployeeMemberEndDate = employee.EmployeeMemberEndDate,
                        EmployeeShippingAddressStreet1 = employee.EmployeeShippingAddressStreet1,
                        EmployeeShippingAddressCity = employee.EmployeeShippingAddressCity,
                        EmployeeShippingAddressProvince = employee.EmployeeShippingAddressProvince,
                        EmployeeShippingAddressPostalCode = employee.EmployeeShippingAddressPostalCode,
                        EmployeeShippingAddressCountry = employee.EmployeeShippingAddressCountry,
                        EmployeeShippingPreference = employee.EmployeeShippingPreference,
                        EmployeeAccountNumber = employee.EmployeeAccountNumber,
                        EmployeeHomeAddressStreet1 = employee.EmployeeHomeAddressStreet1,
                        EmployeeHomeAddressCity = employee.EmployeeHomeAddressCity,
                        EmployeeHomeAddressProvince = employee.EmployeeHomeAddressProvince,
                        EmployeeHomeAddressPostalCode = employee.EmployeeHomeAddressPostalCode,
                        EmployeeHomeAddressCountry = employee.EmployeeHomeAddressCountry
                    };

                    var containments = groupContainmentBuffer.Where(x => x.ChildGroupIdentifier == employee.EmployerGroupIdentifier).ToList();
                    if (containments.Count > 0)
                    {
                        var parents = containments
                            .Select(x => new GroupItem(x.ParentGroupIdentifier, x.ParentGroupName))
                            .Distinct()
                            .OrderBy(x => x.Name)
                            .ToArray();

                        row.EmployerParentFunctionalList = parents;
                    }

                    var roleMembershipGroup = roleMembershipBuffer.FirstOrDefault(x => employee.EmployeeUserIdentifier == x.UserIdentifier);
                    if (roleMembershipGroup != null)
                    {
                        row.RolesParticipationGroupIdentifier = roleMembershipGroup.GroupIdentifier;
                        row.RolesParticipationGroupName = roleMembershipGroup.GroupName;
                    }

                    result.Add(row);
                }

                if (sortExpression.Contains("RolesParticipationGroupName"))
                {
                    return result
                        .AsQueryable()
                        .OrderBy(sortExpression)
                        .ApplyPaging(filter)
                        .ToSearchResult();
                }
                else
                    return result.ToSearchResult();
            }
        }

        private static List<RoleMembershipBufferItem> CreateRoleMembershipBuffer(Guid organization)
        {
            var memberships = MembershipSearch.Bind(
                x => new RoleMembershipBufferItem
                {
                    UserIdentifier = x.UserIdentifier,
                    GroupIdentifier = x.GroupIdentifier,
                    GroupName = x.Group.GroupName
                },
                x => x.Group.OrganizationIdentifier == organization && x.Group.GroupType == GroupTypes.Role)
                .ToList();

            return memberships;
        }

        private static List<GroupContainmentBufferItem> CreateGroupContainmentBuffer(InternalDbContext db, Guid organization)
        {
            var parameters = new SqlParameter[]
                {
                    new SqlParameter("@OrganizationIdentifier", organization)
                };

            var containments = db.Database
                .SqlQuery<GroupContainmentBufferItem>(GroupContainmentQuery, parameters)
                .ToList();

            return containments;
        }

        private static IQueryable<Employee> CreateQueryByEmployeeFilter(EmployeeFilter filter, InternalDbContext db)
        {
            var query = db.Employees.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.EmployerOrganizationIdentifier == filter.OrganizationIdentifier && x.EmployerOrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.EmployerGroupIdentifier.HasValue)
                query = query.Where(x => x.EmployerGroupIdentifier == filter.EmployerGroupIdentifier);

            if (filter.EmployeeName.IsNotEmpty())
                query = query.Where(x => x.EmployeeFirstName.Contains(filter.EmployeeName) || x.EmployeeLastName.Contains(filter.EmployeeName)
                    || filter.EmployeeName.Contains(x.EmployeeFirstName) || filter.EmployeeName.Contains(x.EmployeeLastName));

            if (filter.EmployeeEmail.IsNotEmpty())
                query = query.Where(x => x.EmployeeEmail.Contains(filter.EmployeeEmail));

            if (filter.EmployeeJobTitle.IsNotEmpty())
                query = query.Where(x => x.EmployeeJobTitle.Contains(filter.EmployeeJobTitle));

            if (filter.MembershipStatuses.IsNotEmpty())
                query = query.Where(x => x.EmployeeProcessStep != null && filter.MembershipStatuses.Contains(x.EmployeeProcessStep));

            if (filter.EmployeeGender.IsNotEmpty())
                query = query.Where(x => x.EmployeeGender == filter.EmployeeGender);

            if (filter.EmployeeJoinedSince.HasValue)
                query = query.Where(x => x.EmployeeMemberStartDate >= filter.EmployeeJoinedSince.Value);

            if (filter.EmployeeJoinedBefore.HasValue)
                query = query.Where(x => x.EmployeeMemberStartDate < filter.EmployeeJoinedBefore.Value);

            if (filter.EmployeeEndedSince.HasValue)
                query = query.Where(x => x.EmployeeMemberEndDate >= filter.EmployeeEndedSince.Value);

            if (filter.EmployeeEndedBefore.HasValue)
                query = query.Where(x => x.EmployeeMemberEndDate < filter.EmployeeEndedBefore.Value);

            if (filter.EmployerNumber.IsNotEmpty())
                query = query.Where(x => x.EmployerGroupNumber.Contains(filter.EmployerNumber));

            if (filter.EmployerDistrictIdentifier.HasValue)
                query = query.Where(x => x.EmployerDistrictIdentifier == filter.EmployerDistrictIdentifier);

            if (filter.MembershipGroupIdentifier.HasValue)
            {
                query = query.Where(x => db.Memberships.Any(y => y.UserIdentifier == x.EmployeeUserIdentifier && y.GroupIdentifier == filter.MembershipGroupIdentifier));
            }

            return query;
        }

        #endregion
    }
}