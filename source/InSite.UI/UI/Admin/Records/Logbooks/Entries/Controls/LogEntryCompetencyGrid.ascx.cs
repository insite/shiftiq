using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Records.Logbooks;
using InSite.Application.Records.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.Entries.Controls
{
    public partial class LogEntryCompetencyGrid : UserControl
    {
        private class CompetencyItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }
            public decimal? Hours { get; set; }
            public ExperienceCompetencySatisfactionLevel SatisfactionLevel { get; set; }
            public int? SkillRating { get; set; }
            public string DeleteUrl { get; set; }

            public string SatisfactionLevelName => SatisfactionLevel.GetDescription();
        }

        private class AreaItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }

            public List<CompetencyItem> Competencies { get; set; }
        }

        private ReturnUrl _returnUrl;

        protected Guid ExperienceIdentifier { get; set; }

        private void AreaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var area = (AreaItem)e.Item.DataItem;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = area.Competencies;
            competencyRepeater.DataBind();
        }

        public bool LoadData(Guid experienceIdentifier, bool isValidator)
        {
            _returnUrl = new ReturnUrl();

            var areas = GetAreas(experienceIdentifier, isValidator);
            if (areas == null)
                return false;

            ExperienceIdentifier = experienceIdentifier;

            AreaRepeater.ItemDataBound += AreaRepeater_ItemDataBound;
            AreaRepeater.DataSource = areas;
            AreaRepeater.DataBind();

            return true;
        }

        private List<AreaItem> GetAreas(Guid experienceIdentifier, bool isValidator)
        {
            var experienceCompetencies = ServiceLocator.JournalSearch.GetExperienceCompetencies(experienceIdentifier, x => x.Competency);
            if (experienceCompetencies.Count == 0)
                return null;

            var areas = new List<AreaItem>();

            var classifications = StandardSearch.GetAllTypeNames(CurrentSessionState.Identity.Organization.Identifier);

            foreach (var experienceCompetency in experienceCompetencies)
                AddCompetency(experienceCompetency, classifications, areas, isValidator);

            areas = areas.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();
            foreach (var area in areas)
                area.Competencies = area.Competencies.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();

            return areas;
        }

        private void AddCompetency(QExperienceCompetency experienceCompetency, string[] classifications, List<AreaItem> areas, bool isValidator)
        {
            var competency = experienceCompetency.Competency;

            var area = areas.Find(x => x.Identifier == competency.AreaIdentifier);
            if (area == null)
            {
                if (competency.AreaIdentifier == null)
                    return;

                var areaClassification = competency.AreaStandardType;

                area = new AreaItem
                {
                    Identifier = competency.AreaIdentifier.Value,
                    Sequence = competency.AreaSequence.Value,
                    Name = CompetencyHelper.GetStandardName(
                        competency.AreaIdentifier.Value,
                        competency.AreaAsset.Value,
                        competency.AreaLabel,
                        competency.AreaCode,
                        areaClassification
                    ),
                    Competencies = new List<CompetencyItem>()
                };

                areas.Add(area);
            }

            var classification = "Competency";

            var deleteUrlTemplate = isValidator
                ? "/admin/records/logbooks/validators/competencies/delete?experience={0}&competency={1}"
                : "/admin/records/logbooks/competencies/delete?experience={0}&competency={1}";

            var deleteUrl = _returnUrl
                .GetRedirectUrl(string.Format(deleteUrlTemplate, experienceCompetency.ExperienceIdentifier, competency.CompetencyIdentifier),
                "panel=competencies");

            var competencyItem = new CompetencyItem
            {
                Identifier = competency.CompetencyIdentifier,
                Sequence = competency.CompetencySequence,
                Name = CompetencyHelper.GetStandardName(
                    competency.CompetencyIdentifier,
                    competency.CompetencyAsset,
                    competency.CompetencyLabel,
                    competency.CompetencyCode,
                    classification
                ),
                Hours = experienceCompetency.CompetencyHours,
                SatisfactionLevel = experienceCompetency.SatisfactionLevel.ToEnum(ExperienceCompetencySatisfactionLevel.None),
                SkillRating = experienceCompetency.SkillRating,
                DeleteUrl = deleteUrl
            };

            area.Competencies.Add(competencyItem);
        }

    }
}