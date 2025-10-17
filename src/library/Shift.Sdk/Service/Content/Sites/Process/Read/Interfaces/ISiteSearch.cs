using System;
using System.Linq.Expressions;

namespace InSite.Application.Sites.Read
{
    public interface ISiteSearch
    {
        QSite Select(Guid id);
        QSite Select(string title, Guid organizationId);

        int Count(Expression<Func<QSite, bool>> filter);
        CountInfo[] SelectCount(Guid organizationId);
        RecentInfo[] SelectRecent(Guid organizationId, int take);

        T[] Bind<T>(
            Expression<Func<QSite, T>> binder,
            Expression<Func<QSite, bool>> filter,
            string modelSort = null,
        string entitySort = null);

        T BindFirst<T>(
            Expression<Func<QSite, T>> binder,
            Expression<Func<QSite, bool>> filter,
            string modelSort = null,
            string entitySort = null);

        T[] Bind<T>(
            Expression<Func<QSite, T>> binder,
            QSiteFilter filter);

        int Count(QSiteFilter filter);
    }
}