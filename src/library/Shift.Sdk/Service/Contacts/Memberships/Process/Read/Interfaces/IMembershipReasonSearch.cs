using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Domain.Contacts;

namespace InSite.Application.Contacts.Read
{
    public interface IMembershipReasonSearch
    {
        QMembershipReason Select(Guid reasonId, params Expression<Func<QMembershipReason, object>>[] includes);

        List<QMembershipReason> Select(QMembershipReasonFilter filter, params Expression<Func<QMembershipReason, object>>[] includes);
        int Count(QMembershipReasonFilter filter);
        bool Exists(QMembershipReasonFilter filter);

        List<ReferralGridDataItem> SelectForReferralGrid(QMembershipReasonFilter filter);
    }
}
