using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Records.Programs.Controls
{
    public abstract class TaskGridBase : BaseUserControl
    {
        protected class DataFolder
        {
            public string AchievementLabel { get; set; }
            public DataItem[] Items { get; set; }
        }

        protected class DataItem
        {
            public Guid AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public bool IsTimeSensitive { get; set; }
            public int? LifetimeMonths { get; set; }
            public bool IsPlanned { get; set; }
            public bool IsRequired { get; set; }
        }

        protected DataFolder[] GetDataSource(Guid programId)
        {
            var achievementTypeMapping = Custom.CMDS.Common.Controls.Server.AchievementTypeSelector
                .CreateAchievementLabelMapping(CurrentSessionState.Identity.Organization.Code);

            return TaskSearch
                .SelectByProgram(programId)
                .GroupBy(x => x.AchievementLabel)
                .Select(x => new DataFolder
                {
                    AchievementLabel = achievementTypeMapping.GetOrDefault(x.Key, x.Key),
                    Items = x.Select(y => new DataItem
                    {
                        AchievementIdentifier = y.AchievementIdentifier,
                        AchievementTitle = y.AchievementTitle,
                        IsTimeSensitive = y.LifetimeMonths.HasValue,
                        LifetimeMonths = y.LifetimeMonths,
                        IsPlanned = y.IsPlanned,
                        IsRequired = y.IsRequired
                    })
                    .OrderBy(y => y.AchievementTitle)
                    .ToArray()
                })
                .OrderBy(x => x.AchievementLabel)
                .ToArray();
        }

        protected DataFolder[] GetDataSource(IEnumerable<Guid> achievementIds)
        {
            var achievementTypeMapping = Custom.CMDS.Common.Controls.Server.AchievementTypeSelector
                .CreateAchievementLabelMapping(CurrentSessionState.Identity.Organization.Code);

            return VCmdsAchievementSearch.Select(x => achievementIds.Contains(x.AchievementIdentifier))
                .Select(x => new
                {
                    x.AchievementIdentifier,
                    x.AchievementLabel,
                    x.AchievementTitle
                })
                .ToList()
                .GroupBy(x => x.AchievementLabel)
                .Select(x => new DataFolder
                {
                    AchievementLabel = achievementTypeMapping.GetOrDefault(x.Key, x.Key),
                    Items = x.Select(y => new DataItem
                    {
                        AchievementIdentifier = y.AchievementIdentifier,
                        AchievementTitle = y.AchievementTitle,
                        IsTimeSensitive = false,
                        LifetimeMonths = (int?)null,
                        IsPlanned = false,
                        IsRequired = false
                    })
                    .OrderBy(y => y.AchievementTitle)
                    .ToArray()
                })
                .OrderBy(x => x.AchievementLabel)
                .ToArray();
        }
    }
}