using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Portal.Home.Controls
{
    public partial class Authentications : SearchResultsGridViewController<NullFilter>
    {
        protected override bool IsFinder => false;

        public void LoadData()
        {
            Search(new NullFilter());
        }

        protected override int SelectCount(NullFilter filter)
        {
            TUserSessionFilter filter2 = new TUserSessionFilter()
            {
                UserIdentifier = User.UserIdentifier
            };

            var loginHistoryItem = TUserSessionSearch.SelectFirst(x => x.UserIdentifier == User.UserIdentifier);
            if (loginHistoryItem == null)
                return 0;

            return 1;
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            var user = User;
            var timeZone = user.TimeZone;

            TUserSessionFilter sessionFilter = new TUserSessionFilter()
            {
                UserIdentifier = user.UserIdentifier,
                Paging = filter.Paging,
                OrderBy = "SessionStarted desc"
            };

            var data = TUserSessionSearch.Bind(x => new
            {
                x.SessionStarted,
                x.UserEmail,
                x.UserHostAddress,
                x.UserBrowser,
                x.UserLanguage,
                x.AuthenticationErrorType,
                x.AuthenticationErrorMessage,
                x.SessionIsAuthenticated,
            }, sessionFilter).Take(20)
            .Select(x => new
            {
                Started = TimeZones.Format(x.SessionStarted, timeZone),
                Status = (x.SessionIsAuthenticated ? "Succeeded" : "Failed"),
                StatusInfo = x.AuthenticationErrorMessage,
                Browser = x.UserBrowser,
                Host = x.UserHostAddress,
                Language = x.UserLanguage

            })
            .ToList();

            return new SearchResultList(data);
        }
    }
}