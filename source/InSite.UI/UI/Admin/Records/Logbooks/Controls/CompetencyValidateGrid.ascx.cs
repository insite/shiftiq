using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Application.Journals.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

using RadioButtonList = System.Web.UI.WebControls.RadioButtonList;

namespace InSite.Admin.Records.Logbooks.Controls
{
    public partial class CompetencyValidateGrid : UserControl
    {
        public bool ShowSkillRatings
        {
            get => (ViewState[nameof(ShowSkillRatings)] as bool?) ?? false;
            set => ViewState[nameof(ShowSkillRatings)] = value;
        }

        private class CompetencyItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }
            public decimal? Hours { get; set; }
            public ExperienceCompetencySatisfactionLevel SatisfactionLevel { get; set; }
            public int? SkillRating { get; set; }
            public bool SkillRatingRequired { get; set; }
        }

        private class AreaItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }

            public List<CompetencyItem> Competencies { get; set; }
        }

        protected Guid ExperienceIdentifier { get; set; }
        protected bool IsValidator { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            HandleAjaxRequest();

            base.OnLoad(e);
        }

        private void AreaRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Header)
                return;

            var th = (HtmlTableCell)e.Item.FindControl("SkillRatingHeader");
            if (th != null)
                th.Visible = ShowSkillRatings;
        }

        private void AreaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var area = (AreaItem)e.Item.DataItem;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.ItemDataBound += CompetencyRepeater_ItemDataBound;
            competencyRepeater.DataSource = area.Competencies;
            competencyRepeater.DataBind();
        }

        private void CompetencyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var td = (HtmlTableCell)e.Item.FindControl("SkillRatingData");
            if (td != null)
                td.Visible = ShowSkillRatings;

            var competency = (CompetencyItem)e.Item.DataItem;

            var satisfactionLevel = (RadioButtonList)e.Item.FindControl("SatisfactionLevel");
            satisfactionLevel.Items.Add(new System.Web.UI.WebControls.ListItem("None", ExperienceCompetencySatisfactionLevel.None.ToString()));
            satisfactionLevel.Items.Add(new System.Web.UI.WebControls.ListItem("Not Satisfied", ExperienceCompetencySatisfactionLevel.NotSatisfied.ToString()));
            satisfactionLevel.Items.Add(new System.Web.UI.WebControls.ListItem("Partially Satisfied", ExperienceCompetencySatisfactionLevel.PartiallySatisfied.ToString()));
            satisfactionLevel.Items.Add(new System.Web.UI.WebControls.ListItem("Satisfied", ExperienceCompetencySatisfactionLevel.Satisfied.ToString()));
            satisfactionLevel.SelectedValue = competency.SatisfactionLevel.ToString();

            var skillRating = (ComboBox)e.Item.FindControl("SkillRating");
            skillRating.LoadItems(Enumerable.Range(1, 5).Select(x => x.ToString()));

            var skillRatingUnavailable = (System.Web.UI.WebControls.Literal)e.Item.FindControl("SkillRatingUnavailable");

            if (competency.SkillRatingRequired)
                skillRating.ValueAsInt = competency.SkillRating;
            else
            {
                skillRating.Visible = false;
                skillRatingUnavailable.Visible = true;
            }

        }

        public bool LoadData(Guid experienceIdentifier, Guid? journalSetupIdentifier, bool isValidator)
        {
            var areas = GetAreas(experienceIdentifier, journalSetupIdentifier);
            if (areas == null)
                return false;

            ExperienceIdentifier = experienceIdentifier;
            IsValidator = isValidator;

            ShowSkillRatings = areas.Any(area => area.Competencies.Any(comp => comp.SkillRatingRequired));

            AreaRepeater.ItemCreated += AreaRepeater_ItemCreated;
            AreaRepeater.ItemDataBound += AreaRepeater_ItemDataBound;
            AreaRepeater.DataSource = areas;
            AreaRepeater.DataBind();

            return true;
        }

        private static List<AreaItem> GetAreas(Guid experienceIdentifier, Guid? journalSetupIdentifier)
        {
            var experienceCompetencies = ServiceLocator.JournalSearch.GetExperienceCompetencies(experienceIdentifier, x => x.Competency);
            if (experienceCompetencies.Count == 0)
                return null;

            List<QCompetencyRequirement> requirements = journalSetupIdentifier.HasValue
                ? ServiceLocator.JournalSearch.GetCompetencyRequirements(journalSetupIdentifier.Value)
                : null;

            var areas = new List<AreaItem>();

            var classifications = StandardSearch.GetAllTypeNames(CurrentSessionState.Identity.Organization.Identifier);

            foreach (var experienceCompetency in experienceCompetencies)
                AddCompetency(experienceCompetency, classifications, areas,
                    (requirements != null ? requirements.FirstOrDefault(x => x.CompetencyStandardIdentifier == experienceCompetency.Competency.CompetencyIdentifier) : null));

            areas = areas.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();
            foreach (var area in areas)
                area.Competencies = area.Competencies.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();

            return areas;
        }

        private static void AddCompetency(QExperienceCompetency experienceCompetency, string[] classifications, List<AreaItem> areas, QCompetencyRequirement qCompetencyRequirement = null)
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
                SkillRatingRequired = qCompetencyRequirement != null ? qCompetencyRequirement.SkillRating.HasValue : false
            };

            area.Competencies.Add(competencyItem);
        }

        private void HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest)
                return;

            var action = Page.Request.Form["action"];
            if (string.IsNullOrEmpty(action))
                return;

            var experienceIdentifier = Guid.Parse(Page.Request.Form["experience"]);

            var experience = ServiceLocator.JournalSearch.GetExperience(experienceIdentifier, x => x.Journal.JournalSetup);
            if (experience == null
                || experience.Journal.JournalSetup.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier
                )
            {
                return;
            }

            var competencyIdentifier = Guid.Parse(Page.Request.Form["competency"]);

            switch (action)
            {
                case "change-satisfaction":
                    ChangeSatisfaction(experience.JournalIdentifier, experienceIdentifier, competencyIdentifier);
                    break;
                case "change-skill-rating":
                    ChangeSkillRating(experience.JournalIdentifier, experienceIdentifier, competencyIdentifier);
                    break;
            }

            Response.Clear();
            Response.Write("OK");
            Response.End();
        }

        private void ChangeSatisfaction(Guid journalIdentifier, Guid experienceIdentifier, Guid competencyIdentifier)
        {
            var satisfactionLevel = Page.Request.Form["value"].ToEnum<ExperienceCompetencySatisfactionLevel>();

            ServiceLocator.SendCommand(new ChangeExperienceCompetencySatisfactionLevel(journalIdentifier, experienceIdentifier, competencyIdentifier, satisfactionLevel));
        }

        private void ChangeSkillRating(Guid journalIdentifier, Guid experienceIdentifier, Guid competencyIdentifier)
        {
            var skillRating = !string.IsNullOrEmpty(Page.Request.Form["value"])
                ? int.Parse(Page.Request.Form["value"])
                : (int?)null;

            ServiceLocator.SendCommand(new ChangeExperienceCompetencySkillRating(journalIdentifier, experienceIdentifier, competencyIdentifier, skillRating));
        }
    }
}