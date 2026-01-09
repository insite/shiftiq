using System;
using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Sdk.UI;

namespace InSite.Admin.Contacts.Memberships.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<MembershipFilter>
    {
        public event EventHandler Updated;

        protected override int SelectCount(MembershipFilter filter)
        {
            return MembershipSearch.Count(filter);
        }

        protected override IListSource SelectData(MembershipFilter filter)
        {
            if (filter.OrderBy.IsEmpty())
                filter.OrderBy = "GroupName,UserFullName";

            var memberships = MembershipSearch.Bind(x =>
                new MembershipSearchResult
                {
                    GroupIdentifier = x.Group.GroupIdentifier,
                    GroupName = x.Group.GroupName,
                    GroupType = x.Group.GroupType,
                    GroupLabel = x.Group.GroupLabel,
                    UserIdentifier = x.User.UserIdentifier,
                    UserFullName = x.User.FullName,
                    PersonCode = x.User.Persons.FirstOrDefault(y => y.OrganizationIdentifier == Organization.Identifier).PersonCode,
                    UserEmail = x.User.Email,
                    MembershipAssigned = x.Assigned,
                    MembershipFunction = x.MembershipType,
                    MembershipExpiry = x.MembershipExpiry
                }, filter);

            filter.Paging = null;
            filter.OrderBy = null;

            var all = MembershipSearch.Bind(x =>
                new MembershipSearchResult
                {
                    GroupIdentifier = x.Group.GroupIdentifier,
                    GroupName = x.Group.GroupName,
                    GroupType = x.Group.GroupType,
                    GroupLabel = x.Group.GroupLabel,
                    UserIdentifier = x.User.UserIdentifier,
                    UserFullName = x.User.FullName,
                    PersonCode = x.User.Persons.FirstOrDefault(y => y.OrganizationIdentifier == Organization.Identifier).PersonCode,
                    UserEmail = x.User.Email,
                    MembershipAssigned = x.Assigned,
                    MembershipExpiry = x.MembershipExpiry,
                    MembershipFunction = x.MembershipType
                }, filter);

            Updated?.Invoke(this, new MembershipSearchResultEventArgs(all));

            return memberships.ToSearchResult();
        }

        public class ExportDataItem
        {
            public Guid GroupIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }
            public string GroupName { get; set; }
            public string GroupType { get; set; }
            public string GroupTag { get; set; }
            public string UserFullName { get; set; }
            public string PersonCode { get; set; }
            public string UserEmail { get; set; }
            public string MembershipFunction { get; set; }


            public DateTimeOffset MembershipAssigned { get; set; }
            public DateTimeOffset? MembershipExpiry { get; set; }
        }

        public override IListSource GetExportData(MembershipFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<MembershipSearchResult>().Select(x => new ExportDataItem
            {
                GroupIdentifier = x.GroupIdentifier,
                UserIdentifier = x.UserIdentifier,
                GroupName = x.GroupName,
                GroupType = x.GroupType,
                GroupTag = x.GroupLabel,
                UserFullName = x.UserFullName,
                MembershipFunction = x.MembershipFunction,
                MembershipAssigned = x.MembershipAssigned,
                MembershipExpiry = x.MembershipExpiry,
                PersonCode = x.PersonCode,
                UserEmail = x.UserEmail,
            }).ToList().ToSearchResult();
        }
    }
}