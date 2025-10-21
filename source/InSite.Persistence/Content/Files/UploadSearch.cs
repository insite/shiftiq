using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class UploadSearch
    {
        private class UploadReadHelper : ReadHelper<Upload>
        {
            public static readonly UploadReadHelper Instance = new UploadReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<Upload>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.Uploads.AsNoTracking().AsQueryable();

                    return func(query);
                }
            }
        }

        public static bool Exists(Guid containerId, string name) =>
            UploadReadHelper.Instance.Exists(x => x.ContainerIdentifier == containerId && x.Name == name);

        public static bool Exists(Expression<Func<Upload, bool>> filter) =>
            UploadReadHelper.Instance.Exists(filter);

        public static bool ExistsByOrganizationIdentifier(Guid organizationId, string path) =>
            UploadReadHelper.Instance.Exists(x => x.OrganizationIdentifier == organizationId && x.NavigateUrl == path);

        public static bool ExistsByOrganizationIdentifier(Guid organizationId, Guid uploadId, string path) =>
            UploadReadHelper.Instance.Exists(x => x.OrganizationIdentifier == organizationId && x.UploadIdentifier != uploadId && x.NavigateUrl == path);

        public static Upload Select(Guid uploadId, params Expression<Func<Upload, object>>[] includes) =>
            UploadReadHelper.Instance.SelectFirst(x => x.UploadIdentifier == uploadId, includes);

        public static Upload Select(Guid containerId, string name, params Expression<Func<Upload, object>>[] includes) =>
            UploadReadHelper.Instance.SelectFirst(x => x.ContainerIdentifier == containerId && x.Name == name, includes);

        public static Upload SelectByPath(Guid organizationId, string path, params Expression<Func<Upload, object>>[] includes) =>
            UploadReadHelper.Instance.SelectFirst(x => x.OrganizationIdentifier == organizationId && x.NavigateUrl == path, includes);

        public static IReadOnlyList<T> Bind<T>(
            Expression<Func<Upload, T>> binder,
            Expression<Func<Upload, bool>> filter,
            string sortExpression = null) =>
            UploadReadHelper.Instance.Bind(binder, filter, sortExpression);

        public static T Bind<T>(Guid organizationId, string path, Expression<Func<Upload, T>> binder) =>
            UploadReadHelper.Instance.BindFirst(binder, x => x.OrganizationIdentifier == organizationId && x.NavigateUrl == path);

        public static IReadOnlyList<T> Bind<T>(
            Expression<Func<Upload, T>> binder,
            IEnumerable<Attachment> attachments,
            string sortExpression = null)
        {
            var filter = attachments.Select(x => x.Upload).Distinct().ToArray();
            var stack = string.Join("\n", StringHelper.Split(Environment.StackTrace).Where(x => x.Contains("at InSite")));
            return UploadReadHelper.Instance.Bind(binder, x => stack != null && filter.Contains(x.UploadIdentifier), sortExpression);
        }

        public static IReadOnlyList<T> BindFolderFiles<T>(Guid organizationId, string folder, Expression<Func<Upload, T>> binder, Expression<Func<Upload, bool>> filter = null, string sortExpression = null)
        {
            Expression<Func<Upload, bool>> where = x => x.OrganizationIdentifier == organizationId && DbFunctions.Left(x.NavigateUrl, x.NavigateUrl.Length - x.Name.Length) == folder;
            if (filter != null)
                where = LinqExtensions3.CustomAnd(where, filter);

            return Bind(binder, where, sortExpression);
        }

        public static T BindFirst<T>(
            Expression<Func<Upload, T>> binder,
            Expression<Func<Upload, bool>> filter,
            string sortExpression = null) =>
            UploadReadHelper.Instance.BindFirst(binder, filter, sortExpression);


        public static int CountByFilter(UploadFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return db.Uploads.ApplyFilter(filter, db).Count();
            }
        }

        public static SearchResultList SelectByFilter(UploadFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return db.Uploads
                    .ApplyFilter(filter, db)
                    .Select(x => new
                    {
                        x.UploadIdentifier,
                        x.ContainerType,
                        x.UploadType,
                        x.Title,
                        x.Description,
                        x.Uploader,
                        x.Uploaded
                    })
                    .OrderBy(filter.OrderBy ?? "Uploaded DESC")
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        public static string[] SelectFileTypes(Guid organizationId)
        {
            using (var context = new InternalDbContext())
            {
                return context.Uploads
                    .Where(x => x.UploadType == UploadType.InSiteFile && x.ContainerType == UploadContainerType.Oganization && x.OrganizationIdentifier == organizationId)
                    .Select(x => x.ContentType.ToLower())
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();
            }
        }
    }
}