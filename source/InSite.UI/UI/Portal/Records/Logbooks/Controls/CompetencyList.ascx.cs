using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Journals.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.UI.Portal.Records.Logbooks.Models;

using Shift.Sdk.UI;

namespace InSite.UI.Portal.Records.Logbooks.Controls
{
    public partial class CompetencyList : BaseUserControl
    {
        private decimal? MaxHours
        {
            get => (decimal?)ViewState[nameof(MaxHours)];
            set => ViewState[nameof(MaxHours)] = value;
        }

        public void SetMaxHours(decimal? maxHours)
        {
            MaxHours = maxHours;
        }

        public Guid? StandardIdentifier
        {
            get { return (Guid?)ViewState[nameof(StandardIdentifier)]; }
            set { ViewState[nameof(StandardIdentifier)] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemCreated += AreaRepeater_ItemCreated;

            RefreshButton.Click += RefreshButton_Click;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            if (StandardIdentifier.HasValue)
                LoadData(StandardIdentifier.Value, null);
        }

        private void AreaRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.ItemCreated += CompetencyRepeater_ItemCreated;
        }

        private void CompetencyRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var hoursValidator = (Common.Web.UI.CustomValidator)e.Item.FindControl("HoursValidator");
            hoursValidator.ServerValidate += HoursValidator_ServerValidate;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (RepeaterItem areaRepeaterItem in Repeater.Items)
            {
                var competencyRepeater = (Repeater)areaRepeaterItem.FindControl("CompetencyRepeater");

                foreach (RepeaterItem competencyRepeaterItem in competencyRepeater.Items)
                {
                    var selectedCheckBox = (ICheckBoxControl)competencyRepeaterItem.FindControl("Selected");
                    var hoursInput = (INumericBox)competencyRepeaterItem.FindControl("Hours");
                    hoursInput.Enabled = selectedCheckBox.Checked;
                }
            }
        }

        private void HoursValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var item = (RepeaterItem)((Control)source).NamingContainer;
            var isSelected = (ICheckBoxControl)item.FindControl("Selected");

            args.IsValid = !isSelected.Checked
                || MaxHours == null
                || decimal.TryParse(args.Value, out var hours) && hours <= MaxHours;

            if (args.IsValid)
                return;

            var template = Translate("Number of hours in competency should be less or equal to {0:n2}");

            var hoursValidators = (Common.Web.UI.CustomValidator)source;
            hoursValidators.ErrorMessage = string.Format(template, MaxHours);
        }

        public int LoadData(Guid journalSetupIdentifier, Guid? experienceIdentifier)
        {
            StandardIdentifier = journalSetupIdentifier;

            var areas = CompetencyHelper.GetAreas(journalSetupIdentifier, Identity.Language, true);
            if (areas == null)
                return 0;

            BindJournalItems(journalSetupIdentifier, areas);
            BindExperience(experienceIdentifier, areas);

            Repeater.ItemDataBound += Repeater_ItemDataBound;
            Repeater.DataSource = areas;
            Repeater.DataBind();

            return areas.Sum(x => x.Competencies.Count);
        }

        private static void BindJournalItems(Guid journalSetupIdentifier, List<CompetencyHelper.AreaItem> areas)
        {
            var userCompetencies = ServiceLocator.JournalSearch.GetExperienceCompetencies(new QExperienceCompetencyFilter
            {
                JournalSetupIdentifier = journalSetupIdentifier,
                UserIdentifier = CurrentSessionState.Identity.User.Identifier
            })
            .GroupBy(x => x.CompetencyStandardIdentifier)
            .Select(x => new
            {
                Identifier = x.Key,
                JournalItems = x.Count(),
            })
            .ToDictionary(x => x.Identifier);

            if (userCompetencies != null && userCompetencies.Count > 0)
            {
                foreach (var area in areas)
                {
                    foreach (var competency in area.Competencies)
                    {
                        if (!userCompetencies.TryGetValue(competency.Identifier, out var userCompetency))
                            userCompetency = null;

                        competency.JournalItems = userCompetency?.JournalItems ?? 0;
                    }
                }
            }
        }

        private static void BindExperience(Guid? experienceIdentifier, List<CompetencyHelper.AreaItem> areas)
        {
            if (experienceIdentifier.HasValue)
            {
                var experienceCompetencies = ServiceLocator.JournalSearch
                    .GetExperienceCompetencies(experienceIdentifier.Value)
                    .ToDictionary(x => x.CompetencyStandardIdentifier);

                foreach (var area in areas)
                {
                    foreach (var competency in area.Competencies)
                    {
                        if (experienceCompetencies.TryGetValue(competency.Identifier, out var experienceCompetency))
                        {
                            competency.Selected = true;
                            competency.Hours = experienceCompetency.CompetencyHours;
                        }
                    }
                }
            }
        }

        public void GetChanges(Guid journalIdentifier, Guid experienceIdentifier, List<Command> commands)
        {
            var selected = GetSelectedCompetencies();

            var existing = ServiceLocator.JournalSearch
                .GetExperienceCompetencies(experienceIdentifier)
                .Select(x => x.CompetencyStandardIdentifier)
                .ToHashSet();

            foreach (var competency in selected)
            {
                if (existing.Contains(competency.Identifier))
                    commands.Add(new ChangeExperienceCompetency(journalIdentifier, experienceIdentifier, competency.Identifier, competency.Hours));
                else
                    commands.Add(new AddExperienceCompetency(journalIdentifier, experienceIdentifier, competency.Identifier, competency.Hours));
            }

            foreach (var competency in existing)
            {
                if (!selected.Any(x => x.Identifier == competency))
                    commands.Add(new DeleteExperienceCompetency(journalIdentifier, experienceIdentifier, competency));
            }
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var area = (CompetencyHelper.AreaItem)e.Item.DataItem;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.ItemDataBound += CompetencyRepeater_ItemDataBound;
            competencyRepeater.DataSource = area.Competencies;
            competencyRepeater.DataBind();
        }

        private void CompetencyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var competencyItem = (CompetencyHelper.CompetencyItem)e.Item.DataItem;
            var hoursInput = (INumericBox)e.Item.FindControl("Hours");

            if (competencyItem == null || hoursInput == null)
                return;

            if (!competencyItem.Hours.HasValue && !competencyItem.Selected)
                return;

            hoursInput.ValueAsDecimal = competencyItem.Hours;
        }

        private List<(Guid Identifier, decimal? Hours)> GetSelectedCompetencies()
        {
            var result = new List<(Guid, decimal?)>();

            foreach (RepeaterItem areaRepeaterItem in Repeater.Items)
            {
                var competencyRepeater = (Repeater)areaRepeaterItem.FindControl("CompetencyRepeater");

                foreach (RepeaterItem competencyRepeaterItem in competencyRepeater.Items)
                {
                    var selectedCheckBox = (ICheckBoxControl)competencyRepeaterItem.FindControl("Selected");
                    if (!selectedCheckBox.Checked)
                        continue;

                    var hoursInput = (INumericBox)competencyRepeaterItem.FindControl("Hours");
                    var identifierLiteral = (System.Web.UI.WebControls.Literal)competencyRepeaterItem.FindControl("Identifier");
                    var identifier = Guid.Parse(identifierLiteral.Text);

                    result.Add((identifier, hoursInput.ValueAsDecimal));
                }
            }

            return result;
        }

        protected string EscapeText(object obj) => HttpUtility.HtmlEncode(obj);

    }
}