using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using AngleSharp.Text;

using InSite.Application.Records.Read;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Records.Logbooks.Controls
{
    public partial class CompetencyProgressGrid : UserControl
    {
        private class CompetencyProgressItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }
            public decimal? RequiredHours { get; set; }
            public decimal CompletedHours { get; set; }
            public decimal Hours { get; set; }
            public int? RequiredJournalItems { get; set; }
            public int CompletedJournalItems { get; set; }
            public int JournalItems { get; set; }
            public ExperienceCompetencySatisfactionLevel SatisfactionLevel { get; set; }
            public int? SkillRating { get; set; }
        }

        private class AreaProgressItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }

            public List<CompetencyProgressItem> Competencies { get; set; }
        }

        public bool LoadData(Guid journalSetupIdentifier, Guid frameworkIdentifier, Guid userIdentifier)
        {
            var standard = StandardSearch.BindFirst(x => x, x => x.StandardIdentifier == frameworkIdentifier);
            FrameworkTitle.Text = !string.IsNullOrEmpty(standard?.ContentTitle)
                ? $"<a href=\"/ui/admin/standards/edit?id={standard.StandardIdentifier}\">{standard.ContentTitle} </a>"
                : "None";

            var areas = ReadAreas(journalSetupIdentifier, frameworkIdentifier, userIdentifier);
            if (areas == null)
            {
                AreaRepeater.Visible = false;
                return false;
            }

            AreaRepeater.Visible = true;
            AreaRepeater.ItemDataBound += AreaRepeater_ItemDataBound;
            AreaRepeater.DataSource = areas;
            AreaRepeater.DataBind();

            return true;
        }

        private void AreaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var area = (AreaProgressItem)e.Item.DataItem;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = area.Competencies;
            competencyRepeater.ItemDataBound += CompetencyRepeater_ItemDataBound;
            competencyRepeater.DataBind();
        }

        private void CompetencyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var competency = (CompetencyProgressItem)e.Item.DataItem;

            var hoursText = competency.RequiredHours.HasValue
                ? $"<b>{competency.CompletedHours:n2}</b> of <b>{competency.RequiredHours:n2}</b>"
                : $"<b>{competency.CompletedHours:n2}</b>";

            var hoursLiteral = (Literal)e.Item.FindControl("Hours");
            hoursLiteral.Text = hoursText;

            var journalItemsText = competency.RequiredJournalItems.HasValue
                ? $"<b>{competency.CompletedJournalItems}</b> of <b>{competency.RequiredJournalItems}</b>"
                : $"<b>{competency.CompletedJournalItems}</b>";

            var journalItemsLiteral = (Literal)e.Item.FindControl("JournalItems");
            journalItemsLiteral.Text = journalItemsText;
        }

        private static List<AreaProgressItem> ReadAreas(Guid journalSetupIdentifier, Guid frameworkIdentifier, Guid userIdentifier)
        {
            var areas = CompetencyHelper.GetAreas(journalSetupIdentifier, frameworkIdentifier, true);
            if (areas == null)
                return null;

            var userCompetencies = ServiceLocator.JournalSearch.GetExperienceCompetencies(
            new QExperienceCompetencyFilter
            {
                JournalSetupIdentifier = journalSetupIdentifier,
                UserIdentifier = userIdentifier
            },
            x => x.Experience)
                .GroupBy(x => x.CompetencyStandardIdentifier)
                .Select(AggregateCompetency)
                .ToDictionary(x => x.Identifier);

            return areas
                .Select(x => new AreaProgressItem
                {
                    Identifier = x.Identifier,
                    Sequence = x.Sequence,
                    Name = x.Name,
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
                            SatisfactionLevel = userCompetency?.SatisfactionLevel ?? ExperienceCompetencySatisfactionLevel.None,
                            SkillRating = y.SkillRating
                        };
                    })
                    .ToList()
                })
                .ToList();
        }

        private static CompetencyProgressItem AggregateCompetency(IGrouping<Guid, QExperienceCompetency> g)
        {
            var ordered = g.OrderByDescending(c => c.Experience.ExperienceCreated);

            var level = ordered
                .Select(c => c.SatisfactionLevel.ToEnum(ExperienceCompetencySatisfactionLevel.None))
                .FirstOrDefault(v => v != ExperienceCompetencySatisfactionLevel.None);

            if (level == default) 
                level = ExperienceCompetencySatisfactionLevel.None;

            return new CompetencyProgressItem
            {
                Identifier = g.Key,
                JournalItems = g.Count(),
                Hours = g.Sum(y => y.CompetencyHours ?? 0),
                SatisfactionLevel = level
            };
        }
    }
}