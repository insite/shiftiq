using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Common.Web.UI
{
    public class FindUser : BaseFindEntity<QUserFilter>
    {
        #region Classes

        private class EntityInfo
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string PersonCode { get; set; }

            private static Guid OrganizationID = CurrentSessionState.Identity.Organization.Identifier;

            public static readonly Expression<Func<VUser, EntityInfo>> Binder = LinqExtensions1.Expr(((VUser x) => new EntityInfo
            {
                UserIdentifier = x.UserIdentifier,
                FullName = x.UserFullName,
                PersonCode = x.Persons.Where(y => y.OrganizationIdentifier == OrganizationID).Select(y => y.PersonCode).FirstOrDefault(),
            }));

            public DataItem ToDataItem()
            {
                return new DataItem
                {
                    Value = UserIdentifier,
                    Text = PersonCode.IsNotEmpty() ? FullName + " (" + PersonCode + ")" : FullName
                };
            }
        }

        #endregion

        #region Properties

        public QUserFilter Filter => (QUserFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QUserFilter()));

        public bool FilterByActiveOrganization
        {
            get { return ViewState[nameof(FilterByActiveOrganization)] == null || (bool)ViewState[nameof(FilterByActiveOrganization)]; }
            set { ViewState[nameof(FilterByActiveOrganization)] = value; }
        }

        #endregion

        protected override string GetEntityName() => "User";

        protected override QUserFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.NameOrCode = keyword;

            if (FilterByActiveOrganization && filter.OrganizationIdentifiers.IsEmpty())
                filter.OrganizationIdentifiers = new[] { CurrentSessionState.Identity.Organization.Identifier };

            return filter;
        }

        protected override int Count(QUserFilter filter)
        {
            return ServiceLocator.ContactSearch.Count(filter);
        }

        protected override DataItem[] Select(QUserFilter filter)
        {
            filter.OrderBy = $"{nameof(EntityInfo.FullName)},{nameof(EntityInfo.PersonCode)}";

            return ServiceLocator.ContactSearch
                .Bind(EntityInfo.Binder, filter)
                .Select(x => x.ToDataItem())
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ServiceLocator.ContactSearch
                .Bind(EntityInfo.Binder, x => ids.Contains(x.UserIdentifier))
                .Select(x => x.ToDataItem());
        }
    }
}