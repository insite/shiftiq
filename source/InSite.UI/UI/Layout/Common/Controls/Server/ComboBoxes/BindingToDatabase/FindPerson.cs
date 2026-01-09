using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Common.Web.UI
{
    public class FindPerson : BaseFindEntity<PersonFilter>
    {
        #region Classes

        private class PersonDataItem
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string AccountNumber { get; set; }

            public static readonly Expression<Func<Person, PersonDataItem>> Binder = LinqExtensions1.Expr((Expression<Func<Person, PersonDataItem>>)((Person x) => new PersonDataItem
            {
                UserIdentifier = x.UserIdentifier,
                FullName = x.User.FullName,
                AccountNumber = x.PersonCode,
            }));

            public DataItem GetDataItem() => new DataItem
            {
                Value = UserIdentifier,
                Text = FullName + (AccountNumber.IsNotEmpty() ? " (" + AccountNumber + ")" : string.Empty)
            };
        }

        #endregion

        #region Properties

        public PersonFilter Filter => (PersonFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new PersonFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier
            }));

        public bool EmptyOnLoad
        {
            get => (bool)(ViewState[nameof(EmptyOnLoad)] ?? false);
            set => ViewState[nameof(EmptyOnLoad)] = value;
        }

        public string EntityName
        {
            get => (string)(ViewState[nameof(EntityName)] ?? "Person");
            set => ViewState[nameof(EntityName)] = value;
        }

        public string KeywordFieldText
        {
            get => (string)ViewState[nameof(KeywordFieldText)];
            set => ViewState[nameof(KeywordFieldText)] = value;
        }

        public string NoItemsMessageText
        {
            get => (string)ViewState[nameof(NoItemsMessageText)];
            set => ViewState[nameof(NoItemsMessageText)] = value;
        }

        #endregion

        protected override string GetEntityName() => EntityName;

        protected override string GetEditorUrl() => "/ui/admin/contacts/people/edit?contact={value}";

        protected override PersonFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.NameOrAccountNumber = keyword;

            return filter;
        }

        protected override int Count(PersonFilter filter)
        {
            if (EmptyOnLoad)
                return 0;

            return PersonCriteria.Count(filter);
        }

        protected override DataItem[] Select(PersonFilter filter)
        {
            if (EmptyOnLoad)
                return null;

            filter.OrderBy = "FullName,AccountNumber";

            return PersonCriteria
                .Bind(PersonDataItem.Binder, filter)
                .Select(x => x.GetDataItem())
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            if (EmptyOnLoad)
                return null;

            return PersonCriteria
                .Bind(
                    PersonDataItem.Binder,
                    new PersonFilter
                    {
                        IncludeUserIdentifiers = ids,
                        OrganizationIdentifier = Filter.OrganizationIdentifier,
                        OrderBy = "FullName,AccountNumber"
                    })
                .Select(x => x.GetDataItem());
        }

        #region Helper methods

        public static string GetItemText(Guid organizationId, Guid userId)
        {
            var item = PersonCriteria.BindFirst(PersonDataItem.Binder, new PersonFilter
            {
                OrganizationIdentifier = organizationId,
                UserIdentifier = userId
            });

            return item?.GetDataItem().Text;
        }

        protected override ClientSideSettings CreateClientSideSettings(string clientId, string uniqueId)
        {
            var result = base.CreateClientSideSettings(clientId, uniqueId);

            if (KeywordFieldText.IsNotEmpty())
                result.Strings.KeywordText = EnableTranslation ? HttpUtility.HtmlEncode(Translate(KeywordFieldText)) : KeywordFieldText;

            if (NoItemsMessageText.IsNotEmpty())
                result.Strings.NoItemsMessage = EnableTranslation ? HttpUtility.HtmlEncode(Translate(NoItemsMessageText)) : NoItemsMessageText;

            return result;
        }

        #endregion
    }
}