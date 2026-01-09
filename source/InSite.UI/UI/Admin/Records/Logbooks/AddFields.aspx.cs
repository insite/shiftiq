using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.JournalSetups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Logbooks
{
    public partial class AddFields : AdminBasePage, IHasParentLinkParameters
    {
        private static readonly List<JournalSetupFieldType> FieldTypes = new List<JournalSetupFieldType>
        {
            JournalSetupFieldType.Employer,
            JournalSetupFieldType.Supervisor,
            JournalSetupFieldType.StartAndEndDates,
            JournalSetupFieldType.Completed,
            JournalSetupFieldType.Hours,
            JournalSetupFieldType.TrainingEvidence,
            JournalSetupFieldType.MediaEvidence,
            JournalSetupFieldType.TrainingLevel,
            JournalSetupFieldType.TrainingLocation,
            JournalSetupFieldType.TrainingProvider,
            JournalSetupFieldType.TrainingCourseTitle,
            JournalSetupFieldType.TrainingComment,
            JournalSetupFieldType.TrainingType,
        };
        private class FieldItem
        {
            public JournalSetupFieldType Type { get; set; }
            public bool IsRequired { get; set; }
        }

        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier, x => x.Event);

                if (journalSetup == null || journalSetup.OrganizationIdentifier != Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
                    return;
                }

                var header = LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone);
                PageHelper.AutoBindHeader(this, null, header);

                if (!LoadFields())
                {
                    NoFieldsAlert.AddMessage(AlertType.Warning, "All fields have been added to logbook");

                    MainPanel.Visible = false;
                    SaveButton.Visible = false;
                }

                CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=fields";
            }
        }

        private bool LoadFields()
        {
            var fields = ServiceLocator.JournalSearch
                .GetJournalSetupFields(JournalSetupIdentifier)
                .Select(x => x.FieldType.ToEnum<JournalSetupFieldType>())
                .ToHashSet();

            var list = FieldTypes
                .Where(x => !fields.Contains(x))
                .Select(x => new
                {
                    Type = x.ToString(),
                    Name = x.GetDescription()
                })
                .ToList();

            if (list.Count == 0)
                return false;

            FieldRepeater.DataSource = list;
            FieldRepeater.DataBind();

            return true;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var fields = GetSelectedFields();
            if (fields.Count == 0)
            {
                EditorStatus.AddMessage(AlertType.Error, "There are no selected fields");
                return;
            }

            var existFields = ServiceLocator.JournalSearch.GetJournalSetupFields(JournalSetupIdentifier);
            var sequence = existFields.Count > 0 ? existFields.Max(x => x.Sequence) + 1 : 1;
            var existTypes = existFields.Select(x => x.FieldType.ToEnum<JournalSetupFieldType>()).ToHashSet();

            var commands = new List<Command>();

            foreach (var field in fields)
            {
                if (existTypes.Contains(field.Type))
                    continue;

                var content = new Shift.Common.ContentContainer();
                content[JournalSetupField.ContentLabels.LabelText].Text.Default = field.Type.GetDescription();

                var identifier = UniqueIdentifier.Create();

                commands.Add(new AddJournalSetupField(JournalSetupIdentifier, identifier, field.Type, sequence, field.IsRequired));
                commands.Add(new ChangeJournalSetupFieldContent(JournalSetupIdentifier, identifier, content));

                sequence++;
            }

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=fields");
        }

        private List<FieldItem> GetSelectedFields()
        {
            var list = new List<FieldItem>();

            foreach (RepeaterItem item in FieldRepeater.Items)
            {
                var selectedCheckBox = (ICheckBoxControl)item.FindControl("Selected");
                if (!selectedCheckBox.Checked)
                    continue;

                var typeLiteral = (ITextControl)item.FindControl("Type");
                var fieldType = typeLiteral.Text.ToEnum<JournalSetupFieldType>();

                var isRequiredCheckBox = (ICheckBoxControl)item.FindControl("IsRequired");
                var isRequired = isRequiredCheckBox.Checked;

                list.Add(new FieldItem { Type = fieldType, IsRequired = isRequired });
            }

            return list;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}&panel=fields"
                : null;
        }
    }
}
