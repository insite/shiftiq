using System;
using System.Linq.Expressions;

namespace InSite.Application.Contacts.Read
{
    public interface IPersonSecretSearch
    {
        QPersonSecret GetSecret(Guid secretId);
        QPersonSecret GetSecret(Guid secretId, params Expression<Func<QPersonSecret, object>>[] includes);
        QPersonSecret GetByPerson(Guid personId, string name);
        QPersonSecret GetBySecretValue(string secret);
        QPersonSecret GetBySecretValue(string secret, params Expression<Func<QPersonSecret, object>>[] includes);
        int Count();
        int Count(QPersonSecretFilter filter);
        T[] Bind<T>(Expression<Func<QPersonSecret, T>> binder, QPersonSecretFilter filter);
        T[] Bind<T>(Expression<Func<QPersonSecret, T>> binder, QPersonSecretFilter filter, params Expression<Func<QPersonSecret, object>>[] includes);
    }
}
