using System;
using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Desktops.Custom.SkilledTradesBC.Individuals.Controls
{
    public partial class ContactGroupGrid : SearchResultsGridViewController<NullFilter>
    {
        protected override bool IsFinder => false;

        private Guid ContactID
        {
            get { return (Guid?)ViewState[nameof(ContactID)] ?? Guid.Empty; }
            set { ViewState[nameof(ContactID)] = value; }
        }

        public void LoadData(Guid contactIdentifier)
        {
            ContactID = contactIdentifier;

            Search(new NullFilter());
        }

        protected override int SelectCount(NullFilter filter)
        {
            return MembershipSearch.Count(x => x.UserIdentifier == ContactID);
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            return MembershipSearch.Bind(
                    x => new
                    {
                        Thumbprint = x.Group.GroupIdentifier,
                        GroupType = x.Group.GroupType,
                        GroupName = x.Group.GroupName,
                        GroupSize = x.Group.Memberships.Count()
                    },
                    x => x.UserIdentifier == ContactID,
                    filter.Paging,
                    "GroupType, GroupName"
                )
                .ToSearchResult();
        }
    }
}