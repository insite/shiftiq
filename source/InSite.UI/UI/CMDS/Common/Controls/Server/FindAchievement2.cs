using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class FindAchievement2 : BaseFindEntity<VCmdsAchievementFilter>
    {
        #region Properties

        public VCmdsAchievementFilter Filter => (VCmdsAchievementFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new VCmdsAchievementFilter()));

        #endregion

        protected override string GetEntityName() => "Achievement";

        protected override VCmdsAchievementFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.OrganizationCode = CurrentSessionState.Identity.Organization.Code;
            filter.Title = keyword;

            return filter;
        }

        protected override int Count(VCmdsAchievementFilter filter)
        {
            return VCmdsAchievementSearch.CountSearchResults(filter);
        }

        protected override DataItem[] Select(VCmdsAchievementFilter filter)
        {
            var table = VCmdsAchievementSearch.SelectSearchResults(filter, false);

            return table.Rows.Cast<DataRow>().Select(GetDataItem).ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            var filter = GetFilter((string)null);
            var table = VCmdsAchievementSearch.SelectSearchResults(ids, filter, false);

            return table.Rows.Cast<DataRow>().Select(GetDataItem).ToArray();
        }

        private static DataItem GetDataItem(DataRow x) => new DataItem
        {
            Value = (Guid)x["AchievementIdentifier"],
            Text = (string)x["SelectorText"],
        };
    }
}