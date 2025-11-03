using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Records.Logbooks;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.UI.Portal.Records.Logbooks.Controls;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.Controls
{
    public partial class UserJournalTree : BaseUserControl
    {
        private const int MaxVisibleFieldCount = 5;

        private static List<JournalSetupFieldType> VisibleFields = new List<JournalSetupFieldType>
        {
            JournalSetupFieldType.Employer,
            JournalSetupFieldType.Supervisor,
            JournalSetupFieldType.StartAndEndDates,
            JournalSetupFieldType.Completed,
            JournalSetupFieldType.TrainingLevel,
            JournalSetupFieldType.TrainingLocation,
            JournalSetupFieldType.TrainingProvider,
            JournalSetupFieldType.TrainingCourseTitle,
            JournalSetupFieldType.TrainingType
        };

        private class FieldItem
        {
            public string LabelText { get; set; }
            public ExperienceFieldDescription Descriptor { get; internal set; }
            public QExperience Experience { get; internal set; }
        }

        private class CompetencyItem
        {
            public Guid StandardIdentifier { get; set; }
            public string CompetencyName { get; set; }
            public string Hours { get; set; }
            public string SkillRating { get; set; }
            public string SatisfactionLevel { get; set; }
        }

        private class ExperienceItem
        {
            public Guid ExperienceIdentifier { get; set; }
            public string Hours { get; set; }
            public string SkillRating { get; set; }
            public List<FieldItem> Fields { get; set; }
            public List<CompetencyItem> Competencies { get; set; }

            public string ColumnStyle
            {
                get
                {
                    var width = 100 / (Fields.Count + 2);
                    return $"width:{width}%;";
                }
            }
        }

        private class JournalItem
        {
            public Guid JournalIdentifier { get; set; }
            public string JournalSetupName { get; set; }
            public string Hours { get; set; }
            public string JournalUrl { get; set; }
            public List<ExperienceItem> Experiences { get; set; }
        }

        protected Guid UserIdentifier { get; set; }
        protected string ParentColumnStyle { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "initTreeView",
                "(function () { initUserJournalTreeView(); })();",
                true);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            JournalRepeater.ItemDataBound += JournalRepeater_ItemDataBound;
        }

        private void JournalRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e.Item))
                return;

            var journal = (JournalItem)e.Item.DataItem;

            var experienceRepeater = (Repeater)e.Item.FindControl("ExperienceRepeater");
            experienceRepeater.ItemDataBound += ExperienceRepeater_ItemDataBound;
            experienceRepeater.DataSource = journal.Experiences;
            experienceRepeater.DataBind();
        }

        private void ExperienceRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e.Item))
                return;

            var experience = (ExperienceItem)e.Item.DataItem;

            ParentColumnStyle = experience.ColumnStyle;

            var labelRepeater = (Repeater)e.Item.FindControl("LabelRepeater");
            labelRepeater.DataSource = experience.Fields;
            labelRepeater.DataBind();

            var valueRepeater = (Repeater)e.Item.FindControl("ValueRepeater");
            valueRepeater.ItemDataBound += ValueRepeater_ItemDataBound;
            valueRepeater.DataSource = experience.Fields;
            valueRepeater.DataBind();

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = experience.Competencies;
            competencyRepeater.DataBind();
        }

        private void ValueRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e.Item))
                return;

            var fieldItem = (FieldItem)e.Item.DataItem;
            var valueControl = (DynamicControl)e.Item.FindControl("Value");
            fieldItem.Descriptor.AddValue(fieldItem.Experience, valueControl);
        }

        public bool LoadData(Guid organizationIdentifier, Guid userIdentifier, Guid? validatorUserIdentifier)
        {
            var journalItems = GetJournalItems(organizationIdentifier, userIdentifier, validatorUserIdentifier);

            UserIdentifier = userIdentifier;

            JournalRepeater.DataSource = journalItems;
            JournalRepeater.DataBind();

            return journalItems.Count > 0;
        }

        private List<JournalItem> GetJournalItems(Guid organizationIdentifier, Guid userIdentifier, Guid? validatorUserIdentifier)
        {
            var journalFilter = new QJournalFilter
            {
                OrganizationIdentifier = organizationIdentifier,
                UserIdentifier = userIdentifier,
                ValidatorUserIdentifier = validatorUserIdentifier
            };

            var journals = ServiceLocator.JournalSearch.GetJournals(
                journalFilter,
                null,
                null,
                x => x.JournalSetup.Fields,
                x => x.Experiences
            );

            var competencyFilter = new QExperienceCompetencyFilter
            {
                OrganizationIdentifier = organizationIdentifier,
                UserIdentifier = userIdentifier,
                ValidatorUserIdentifier = validatorUserIdentifier
            };

            var competencies = ServiceLocator.JournalSearch.GetExperienceCompetencies(
                competencyFilter,
                null,
                null,
                x => x.Competency
            );

            return journals
                .OrderBy(x => x.JournalSetup.JournalSetupName)
                .Select(x => GetJournalItem(validatorUserIdentifier.HasValue, x, competencies))
                .ToList();
        }

        private static JournalItem GetJournalItem(bool isValidator, QJournal journal, List<QExperienceCompetency> competencies)
        {
            var journalItem = new JournalItem
            {
                JournalIdentifier = journal.JournalIdentifier,
                JournalSetupName = journal.JournalSetup.JournalSetupName,
                JournalUrl = isValidator
                    ? $"/ui/admin/records/logbooks/validators/outline-journal?journalsetup={journal.JournalSetupIdentifier}&user={journal.UserIdentifier}"
                    : $"/ui/admin/records/logbooks/outline-journal?journalsetup={journal.JournalSetupIdentifier}&user={journal.UserIdentifier}",
                Experiences = new List<ExperienceItem>()
            };

            var classification = "Competency";

            foreach (var experience in journal.Experiences)
            {
                var experienceItem = new ExperienceItem
                {
                    ExperienceIdentifier = experience.ExperienceIdentifier,
                    Hours = experience.ExperienceHours.HasValue ? $"{experience.ExperienceHours:n2}" : "<i>None</i>",
                    SkillRating = experience.SkillRating.HasValue ? experience.SkillRating.ToString() : "<i>None</i>",
                    Fields = GetFields(journal.JournalSetup, experience)
                };

                experienceItem.Competencies = competencies
                    .Where(x => x.ExperienceIdentifier == experience.ExperienceIdentifier)
                    .OrderBy(x => x.Competency.CompetencyTitle)
                    .Select(x => new CompetencyItem
                    {
                        StandardIdentifier = x.CompetencyStandardIdentifier,
                        CompetencyName = CompetencyHelper.GetStandardName(
                            x.Competency.CompetencyIdentifier,
                            x.Competency.CompetencyAsset,
                            x.Competency.CompetencyLabel,
                            x.Competency.CompetencyCode,
                            classification
                        ),
                        Hours = x.CompetencyHours.HasValue ? $"{x.CompetencyHours:n2}" : "<i>None</i>",
                        SkillRating = x.SkillRating.HasValue ? x.SkillRating.ToString() : "<i>None</i>",
                        SatisfactionLevel = !string.IsNullOrEmpty(x.SatisfactionLevel) ? x.SatisfactionLevel : "<i>None</i>"
                    })
                    .ToList();

                journalItem.Experiences.Add(experienceItem);
            }

            var totalHours = journal.Experiences.Sum(x => x.ExperienceHours ?? 0);
            journalItem.Hours = $"{totalHours:n2}";

            return journalItem;
        }

        private static List<FieldItem> GetFields(QJournalSetup journalSetup, QExperience experience)
        {
            var fields = journalSetup.Fields
                .Select(x => new
                {
                    Identifier = x.JournalSetupFieldIdentifier,
                    FieldType = x.FieldType.ToEnum<JournalSetupFieldType>(),
                })
                .ToDictionary(x => x.FieldType, x => x.Identifier);

            var result = new List<FieldItem>();

            foreach (var fieldType in VisibleFields)
            {
                if (!fields.TryGetValue(fieldType, out var fieldIdentifier))
                    continue;

                var content = ServiceLocator.ContentSearch.GetBlock(fieldIdentifier, ContentContainer.DefaultLanguage);
                var labelText = content[JournalSetupField.ContentLabels.LabelText]
                    .Text
                    .Default
                    .IfNullOrEmpty(fieldType.GetDescription());

                var descriptor = ExperienceFieldDescription.Items[fieldType];

                result.Add(new FieldItem
                {
                    LabelText = labelText,
                    Descriptor = descriptor,
                    Experience = experience
                });

                if (result.Count == MaxVisibleFieldCount)
                    break;
            }

            return result;
        }
    }
}