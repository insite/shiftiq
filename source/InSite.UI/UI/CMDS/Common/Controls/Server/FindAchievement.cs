using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class FindAchievement : BaseFindEntity<VCmdsAchievementFilter>
    {
        #region Properties

        public VCmdsAchievementFilter Filter => (VCmdsAchievementFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new VCmdsAchievementFilter()));

        public bool IncludeUserAchievement
        {
            get { return (bool?)ViewState[nameof(IncludeUserAchievement)] ?? false; }
            set { ViewState[nameof(IncludeUserAchievement)] = value; }
        }

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
            return VCmdsAchievementSearch.CountForSelector(filter, IncludeUserAchievement);
        }

        protected override DataItem[] Select(VCmdsAchievementFilter filter)
        {
            var table = VCmdsAchievementSearch.SelectForSelector(filter, IncludeUserAchievement);

            return table.Rows.Cast<DataRow>().Select(GetDataItem).ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            var filter = GetFilter((string)null);
            var table = VCmdsAchievementSearch.SelectForSelector(ids, filter, IncludeUserAchievement);

            return table.Rows.Cast<DataRow>().Select(GetDataItem).ToArray();
        }

        private static DataItem GetDataItem(DataRow x) => new DataItem
        {
            Value = (Guid)x["Value"],
            Text = (string)x["Text"],
        };
    }
}