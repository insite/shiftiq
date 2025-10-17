using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Resources.Read
{
    public interface IUploadSearch
    {
        IReadOnlyList<T> Bind<T>(Expression<Func<VUpload, T>> binder, Expression<Func<VUpload, bool>> filter, string sortExpression = null);
        int Count(Expression<Func<VUpload, bool>> filter);
    }
}
