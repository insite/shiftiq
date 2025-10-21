using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Resources.Read;

namespace InSite.Persistence
{
    public class UploadSearch2 : IUploadSearch
    {
        private class UploadReadHelper : ReadHelper<VUpload>
        {
            public static readonly UploadReadHelper Instance = new UploadReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VUpload>, TResult> func)
            {
                using (var context = new InternalDbContext(false))
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.VUploads.AsNoTracking().AsQueryable();

                    return func(query);
                }
            }
        }

        public IReadOnlyList<T> Bind<T>(Expression<Func<VUpload, T>> binder, Expression<Func<VUpload, bool>> filter, string sortExpression = null) =>
            UploadReadHelper.Instance.Bind(binder, filter, sortExpression);

        public int Count(Expression<Func<VUpload, bool>> filter) =>
            UploadReadHelper.Instance.Count(filter);
    }
}