using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class UserConnectionSearch
    {
        private class UserConnectionReadHelper : ReadHelper<UserConnection>
        {
            public static readonly UserConnectionReadHelper Instance = new UserConnectionReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<UserConnection>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.UserConnections.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static T[] Bind<T>(
            Expression<Func<UserConnection, T>> binder,
            Expression<Func<UserConnection, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            UserConnectionReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T[] Bind<T>(
            Expression<Func<UserConnection, T>> binder,
            Expression<Func<UserConnection, bool>> filter,
            Paging paging,
            string modelSort = null, string entitySort = null) =>
            UserConnectionReadHelper.Instance.Bind(binder, filter, paging, modelSort, entitySort);

        public static int Count(Expression<Func<UserConnection, bool>> filter) =>
            UserConnectionReadHelper.Instance.Count(filter);

        public static int Count(Guid from, Guid to)
        {
            using (var db = new InternalDbContext())
            {
                return db.UserConnections
                    .Count(x => x.FromUserIdentifier == from && x.ToUserIdentifier == to);
            }
        }

        public static UserConnection Select(Guid from, Guid to, params Expression<Func<UserConnection, object>>[] includes)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.UserConnections
                    .Where(x => x.FromUserIdentifier == from && x.ToUserIdentifier == to)
                    .ApplyIncludes(includes);

                return query.SingleOrDefault();
            }
        }

        public static UserConnection[] Select(
            Expression<Func<UserConnection, bool>> filter,
            string sortExpression,
            params Expression<Func<UserConnection, object>>[] includes) =>
            UserConnectionReadHelper.Instance.Select(filter, sortExpression, includes);

        public static List<CmdsUserConnection> SelectCmdsDetails(Guid user, Guid organization, string[] descriptions)
        {
            bool? isLeader = null, isManager = null, isSupervisor = null, isValidator = null;

            foreach (var description in descriptions)
            {
                if (description == UserConnectionType.Leader)
                    isLeader = true;

                if (description == UserConnectionType.Manager)
                    isManager = true;

                else if (description == UserConnectionType.Supervisor)
                    isSupervisor = true;

                else if (description == UserConnectionType.Validator)
                    isValidator = true;
            }

            return SelectCmdsDetails(user, organization, isLeader, isManager, isSupervisor, isValidator, true);
        }

        public static List<CmdsUserConnection> SelectCmdsDetails(Guid user, Guid organization, bool? isLeader, bool? isManager, bool? isSupervisor, bool? isValidator, bool merge = false)
        {
            const string query1 = @"
SELECT C.UserIdentifier,
       C.FullName,
       C.Email,
       p.PhoneWork AS Phone,
       R.IsLeader,
       R.IsManager,
       R.IsSupervisor,
       R.IsValidator,
       CASE WHEN LeaderRole.UserIdentifier IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS CanBeLeader,
       CASE WHEN ManagerRole.UserIdentifier IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS CanBeManager,
       CASE WHEN SupervisorRole.UserIdentifier IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS CanBeSupervisor,
       CASE WHEN ValidatorRole.UserIdentifier IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS CanBeValidator
FROM
    identities.UserConnection AS R
    INNER JOIN identities.[User] AS C ON C.UserIdentifier = R.FromUserIdentifier
    LEFT JOIN contacts.Person AS p ON p.UserIdentifier = R.FromUserIdentifier AND p.OrganizationIdentifier = @OrganizationIdentifier
    LEFT JOIN custom_cmds.UserRole AS LeaderRole ON LeaderRole.UserIdentifier = C.UserIdentifier AND LeaderRole.GroupName = 'CMDS Leaders'    
    LEFT JOIN custom_cmds.UserRole AS ManagerRole ON ManagerRole.UserIdentifier = C.UserIdentifier AND ManagerRole.GroupName = 'CMDS Managers'
    LEFT JOIN custom_cmds.UserRole AS SupervisorRole ON SupervisorRole.UserIdentifier = C.UserIdentifier AND SupervisorRole.GroupName = 'CMDS Supervisors'
    LEFT JOIN custom_cmds.UserRole AS ValidatorRole ON ValidatorRole.UserIdentifier = C.UserIdentifier AND ValidatorRole.GroupName = 'CMDS Validators'
WHERE
    R.ToUserIdentifier = @ToUserIdentifier
    AND (@IsLeader IS NULL OR IsLeader = @IsLeader)    
    AND (@IsManager IS NULL OR IsManager = @IsManager)
    AND (@IsSupervisor IS NULL OR IsSupervisor = @IsSupervisor)
    AND (@IsValidator IS NULL OR IsValidator = @IsValidator)
    AND C.UtcArchived IS NULL
ORDER BY
    C.FullName;";

            const string query2 = @"
SELECT C.UserIdentifier,
       C.FullName,
       C.Email,
       p.PhoneWork AS Phone,
       R.IsLeader,
       R.IsManager,
       R.IsSupervisor,
       R.IsValidator,
       CASE WHEN LeaderRole.UserIdentifier IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS CanBeLeader,
       CASE WHEN ManagerRole.UserIdentifier IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS CanBeManager,
       CASE WHEN SupervisorRole.UserIdentifier IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS CanBeSupervisor,
       CASE WHEN ValidatorRole.UserIdentifier IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS CanBeValidator
FROM
    identities.UserConnection AS R
    INNER JOIN identities.[User] AS C ON C.UserIdentifier = R.FromUserIdentifier
    LEFT JOIN contacts.Person AS p ON p.UserIdentifier = R.FromUserIdentifier AND p.OrganizationIdentifier = @OrganizationIdentifier
    LEFT JOIN custom_cmds.UserRole AS LeaderRole ON LeaderRole.UserIdentifier = C.UserIdentifier AND LeaderRole.GroupName = 'CMDS Leaders'
    LEFT JOIN custom_cmds.UserRole AS ManagerRole ON ManagerRole.UserIdentifier = C.UserIdentifier AND ManagerRole.GroupName = 'CMDS Managers'
    LEFT JOIN custom_cmds.UserRole AS SupervisorRole ON SupervisorRole.UserIdentifier = C.UserIdentifier AND SupervisorRole.GroupName = 'CMDS Supervisors'
    LEFT JOIN custom_cmds.UserRole AS ValidatorRole ON ValidatorRole.UserIdentifier = C.UserIdentifier AND ValidatorRole.GroupName = 'CMDS Validators'
WHERE R.ToUserIdentifier = @ToUserIdentifier
    AND ((@IsLeader IS NULL OR IsLeader = @IsLeader)
      OR (@IsManager IS NULL OR IsManager = @IsManager)
      OR (@IsSupervisor IS NULL OR IsSupervisor = @IsSupervisor)
      OR (@IsValidator IS NULL OR IsValidator = @IsValidator))
    AND C.UtcArchived IS NULL
ORDER BY 
    C.FullName;";

            using (var db = new InternalDbContext())
            {
                var a = new SqlParameter { ParameterName = "@ToUserIdentifier", SqlDbType = SqlDbType.UniqueIdentifier, Value = user };
                var b = new SqlParameter { ParameterName = "@IsLeader", SqlDbType = SqlDbType.Bit, Value = isLeader };
                var c = new SqlParameter { ParameterName = "@IsManager", SqlDbType = SqlDbType.Bit, Value = isManager };
                var d = new SqlParameter { ParameterName = "@IsSupervisor", SqlDbType = SqlDbType.Bit, Value = isSupervisor };
                var e = new SqlParameter { ParameterName = "@IsValidator", SqlDbType = SqlDbType.Bit, Value = isValidator };
                var organizationParam = new SqlParameter { ParameterName = "@OrganizationIdentifier", Value = organization };
                var parameters = new[] { a, b, c, d, e, organizationParam };
                foreach (var parameter in parameters)
                    if (parameter.Value == null)
                        parameter.Value = DBNull.Value;

                return db.Database
                    .SqlQuery<CmdsUserConnection>(merge ? query2 : query1, parameters)
                    .ToList();
            }
        }
    }
}
