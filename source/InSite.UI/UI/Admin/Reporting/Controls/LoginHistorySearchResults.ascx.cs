using System;
using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class LoginHistorySearchResults : SearchResultsGridViewController<TUserSessionFilter>
    {
        protected override int SelectCount(TUserSessionFilter filter)
        {
            return TUserSessionSearch.Count(filter);
        }

        protected override IListSource SelectData(TUserSessionFilter filter)
        {
            filter.OrderBy = "SessionStarted DESC, UserEmail ASC";

            return TUserSessionSearch.Select(filter).ToSearchResult();
        }

        public class ExportDataItem
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid SessionIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }

            public string AuthenticationErrorMessage { get; set; }
            public string AuthenticationErrorType { get; set; }
            public string SessionCode { get; set; }
            public string UserAgent { get; set; }
            public string UserBrowser { get; set; }
            public string UserBrowserVersion { get; set; }
            public string UserEmail { get; set; }
            public string UserHostAddress { get; set; }
            public string UserLanguage { get; set; }
            public string OrganizationRole { get; set; }
            public string AuthenticationSource { get; set; }

            public int? SessionMinutes { get; set; }

            public DateTimeOffset SessionStarted { get; set; }
            public DateTimeOffset? SessionStopped { get; set; }

            public bool SessionIsAuthenticated { get; set; }
        }

        public override IListSource GetExportData(TUserSessionFilter filter, bool empty)
        {
            if (empty)
                return new ExportDataItem[0].ToSearchResult();

            return TUserSessionSearch.Select(filter)
                    .Cast<TUserSessionDetail>()
                    .Select(x => new ExportDataItem
                    {
                        UserIdentifier = x.UserIdentifier,
                        AuthenticationErrorMessage = x.AuthenticationErrorMessage,
                        AuthenticationErrorType = x.AuthenticationErrorType,
                        AuthenticationSource = x.AuthenticationSource,
                        OrganizationIdentifier = x.OrganizationIdentifier,
                        SessionCode = x.SessionCode,
                        SessionIdentifier = x.SessionIdentifier,
                        SessionIsAuthenticated = x.SessionIsAuthenticated,
                        SessionMinutes = x.SessionMinutes,
                        SessionStarted = x.SessionStarted,
                        SessionStopped = x.SessionStopped,
                        UserAgent = x.UserAgent,
                        UserBrowser = x.UserBrowser,
                        UserBrowserVersion = x.UserBrowserVersion,
                        UserEmail = x.UserEmail,
                        UserHostAddress = x.UserHostAddress,
                        UserLanguage = x.UserLanguage,
                        OrganizationRole = (x.IsAdministrator ? "Administrator" : "") +
                                           (x.IsAdministrator && x.IsLearner ? ", " : "") +
                                           (x.IsLearner ? "Learner" : "")
                    })
                    .ToList()
                    .ToSearchResult();
        }
    }
}