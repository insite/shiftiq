using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class VCmdsCredentialAndExperienceSearch
    {
        #region Classes

        private class ContactExperienceReadHelper : ReadHelper<ContactExperience>
        {
            public static readonly ContactExperienceReadHelper Instance = new ContactExperienceReadHelper();

            public T[] Bind<T>(
                InternalDbContext context,
                Expression<Func<ContactExperience, T>> binder,
                Expression<Func<ContactExperience, bool>> filter,
                string modelSort = null,
                string entitySort = null)
            {
                var query = context.ContactExperiences.AsQueryable().AsNoTracking();
                var modelQuery = BuildQuery(query, binder, filter, null, modelSort, entitySort, false);

                return modelQuery.ToArray();
            }

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<ContactExperience>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.ContactExperiences.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region SELECT

        public static ContactExperience Select(Guid id, params Expression<Func<ContactExperience, object>>[] includes) =>
            ContactExperienceReadHelper.Instance.SelectFirst(x => x.ExperienceIdentifier == id, includes);

        public static ContactExperience SelectFirst(Expression<Func<ContactExperience, bool>> filter, params Expression<Func<ContactExperience, object>>[] includes) =>
            ContactExperienceReadHelper.Instance.SelectFirst(filter, includes);

        public static IReadOnlyList<ContactExperience> Select(
            Expression<Func<ContactExperience, bool>> filter,
            params Expression<Func<ContactExperience, object>>[] includes) => ContactExperienceReadHelper.Instance.Select(filter, includes);

        public static IReadOnlyList<ContactExperience> Select(
            Expression<Func<ContactExperience, bool>> filter,
            string sortExpression,
            params Expression<Func<ContactExperience, object>>[] includes) => ContactExperienceReadHelper.Instance.Select(filter, sortExpression, includes);

        public static T[] Bind<T>(
            Expression<Func<ContactExperience, T>> binder,
            Expression<Func<ContactExperience, bool>> filter,
            string modelSort = null,
            string entitySort = null) => ContactExperienceReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<ContactExperience, T>> binder,
            Expression<Func<ContactExperience, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ContactExperienceReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static int Count(Expression<Func<ContactExperience, bool>> filter) => ContactExperienceReadHelper.Instance.Count(filter);

        public static bool Exists(Guid id) =>
            ContactExperienceReadHelper.Instance.Exists(x => x.ExperienceIdentifier == id);

        public static bool Exists(Expression<Func<ContactExperience, bool>> filter) =>
            ContactExperienceReadHelper.Instance.Exists(filter);

        #endregion
    }
}
