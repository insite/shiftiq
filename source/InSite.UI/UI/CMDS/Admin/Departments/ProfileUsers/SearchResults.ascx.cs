using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Custom.CMDS.Admin.Standards.DepartmentProfileUsers.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<DepartmentProfileUserFilter>
    {
        protected override void OnRowCommand(GridViewRow row, string name, object argument)
        {
            if (name == "DeleteRecord")
            {
                try
                {
                    var department = Grid.GetDataKey<Guid>(row, "DepartmentIdentifier");
                    var profile = Grid.GetDataKey<Guid>(row, "ProfileStandardIdentifier");
                    var user = Grid.GetDataKey<Guid>(row, "UserIdentifier");

                    var entity = DepartmentProfileUserSearch.SelectFirst(
                        x => x.DepartmentIdentifier == department
                          && x.ProfileStandardIdentifier == profile
                          && x.UserIdentifier == user);

                    if (entity != null)
                    {
                        DepartmentProfileUserStore.Delete(new[] { entity }, User.UserIdentifier, Organization.Identifier);
                        SearchWithCurrentPageIndex(Filter);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {

                }
            }
            else
            {
                base.OnRowCommand(row, name, argument);
            }
        }

        protected override int SelectCount(DepartmentProfileUserFilter filter)
        {
            return DepartmentProfileUserSearch.Count(filter);
        }

        protected override IListSource SelectData(DepartmentProfileUserFilter filter)
        {
            return DepartmentProfileUserSearch.Select(filter);
        }
    }
}