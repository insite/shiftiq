using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Persistence;

namespace InSite.Admin.Records.Logbooks.Controls
{
    public partial class UserCompetencySummary : UserControl
    {
        private class CompetencyProgressItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }
            public decimal Hours { get; set; }
            public int JournalItems { get; set; }
            public int? SkillRating { get; set; }
        }

        private class AreaProgressItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }

            public List<CompetencyProgressItem> Competencies { get; set; }
        }

        private class FrameworkProgressItem
        {
            public Guid Identifier { get; set; }
            public string Name { get; set; }
            public List<AreaProgressItem> Areas { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FrameworkRepeater.ItemDataBound += FrameworkRepeater_ItemDataBound;
        }

        public bool LoadData(Guid organizationIdentifier, Guid userIdentifier, Guid? validatorUserIdentifier)
        {
            var frameworks = ReadFrameworks(organizationIdentifier, userIdentifier, validatorUserIdentifier);

            FrameworkRepeater.DataSource = frameworks;
            FrameworkRepeater.DataBind();

            return frameworks.Any();
        }

        private void FrameworkRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var framework = (FrameworkProgressItem)e.Item.DataItem;

            var areaRepeater = (Repeater)e.Item.FindControl("AreaRepeater");
            areaRepeater.ItemDataBound += AreaRepeater_ItemDataBound;
            areaRepeater.DataSource = framework.Areas;
            areaRepeater.DataBind();
        }

        private void AreaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var area = (AreaProgressItem)e.Item.DataItem;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = area.Competencies;
            competencyRepeater.DataBind();
        }

        private static List<FrameworkProgressItem> ReadFrameworks(Guid organizationIdentifier, Guid userIdentifier, Guid? validatorUserIdentifier)
        {
            var frameworks = new List<FrameworkProgressItem>();

            var journalFilter = new QJournalFilter
            {
                OrganizationIdentifier = organizationIdentifier,
                UserIdentifier = userIdentifier,
                ValidatorUserIdentifier = validatorUserIdentifier
            };

            var journals = ServiceLocator.JournalSearch.GetJournals(journalFilter, null, null, x => x.JournalSetup);
            foreach (var journal in journals)
            {
                if (journal.JournalSetup.FrameworkStandardIdentifier == null)
                    continue;

                AddFramework(userIdentifier, journal.JournalSetupIdentifier, journal.JournalSetup.FrameworkStandardIdentifier.Value, frameworks);
            }

            foreach (var framework in frameworks)
            {
                framework.Areas = framework.Areas.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();

                foreach (var area in framework.Areas)
                    area.Competencies = area.Competencies.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();
            }

            return frameworks;
        }

        private static void AddFramework(
            Guid userIdentifier,
            Guid journalSetupIdentifier,
            Guid frameworkIdentifier,
            List<FrameworkProgressItem> frameworks
            )
        {
            var areas = ReadAreas(journalSetupIdentifier, frameworkIdentifier, userIdentifier);
            if (areas == null)
                return;

            var framework = frameworks.Find(x => x.Identifier == frameworkIdentifier);
            if (framework == null)
            {
                var standard = StandardSearch.BindFirst(x => x, x => x.StandardIdentifier == frameworkIdentifier);
                frameworks.Add(framework = new FrameworkProgressItem
                {
                    Identifier = frameworkIdentifier,
                    Name = standard.ContentTitle,
                    Areas = areas
                });
            }
            else
            {
                MergeAreas(framework, areas);
            }
        }

        private static void MergeAreas(FrameworkProgressItem framework, List<AreaProgressItem> areas)
        {
            foreach (var area in areas)
            {
                var existingArea = framework.Areas.Find(x => x.Identifier == area.Identifier);
                if (existingArea == null)
                {
                    framework.Areas.Add(area);
                    continue;
                }

                foreach (var competency in area.Competencies)
                {
                    var existingCompetency = existingArea.Competencies.Find(x => x.Identifier == competency.Identifier);
                    if (existingCompetency == null)
                    {
                        existingArea.Competencies.Add(competency);
                    }
                    else
                    {
                        existingCompetency.JournalItems += competency.JournalItems;
                        existingCompetency.Hours += competency.Hours;
                    }
                }
            }
        }

        private static List<AreaProgressItem> ReadAreas(Guid journalSetupIdentifier, Guid frameworkIdentifier, Guid userIdentifier)
        {
            var areas = CompetencyHelper.GetAreas(journalSetupIdentifier, frameworkIdentifier, true);
            if (areas == null)
                return null;

            var userCompetencies = ServiceLocator.JournalSearch.GetExperienceCompetencies(new QExperienceCompetencyFilter
                {
                    JournalSetupIdentifier = journalSetupIdentifier,
                    UserIdentifier = userIdentifier
                })
                .GroupBy(x => x.CompetencyStandardIdentifier)
                .Select(x => new
                {
                    Identifier = x.Key,
                    JournalItems = x.Count(),
                    Hours = x.Sum(y => y.CompetencyHours ?? 0)
                })
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
                            Hours = userCompetency?.Hours ?? 0,
                            JournalItems = userCompetency?.JournalItems ?? 0,
                            SkillRating = y.SkillRating
                        };
                    })
                    .ToList()
                })
                .ToList();
        }
    }
}