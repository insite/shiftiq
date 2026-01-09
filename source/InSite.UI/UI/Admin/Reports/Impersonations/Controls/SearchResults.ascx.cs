using System;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Reports.Impersonations.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<ImpersonationFilter>
    {
        #region Classes

        public class ExportDataItem
        {
            public string Started { get; set; }
            public string Stopped { get; set; }
            public string ImpersonatorContactName { get; set; }
            public string ImpersonatedContactName { get; set; }
        }

        private class SearchDataItem
        {
            public string ImpersonatedContactName { get; set; }
            public string ImpersonatedContactLink { get; set; }
            public string ImpersonatorContactName { get; set; }
            public string ImpersonatorContactLink { get; set; }
            public DateTimeOffset ImpersonationStarted { get; set; }
            public string Started { get; set; }
            public DateTimeOffset? ImpersonationStopped { get; set; }
            public string Stopped { get; set; }
            public TimeZoneInfo TimeZone { get; set; }
        }

        #endregion

        #region Database

        public override System.ComponentModel.IListSource GetExportData(ImpersonationFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<SearchDataItem>().Select(x => new ExportDataItem
            {
                Started = x.Started,
                Stopped = x.Stopped,
                ImpersonatorContactName = x.ImpersonatorContactName,
                ImpersonatedContactName = x.ImpersonatedContactName
            }).ToList().ToSearchResult();
        }

        protected override System.ComponentModel.IListSource SelectData(ImpersonationFilter filter)
        {
            var user = User;
            var timeZone = user.TimeZone;

            var list = ImpersonationSearch.Select(filter)
                .Select(x => new SearchDataItem
                {
                    ImpersonatedContactName = x.ImpersonatedUserFullName,
                    ImpersonatedContactLink = GetImpersonationName(x.ImpersonatedUserIdentifier, x.ImpersonatedUserFullName),
                    ImpersonatorContactName = x.ImpersonatorUserFullName,
                    ImpersonatorContactLink = GetImpersonationName(x.ImpersonatorUserIdentifier, x.ImpersonatorUserFullName),
                    ImpersonationStarted = x.ImpersonationStarted,
                    Started = x.ImpersonationStarted.Format(user.TimeZone),
                    ImpersonationStopped = x.ImpersonationStopped,
                    Stopped = x.ImpersonationStopped.Format(user.TimeZone, nullValue: string.Empty),
                    TimeZone = timeZone
                })
                .ToList();


            return new SearchResultList(list);
        }

        protected override int SelectCount(ImpersonationFilter filter)
        {
            return ImpersonationSearch.Count(filter);
        }

        #endregion

        #region Data binding evaluation methods

        private string GetImpersonationName(Guid? contact, string name)
        {
            return contact.HasValue
                ? string.Format("<a href='/ui/admin/contacts/people/edit?contact={1}'>{0}</a>", name, contact)
                : name;
        }

        #endregion
    }
}