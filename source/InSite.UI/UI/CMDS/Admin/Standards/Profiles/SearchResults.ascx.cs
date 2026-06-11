using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Profiles.Profiles
{
    public partial class ProfileSearchResults : SearchResultsGridViewController<ProfileFilter>
    {
        protected override int SelectCount(ProfileFilter filter)
        {
            return ProfileRepository.CountSearchResults(filter);
        }

        protected override IListSource SelectData(ProfileFilter filter)
        {
            return ProfileRepository.SelectSearchResults(filter);
        }
    }
}