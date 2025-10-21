using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.UI.Portal.Records.Logbooks.Models;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Logbooks.Controls
{
    public partial class CompetencyProgressGrid : BaseUserControl
    {
        private class CompetencyProgressItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }
            public decimal? RequiredHours { get; set; }
            public decimal CompletedHours { get; set; }
            public int? RequiredJournalItems { get; set; }
            public int CompletedJournalItems { get; set; }
            public int? SkillRating { get; set; }
            public bool IncludeHoursToArea { get; set; }
            public ExperienceCompetencySatisfactionLevel SatisfactionLevel { get; set; }
        }

        private class AreaProgressItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }
            public decimal? RequiredHours { get; set; }

            public bool HasRequiredHours => RequiredHours.HasValue
                || Competencies.Any(x => x.IncludeHoursToArea && x.RequiredHours.HasValue && x.RequiredHours.Value > 0);

            public decimal HoursCompleted
            {
                get
                {
                    if (Competencies.Count == 0)
                        return 0;

                    decimal required = 0, completed = 0;

                    if (!RequiredHours.HasValue)
                    {
                        foreach (var competency in Competencies)
                        {
                            if (!competency.IncludeHoursToArea || (competency.RequiredHours ?? 0) <= 0)
                                continue;

                            required += competency.RequiredHours.Value;
                            completed += competency.CompletedHours;
                        }
                    }
                    else
                    {
                        required = RequiredHours.Value;
                        completed = Competencies.Where(x => x.IncludeHoursToArea).Sum(x => x.CompletedHours);
                    }

                    if (completed > required)
                        completed = required;

                    return completed > 0 ? Calculator.GetPercentDecimal(completed, required) : 0;
                }
            }

            public List<CompetencyProgressItem> Competencies { get; set; }
        }

        protected bool ShowSkillRating { get; set; }

        public bool LoadData(Guid journalSetupIdentifier, Guid frameworkIdentifier, Guid userIdentifier)
        {
            var areas = ReadAreas(journalSetupIdentifier, frameworkIdentifier, userIdentifier, Identity.Language);
            if (areas == null)
            {
                AreaRepeater.Visible = false;
                return false;
            }

            ShowSkillRating = areas.SelectMany(x => x.Competencies).Any(x => x.SkillRating.HasValue);

            AreaRepeater.Visible = true;
            AreaRepeater.ItemDataBound += AreaRepeater_ItemDataBound;
            AreaRepeater.DataSource = areas;
            AreaRepeater.DataBind();

            return true;
        }

        private void AreaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var area = (AreaProgressItem)e.Item.DataItem;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = area.Competencies;
            competencyRepeater.ItemDataBound += CompetencyRepeater_ItemDataBound;
            competencyRepeater.DataBind();
        }

        private void CompetencyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var competency = (CompetencyProgressItem)e.Item.DataItem;

            var hoursText = (competency.RequiredHours.HasValue && competency.RequiredHours.Value > 0)
                ? GenerateUserProgressCompetencyHours(competency)
                : $"<b>{competency.CompletedHours:n2}</b>";

            var hoursLiteral = (ITextControl)e.Item.FindControl("Hours");
            hoursLiteral.Text = hoursText;

            var journalItemsText = competency.RequiredJournalItems.HasValue
                ? $"<b>{competency.CompletedJournalItems}</b> of <b>{competency.RequiredJournalItems}</b>"
                : $"<b>{competency.CompletedJournalItems}</b>";

            var journalItemsLiteral = (ITextControl)e.Item.FindControl("JournalItems");
            journalItemsLiteral.Text = journalItemsText;
        }

        private static string GenerateUserProgressCompetencyHours(CompetencyProgressItem competency) =>
            GetUserProgressHoursHtml(competency.CompletedHours, competency.RequiredHours.Value);

        private static string GetUserProgressHoursHtml(decimal value, decimal total)
        {
            var completedPercentage = value >= total ? 100 : (value / total) * 100;

            return $"<div><b>{value:n2}</b> of <b>{total:n2}</b></div>" +
                    $"<div class='progress'>" +
                        $"<div class='progress-bar' role='progressbar' style='width:{completedPercentage}%' aria-valuenow='{completedPercentage}' aria-valuemin='0' aria-valuemax='100'></div>" +
                    $"</div>";
        }

        private static List<AreaProgressItem> ReadAreas(Guid journalSetupIdentifier, Guid frameworkIdentifier, Guid userIdentifier, string language)
        {
            var areas = CompetencyHelper.GetAreas(journalSetupIdentifier, language, true);
            if (areas == null)
                return null;

            var userCompetencies = ServiceLocator.JournalSearch
                .GetExperienceCompetencies(
                    new QExperienceCompetencyFilter
                    {
                        JournalSetupIdentifier = journalSetupIdentifier,
                        UserIdentifier = userIdentifier
                    },
                    x => x.Experience)
                .GroupBy(x => x.CompetencyStandardIdentifier)
                .Select(g =>
                {
                    var ordered = g.OrderByDescending(c => c.Experience.ExperienceCreated).ToList();

                    var satisfactionLevelExpList = ordered
                        .Select(c => c.SatisfactionLevel.ToEnum(ExperienceCompetencySatisfactionLevel.None))
                        .ToList();

                    ExperienceCompetencySatisfactionLevel value;

                    if (ordered.Count > 1)
                    {
                        value = ordered
                            .Zip(satisfactionLevelExpList, (c, v) => new { c.Experience.ExperienceCreated, Value = v })
                            .OrderByDescending(z => z.ExperienceCreated)
                            .Select(z => z.Value)
                            .FirstOrDefault(v => v != ExperienceCompetencySatisfactionLevel.None);
                    }
                    else
                        value = satisfactionLevelExpList[0];

                    return new
                    {
                        Identifier = g.Key,
                        JournalItems = g.Count(),
                        Hours = g.Sum(c => c.CompetencyHours ?? 0m),
                        SatisfactionLevel = value
                    };
                })
                .ToDictionary(x => x.Identifier);

            var areaRequirements = ServiceLocator.JournalSearch.GetAreaRequirements(journalSetupIdentifier)
                .Where(x => x.AreaHours.HasValue && x.AreaHours.Value > 0)
                .ToDictionary(x => x.AreaStandardIdentifier, x => x);

            return areas
                .Select(x => new AreaProgressItem
                {
                    Identifier = x.Identifier,
                    Sequence = x.Sequence,
                    Name = x.Name,
                    RequiredHours = areaRequirements.GetOrDefault(x.Identifier)?.AreaHours,
                    Competencies = x.Competencies.Select(y =>
                    {
                        if (!userCompetencies.TryGetValue(y.Identifier, out var userCompetency))
                            userCompetency = null;

                        return new CompetencyProgressItem
                        {
                            Identifier = y.Identifier,
                            Sequence = y.Sequence,
                            Name = y.Name,
                            RequiredHours = y.Hours,
                            CompletedHours = userCompetency?.Hours ?? 0,
                            RequiredJournalItems = y.JournalItems,
                            CompletedJournalItems = userCompetency?.JournalItems ?? 0,
                            SatisfactionLevel = userCompetency == null
                                ? ExperienceCompetencySatisfactionLevel.None
                                : userCompetency.SatisfactionLevel,
                            SkillRating = y.SkillRating,
                            IncludeHoursToArea = y.IncludeHoursToArea
                        };
                    })
                    .ToList()
                })
                .ToList();
        }

        protected static int GetSatisfactionLevel(ExperienceCompetencySatisfactionLevel value)
        {
            switch (value)
            {
                case ExperienceCompetencySatisfactionLevel.None: return 0;
                case ExperienceCompetencySatisfactionLevel.NotSatisfied: return 1;
                case ExperienceCompetencySatisfactionLevel.PartiallySatisfied: return 2;
                case ExperienceCompetencySatisfactionLevel.Satisfied: return 3;
                default: throw new ArgumentException($"Unsupported satisfaction level: {value.GetName()}");
            }
        }

        protected string EvalSatisfactionLevelHtml(string expression)
        {
            var dataItem = Page.GetDataItem();
            var value = (ExperienceCompetencySatisfactionLevel)DataBinder.Eval(dataItem, expression);

            string @class, text;

            switch (value)
            {
                case ExperienceCompetencySatisfactionLevel.None:
                    return string.Empty;
                case ExperienceCompetencySatisfactionLevel.NotSatisfied:
                    @class = "danger";
                    text = "Not Satisfied";
                    break;
                case ExperienceCompetencySatisfactionLevel.PartiallySatisfied:
                    @class = "warning";
                    text = "Partially Satisfied";
                    break;
                case ExperienceCompetencySatisfactionLevel.Satisfied:
                    @class = "success";
                    text = "Satisfied";
                    break;
                default:
                    throw new ArgumentException($"Unsupported satisfaction level: {value}");
            }

            return $"<span class='badge bg-{@class}'>{text}</span>";
        }
    }
}