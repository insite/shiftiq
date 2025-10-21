using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.UI.Portal.Records.Logbooks.Controls;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.Entries.Controls
{
    public partial class LogEntryDetail : BaseUserControl
    {
        protected Guid ExperienceIdentifier { get; set; }

        private class FieldItem
        {
            public string LabelText { get; set; }
            public string HelpText { get; set; }
            public QExperience Experience { get; set; }
            public ExperienceFieldDescription Descriptor { get; set; }
        }

        public void LoadData(Guid journalSetupIdentifier, QExperience experience)
        {
            EntryNumber.Text = experience.Sequence.ToString();
            EntryCreated.Text = TimeZones.Format(experience.ExperienceCreated, CurrentSessionState.Identity.User.TimeZone, true);

            LoadFields(journalSetupIdentifier, experience);
        }

        private void LoadFields(Guid journalSetupIdentifier, QExperience experience)
        {
            ExperienceIdentifier = experience.ExperienceIdentifier;

            FieldRepeater.ItemDataBound += FieldRepeater_ItemDataBound;
            FieldRepeater.DataSource = ServiceLocator.JournalSearch.GetJournalSetupFields(journalSetupIdentifier)
                .Select(entity =>
                {
                    var content = ServiceLocator.ContentSearch.GetBlock(
                        entity.JournalSetupFieldIdentifier, ContentContainer.DefaultLanguage);
                    var fieldType = entity.FieldType.ToEnum<JournalSetupFieldType>();
                    
                    return new FieldItem
                    {
                        LabelText = content[JournalSetupField.ContentLabels.LabelText].Text.Default.IfNullOrEmpty(entity.FieldType),
                        HelpText = content[JournalSetupField.ContentLabels.HelpText].Text.Default,
                        Experience = experience,
                        Descriptor = ExperienceFieldDescription.Items[fieldType]
                    };
                });
            FieldRepeater.DataBind();
        }

        private void FieldRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var fieldItem = (FieldItem)e.Item.DataItem;
            var valueControl = (DynamicControl)e.Item.FindControl("FieldValue");
            fieldItem.Descriptor.AddValue(fieldItem.Experience, valueControl);
        }
    }
}