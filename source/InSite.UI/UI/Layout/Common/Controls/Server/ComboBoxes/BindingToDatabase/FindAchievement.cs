using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;

namespace InSite.Common.Web.UI
{
    public class FindAchievement : BaseFindEntity<QAchievementFilter>
    {
        #region Properties

        public QAchievementFilter Filter
        {
            get
            {
                var organization = CurrentSessionState.Identity.Organization;

                var filter = (QAchievementFilter)ViewState[nameof(Filter)];

                if (filter == null)
                {
                    filter = new QAchievementFilter(organization.Identifier);

                    var partition = ServiceLocator.Partition;

                    if (partition.IsE03())
                    {
                        var global = ServiceLocator.AppSettings.Application.Organizations.Global;

                        filter.OrganizationIdentifiers.Add(global);
                    }
                }

                ViewState[nameof(Filter)] = filter;

                return filter;
            }
            set
            {
                ViewState[nameof(Filter)] = value;
            }
        }

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