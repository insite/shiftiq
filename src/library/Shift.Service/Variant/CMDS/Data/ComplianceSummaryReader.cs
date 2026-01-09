using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Variant.CMDS;

public class ComplianceSummaryReader
{
    private readonly IShiftIdentityService _identityService;
    private readonly CmdsDbContext _cmds;
    private readonly IDbContextFactory<TableDbContext> _core;

    public ComplianceSummaryReader(IShiftIdentityService identityService, CmdsDbContext cmds, IDbContextFactory<TableDbContext> core)
    {
        _identityService = identityService;
        _cmds = cmds;
        _core = core;
    }

    public async Task<IEnumerable<ComplianceSummaryEntity>> ExportAsync(ComplianceSummaryCriteria criteria, CancellationToken token)
    {
        using var db = _core.CreateDbContext();

        var organizationId = _identityService.OrganizationId;

        var allDepartmentUsers = db.QMembership
            .Where(x => x.Group != null && x.Group.OrganizationIdentifier == organizationId && x.Group.GroupType == "Department")
            .Select(x => new DepartmentUser
            {
                UserIdentifier = x.UserIdentifier,
                GroupIdentifier = x.GroupIdentifier,
                MembershipFunction = x.MembershipFunction
            })
            .ToList();

        if (criteria.Learners == null || criteria.Learners.Length == 0)
            criteria.Learners = GetUsersInDepartments(criteria.Departments);

        var a = new SqlParameter("@OrganizationIdentifier", organizationId);
        var b = IdentifierList("@Departments", criteria.Departments);
        var c = IdentifierList("@Users", criteria.Learners);
        var d = IntegerList("@Sequences", criteria.Measurements);
        var e = new SqlParameter("@Option", criteria.ProfileCondition);
        var f = new SqlParameter("@ExcludeUsersWithoutProfile", criteria.LearnerMustHaveProfile);

        var entities = await _cmds.ComplianceSummaries
            .FromSql($"custom_cmds.SelectQUserStatus {a},{b},{c},{d},{e},{f}")
            .ToListAsync(token);

        var removals = new List<ComplianceSummaryEntity>();

        foreach (var entity in entities)
        {
            if (!DepartmentContainsUser(entity.UserIdentifier, entity.DepartmentIdentifier, criteria.DepartmentEmployment, criteria.OrganizationEmployment, criteria.DataAccess, allDepartmentUsers))
                removals.Add(entity);
        }

        foreach (var removal in removals)
        {
            entities.Remove(removal);
        }

        return entities;
    }

    private bool DepartmentContainsUser(Guid user, Guid department, bool? isDepartment, bool? isCompany, bool? isAdmin,
        List<DepartmentUser> allDepartmentUsers)
    {
        var membershipFunctions = new List<string>();

        if (isDepartment ?? false)
            membershipFunctions.Add("Department");

        if (isCompany ?? false)
            membershipFunctions.Add("Organization");

        if (isAdmin ?? false)
            membershipFunctions.Add("Administration");

        if (membershipFunctions.Count == 0)
            return false;

        var matches = allDepartmentUsers.Count(x =>
            x.UserIdentifier == user &&
            x.GroupIdentifier == department &&
            x.MembershipFunction != null &&
            membershipFunctions.Contains(x.MembershipFunction));

        return matches > 0;
    }

    private Guid[] GetUsersInDepartments(Guid[] departments)
    {
        if (departments == null || departments.Length == 0)
            return Array.Empty<Guid>();

        var csv = string.Empty;

        foreach (var department in departments)
        {
            csv += $"'{department}'";

            if (department != departments.Last())
                csv += ",";
        }

        var query = $"select distinct UserIdentifier from contacts.QMembership where MembershipFunction = 'Department' and GroupIdentifier in ({csv})";

        return _cmds.Database
            .SqlQueryRaw<Guid>(query)
            .ToArray();
    }

    public static SqlParameter IdentifierList(string parameterName, IEnumerable<Guid> values)
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("IdentifierItem", typeof(Guid));

        if (values != null)
        {
            foreach (var value in values)
            {
                var row = table.NewRow();
                row["IdentifierItem"] = value;

                table.Rows.Add(row);
            }
        }

        var param = new SqlParameter(parameterName, System.Data.SqlDbType.Structured)
        {
            TypeName = "dbo.IdentifierList",
            Value = table
        };

        return param;
    }

    public static SqlParameter IntegerList(string parameterName, IEnumerable<int> values)
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("KeyValue", typeof(int));

        if (values != null)
        {
            foreach (var value in values)
            {
                var row = table.NewRow();
                row["KeyValue"] = value;

                table.Rows.Add(row);
            }
        }

        var param = new SqlParameter(parameterName, System.Data.SqlDbType.Structured)
        {
            TypeName = "dbo.IntegerList",
            Value = table
        };

        return param;
    }

    private class DepartmentUser
    {
        public Guid UserIdentifier { get; internal set; }
        public Guid GroupIdentifier { get; internal set; }
        public string? MembershipFunction { get; internal set; }
    }
}