using System;
using System.ComponentModel;
using System.Linq;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports
{
    public partial class OnlineUsersGrid : SearchResultsGridViewController<NullFilter>
    {
        #region Fields

        private CookieToken[] _currentSessions;

        #endregion

        #region Public methods

        public void LoadData()
        {
            Search(new NullFilter());
        }

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        private CookieToken[] CurrentSessions
        {
            get
            {
                if (_currentSessions == null)
                    LoadCurrentSessions();

                return _currentSessions;
            }
        }

        private void LoadCurrentSessions()
        {
            _currentSessions = CookieTokenModule.GetActiveTokens();

            var parent = Organization.ParentOrganizationIdentifier;

            if (parent.HasValue && parent != Guid.Empty)
                _currentSessions = _currentSessions
                    .Where(x => x.OrganizationCode == CookieTokenModule.Current.OrganizationCode)
                    .ToArray();
        }

        #endregion

        #region Search results

        protected override int SelectCount(NullFilter filter)
        {
            EmptyGrid.Visible = CurrentSessions.Length == 0;

            return CurrentSessions.Length;
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            var items = CurrentSessions
                .ApplyPaging(filter)
                .ToList()
                .Select(x => new ActiveUserGridItem
                {
                    LoginName = x.UserEmail,
                    OrganizationCode = x.OrganizationCode,
                    Started = TimeZones.Format(x.ParseCreated(), User.TimeZone),
                    StartedHumanize = x.ParseCreated().Humanize(),
                    IsOnline = x.IsActive(),
                    Browser = x.CurrentBrowser,
                    BrowserVersion = x.CurrentBrowserVersion
                })
                .ToList();

            foreach (var item in items)
            {
                var organization = OrganizationSearch.Select(item.OrganizationCode);
                var user = UserSearch.SelectWebContact(item.LoginName, organization.Identifier);
                item.UserName = user.FullName;
                item.UserEmail = user.Email;
                item.PersonCode = user.PersonCode;
                item.OrganizationName = organization.CompanyName;
            }

            return new SearchResultList(items);
        }

        #endregion
    }
}