using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

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
            public bool IsInherited { get; set; }
            public string SourceProgramNames { get; set; }
        }

        protected DataFolder[] GetDataSource(Guid programId)
        {
            var achievementTypeMapping = Custom.CMDS.Common.Controls.Server.AchievementTypeSelector
                .CreateAchievementLabelMapping(CurrentSessionState.Identity.Organization.Code);

            var sourceNames = ProgramContainmentSearch
                .SelectParentTaskSources(programId)
                .GroupBy(x => x.ObjectIdentifier)
                .ToDictionary(
                    x => x.Key,
                    x => string.Join(", ", x.Select(y => y.ParentProgramName).Distinct().OrderBy(y => y)));

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
                        IsRequired = y.IsRequired,
                        IsInherited = y.IsInherited,
                        SourceProgramNames = y.IsInherited ? sourceNames.GetOrDefault(y.AchievementIdentifier) : null
                    })
                    .OrderBy(y => y.AchievementTitle)
                    .ToArray()
                })
                .OrderBy(x => x.AchievementLabel)
                .ToArray();
        }

        protected string GetInheritedBadge()
        {
            var item = Page.GetDataItem();

            if (!(bool)DataBinder.Eval(item, nameof(DataItem.IsInherited)))
                return string.Empty;

            var sources = (string)DataBinder.Eval(item, nameof(DataItem.SourceProgramNames));
            var title = sources.HasValue()
                ? $"Inherited from {sources}"
                : "Inherited from a parent program";

            return $"<span class='badge bg-info fs-sm ms-2' title='{HttpUtility.HtmlAttributeEncode(title)}'>Inherited</span>";
        }

        /// <summary>
        /// Binds a list of achievements where some are supplied by parent programs the
        /// new program will be linked to. Parent-supplied achievements render as
        /// inherited (badged, read-only) with the most-restrictive merged settings.
        /// </summary>
        protected DataFolder[] GetDataSource(IEnumerable<Guid> achievementIds, Guid[] parentProgramIds)
        {
            var achievementTypeMapping = Custom.CMDS.Common.Controls.Server.AchievementTypeSelector
                .CreateAchievementLabelMapping(CurrentSessionState.Identity.Organization.Code);

            var parentTasks = parentProgramIds.Length > 0
                ? TaskSearch.Select(x => parentProgramIds.Contains(x.ProgramIdentifier))
                : new List<InSite.Application.Records.Read.TTask>();

            var parentNames = parentProgramIds.Length > 0
                ? ProgramSearch.GetPrograms(parentProgramIds).ToDictionary(x => x.ProgramIdentifier, x => x.ProgramName)
                : new Dictionary<Guid, string>();

            var merged = parentTasks
                .GroupBy(t => t.ObjectIdentifier)
                .ToDictionary(g => g.Key, g => new
                {
                    IsRequired = g.Any(t => t.TaskIsRequired),
                    IsPlanned = g.Any(t => t.TaskIsPlanned),
                    LifetimeMonths = g.Min(t => t.TaskLifetimeMonths),
                    SourceProgramNames = string.Join(", ", g
                        .Select(t => parentNames.GetOrDefault(t.ProgramIdentifier))
                        .Where(n => n != null)
                        .Distinct()
                        .OrderBy(n => n))
                });

            var defaultLifetimes = ProgramTaskDefaults.GetLifetimeMonths(achievementIds);

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
                    Items = x.Select(y =>
                    {
                        var source = merged.GetOrDefault(y.AchievementIdentifier);

                        // A parent-supplied achievement inherits the merged parent settings.
                        // Anything else is a new task, so it starts from the program-task baseline.
                        var lifetime = source != null
                            ? source.LifetimeMonths
                            : ProgramTaskDefaults.GetLifetimeMonths(y.AchievementIdentifier, defaultLifetimes);

                        return new DataItem
                        {
                            AchievementIdentifier = y.AchievementIdentifier,
                            AchievementTitle = y.AchievementTitle,
                            IsTimeSensitive = lifetime != null,
                            LifetimeMonths = lifetime,
                            IsPlanned = source?.IsPlanned ?? ProgramTaskDefaults.IsPlanned,
                            IsRequired = source?.IsRequired ?? ProgramTaskDefaults.IsRequired,
                            IsInherited = source != null,
                            SourceProgramNames = source?.SourceProgramNames
                        };
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

            var defaultLifetimes = ProgramTaskDefaults.GetLifetimeMonths(achievementIds);

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
                    Items = x.Select(y =>
                    {
                        var lifetime = ProgramTaskDefaults.GetLifetimeMonths(y.AchievementIdentifier, defaultLifetimes);

                        return new DataItem
                        {
                            AchievementIdentifier = y.AchievementIdentifier,
                            AchievementTitle = y.AchievementTitle,
                            IsTimeSensitive = lifetime != null,
                            LifetimeMonths = lifetime,
                            IsPlanned = ProgramTaskDefaults.IsPlanned,
                            IsRequired = ProgramTaskDefaults.IsRequired
                        };
                    })
                    .OrderBy(y => y.AchievementTitle)
                    .ToArray()
                })
                .OrderBy(x => x.AchievementLabel)
                .ToArray();
        }
    }
}
