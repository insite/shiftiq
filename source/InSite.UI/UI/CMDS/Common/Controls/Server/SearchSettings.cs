using System;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class SearchSettings
    {
        public SearchSettings()
        {
        }

        public SearchSettings(Shift.Common.Filter filter, int pageIndex = 0)
            : this()
        {
            Filter = filter;
            PageIndex = pageIndex;
        }

        public SearchSettings(Shift.Common.Filter filter, int pageIndex, SearchSort sortExpression, DateTimeOffset? lastSearched)
            : this(filter, pageIndex)
        {
            SortExpression = sortExpression;
            LastSearched = lastSearched;
        }

        public SearchSettings(Shift.Common.Filter filter, int pageIndex, string sortExpression, SortOrder sortOrder, DateTimeOffset? lastSearched)
            : this(filter, pageIndex, SearchSort.Create(sortExpression, sortOrder), lastSearched)
        {
        }

        public Shift.Common.Filter Filter { get; set; }
        public int PageIndex { get; set; }
        public SearchSort SortExpression { get; set; }
        public DateTimeOffset? LastSearched { get; set; }

        public static SearchSettings Load(Page page)
        {
            var actionName = ((IHasWebRoute)page).Route.Name;

            return Load(actionName);
        }

        public static SearchSettings Load(string name)
        {
            try
            {
                return PersonalizationRepository.GetValue<SearchSettings>(Guid.Empty, CurrentSessionState.Identity.User.UserIdentifier,
                    "cmds:" + name, false);
            }
            catch (JsonSerializationException)
            {
                return null;
            }
        }

        public void Save(Page page)
        {
            var actionName = ((IAdminPage)page).Route.Name;

            if (LastSearched == null)
                LastSearched = DateTimeOffset.UtcNow;

            Save(actionName);
        }

        public void Save(string name)
        {
            PersonalizationRepository.SetValue(Guid.Empty, CurrentSessionState.Identity.User.UserIdentifier, "cmds:" + name, this);
        }
    }
}