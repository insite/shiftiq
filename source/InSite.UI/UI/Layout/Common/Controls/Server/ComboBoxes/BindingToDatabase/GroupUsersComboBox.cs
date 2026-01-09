using System;
using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class GroupUsersComboBox : ComboBox
    {
        #region Properties

        public Guid? GroupIdentifire
        {
            get { return ViewState[nameof(GroupIdentifire)] as Guid? ?? null; }
            set { ViewState[nameof(GroupIdentifire)] = value; }
        }

        #endregion

        #region Data binding

        protected override ListItemArray CreateDataSource()
        {
            if(GroupIdentifire.HasValue )
            {
                var members = MembershipSearch.Select(x => x.GroupIdentifier == GroupIdentifire.Value, x=>x.User)
                    .ToList().Select(
                    x => new ListItem
                    {
                        Value = x.UserIdentifier.ToString(),
                        Text = x.User.FullName
                    }).ToList();
                ;
                return new ListItemArray(members);
            }
            return new ListItemArray();
        }

        #endregion
    }
}