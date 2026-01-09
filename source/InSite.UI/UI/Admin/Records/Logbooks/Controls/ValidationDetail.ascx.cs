using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Portal.Records.Logbooks.Controls;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Logbooks.Controls
{
    public partial class ValidationDetail : BaseUserControl
    {
        private class FieldItem
        {
            public string LabelText { get; set; }
            public string HelpText { get; set; }
            public string FieldType { get; set; }
            public QExperience Experience { get; set; }
            public ExperienceFieldDescription Descriptor { get; set; }
        }

        protected Guid ExperienceIdentifier { get; set; }
        protected string ChangeUrl { get; set; }

        public void LoadData(Guid journalSetupIdentifier, Guid userIdentifier, QExperience experience, bool isValidator)
        {
            var user = UserSearch.Select(userIdentifier);

            UserName.Text = user.FullName;
            EntryNumber.Text = experience.Sequence.ToString();
            EntryCreated.Text = TimeZones.Format(experience.ExperienceCreated, CurrentSessionState.Identity.User.TimeZone, true);

            LoadFields(journalSetupIdentifier, experience, isValidator);
        }

        private void LoadFields(Guid journalSetupIdentifier, QExperience experience, bool isValidator)
        {
            ExperienceIdentifier = experience.ExperienceIdentifier;
            ChangeUrl = isValidator
                ? "/ui/admin/records/logbooks/validators/change-experience-field"
                : "/ui/admin/records/logbooks/change-experience-field";

            FieldRepeater.ItemDataBound += FieldRepeater_ItemDataBound;
            FieldRepeater.DataSource = ServiceLocator.JournalSearch
                .GetJournalSetupFields(journalSetupIdentifier)
                .Select(entity =>
                {
                    var content = ServiceLocator.ContentSearch
                        .GetBlock(entity.JournalSetupFieldIdentifier, ContentContainer.DefaultLanguage);
                    var fieldType = entity.FieldType.ToEnum<JournalSetupFieldType>();
                    var description = ExperienceFieldDescription.Items[fieldType];
                    return new FieldItem
                    {
                        LabelText = content[JournalSetupField.ContentLabels.LabelText].Text.Default
                            .IfNullOrEmpty(entity.FieldType),
                        HelpText = content[JournalSetupField.ContentLabels.HelpText].Text.Default,
                        FieldType = entity.FieldType,
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