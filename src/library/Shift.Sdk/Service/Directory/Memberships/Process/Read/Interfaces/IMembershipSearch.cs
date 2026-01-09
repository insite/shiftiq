using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Contacts.Read
{
    public interface IMembershipSearch
    {
        QMembership Select(Guid membership);
        QMembership Select(Guid user, Guid group);
        QMembershipDeletion SelectDeletion(Guid membershipId);
        bool Exists(Guid user, Guid group);
        Guid? GetMembershipId(Guid user, Guid group);
        List<Guid> GetUserAllMembershipIds(Guid user);
        List<Guid> GetGroupAllMembershipIds(Guid group);
        List<QMembership> SelectExpired(DateTimeOffset expireDate);
        List<QMembership> SelectExpired(Guid groupId, DateTimeOffset nowDate, int lifetimeDays);
        List<QMembership> Select(QMembershipFilter filter, params Expression<Func<QMembership, object>>[] includes);
    }
}