using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;

namespace InSite.Common.Web.UI
{
    public class FindAchievement : BaseFindEntity<QAchievementFilter>
    {
        #region Properties

        public QAchievementFilter Filter => (QAchievementFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QAchievementFilter(CurrentSessionState.Identity.Organization.Identifier)));

        #endregion

        protected override string GetEntityName() => "Achievement";

        protected override QAchievementFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.AchievementTitle = keyword;

            return filter;
        }

        protected override int Count(QAchievementFilter filter)
        {
            return ServiceLocator.AchievementSearch.CountAchievements(filter);
        }

        protected override DataItem[] Select(QAchievementFilter filter)
        {
            return ServiceLocator.AchievementSearch.GetAchievements(filter)
                .Select(x => new DataItem
                {
                    Value = x.AchievementIdentifier,
                    Text = x.AchievementTitle,
                })
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ServiceLocator.AchievementSearch.GetAchievements(ids)
                .Select(x => new DataItem
                {
                    Value = x.AchievementIdentifier,
                    Text = x.AchievementTitle,
                });
        }
    }
}