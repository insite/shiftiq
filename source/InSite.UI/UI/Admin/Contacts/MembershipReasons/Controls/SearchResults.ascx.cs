using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Admin.Contacts.MembershipReasons.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QMembershipReasonFilter>
    {
        private Dictionary<Guid, string> _createdBy;
        private Dictionary<Guid, string> _personCode;

        #region Classes

        public class ExportDataItem
        {
            public Guid ReasonIdentifier { get; set; }
            public Guid GroupIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }
            public Guid CreatedByIdentifier { get; set; }


            public string GroupName { get; set; }
            public string UserFullName { get; set; }
            public string ReasonType { get; set; }
            public string ReasonSubtype { get; set; }
            public string PersonOccupation { get; set; }
            public string PersonCode { get; set; }

            public DateTimeOffset ReasonEffective { get; set; }
            public DateTimeOffset? ReasonExpiry { get; set; }
            public string CreatedByName { get; internal set; }
        }

        private class SearchDataItem
        {
            public Guid ReasonIdentifier { get; set; }
            public Guid GroupIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }
            public Guid CreatedBy { get; set; }


            public string GroupName { get; set; }
            public string UserFullName { get; set; }
            public string ReasonType { get; set; }
            public string ReasonSubtype { get; set; }
            public string PersonOccupation { get; set; }
            public string CreatedByName { get; set; }

            public DateTimeOffset ReasonEffective { get; set; }
            public DateTimeOffset? ReasonExpiry { get; set; }
        }

        #endregion

        protected override int SelectCount(QMembershipReasonFilter filter)
        {
            return ServiceLocator.MembershipReasonSearch.Count(filter);
        }

        protected override IListSource SelectData(QMembershipReasonFilter filter)
        {
            filter.OrderBy = "Membership.Group.GroupType,Membership.Group.GroupName";

            var data = ServiceLocator.MembershipReasonSearch.Select(filter, x => x.Membership.Group, x => x.Membership.User, x => x.Membership.User.Persons)
                .Select(x => new SearchDataItem()
                {
                    ReasonIdentifier = x.ReasonIdentifier,
                    GroupIdentifier = x.Membership.Group.GroupIdentifier,
                    GroupName = x.Membership.Group.GroupName,
                    UserIdentifier = x.Membership.User.UserIdentifier,
                    UserFullName = x.Membership.User.FullName,
                    ReasonType = x.ReasonType,
                    ReasonSubtype = x.ReasonSubtype,
                    ReasonEffective = x.ReasonEffective,
                    ReasonExpiry = x.ReasonExpiry,
                    PersonOccupation = x.PersonOccupation,
                    CreatedBy = x.CreatedBy,
                })
                .ToList();

            _createdBy = UserSearch
                .Bind(
                    x => new { x.UserIdentifier, x.FullName },
                    new UserFilter { IncludeUserIdentifiers = data.Select(x => x.CreatedBy).Distinct().ToArray() })
                .ToDictionary(x => x.UserIdentifier, x => x.FullName);
            _personCode = PersonCriteria
                .Bind(
                    x => new { x.UserIdentifier, x.PersonCode },
                    new PersonFilter
                    {
                        OrganizationIdentifier = Organization.OrganizationIdentifier,
                        IncludeUserIdentifiers = data.Select(x => x.UserIdentifier).Distinct().ToArray()
                    })
                .ToDictionary(x => x.UserIdentifier, x => x.PersonCode);

            data = data.Select(d =>
            {
                string name;
                d.CreatedByName = (_createdBy != null && _createdBy.TryGetValue(d.CreatedBy, out name))
                    ? name
                    : null;
                return d;
            }).ToList();

            return data.ToSearchResult();
        }

        public override IListSource GetExportData(QMembershipReasonFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<SearchDataItem>().Select(x => new ExportDataItem
            {
                ReasonIdentifier = x.ReasonIdentifier,
                GroupIdentifier = x.GroupIdentifier,
                GroupName = x.GroupName,
                UserIdentifier = x.UserIdentifier,
                UserFullName = x.UserFullName,
                ReasonType = x.ReasonType,
                ReasonSubtype = x.ReasonSubtype,
                ReasonEffective = x.ReasonEffective,
                ReasonExpiry = x.ReasonExpiry,
                PersonOccupation = x.PersonOccupation,
                CreatedByIdentifier = x.CreatedBy,
                CreatedByName = x.CreatedByName,
                PersonCode = _personCode.GetOrDefault(x.UserIdentifier, string.Empty)
            }).ToList().ToSearchResult();
        }

        protected string GetCreatedBy()
        {
            var userId = (Guid)DataBinder.Eval(Page.GetDataItem(), "CreatedBy");

            return _createdBy.GetOrDefault(userId, string.Empty);
        }

        protected string GetPersonCode()
        {
            var userId = (Guid)DataBinder.Eval(Page.GetDataItem(), "UserIdentifier");

            return _personCode.GetOrDefault(userId, string.Empty);
        }
    }
}