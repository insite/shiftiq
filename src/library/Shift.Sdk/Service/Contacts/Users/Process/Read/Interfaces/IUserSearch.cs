using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Contacts.Read
{
    public interface IUserSearch
    {
        bool IsUserExist(Guid userId);
        bool IsUserExist(string email);
        bool? IsOrphan(string email);
        QUser GetUser(Guid userId);
        QUser GetUserByEmail(string email);
        List<QUserConnection> GetConnections(Guid fromUserId);
        QUserConnection GetConnection(Guid fromUserId, Guid toUserId);
        int CountConnections(QUserConnectionFilter filter);
        List<QUserConnection> GetConnections(QUserConnectionFilter filter, params Expression<Func<QUserConnection, object>>[] includes);
    }
}
