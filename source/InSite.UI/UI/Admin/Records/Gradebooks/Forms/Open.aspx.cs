using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Gradebooks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Gradebooks.Forms
{
    public partial class Open : AdminBasePage
    {
        private Guid ClassIdentifier => Guid.TryParse(Request["event"], out var value) ? value : Guid.Empty;

        private string Action => Request.QueryString["action"];
        protected Guid? GradebookIdentifier => Guid.TryParse(Request.QueryString["gradebook"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += CreationType_ValueChanged;

            IncludeValidator.ServerValidate += IncludeValidator_ServerValidate;

            EventIdentifier.AutoPostBack = true;
            EventIdentifier.ValueChanged += EventIdentifier_ValueChanged;

            Standards.AutoPostBack = true;
            Standards.CheckedChanged += Standards_CheckedChanged;

            AddStudentsButton.Click += AddStudentsButton_Click;

            CopyGradebookSelector.AutoPostBack = true;
            CopyGradebookSelector.ListFilter.OrganizationIdentifier = Organization.Identifier;
            CopyGradebookSelector.ValueChanged += CopyGradebookSelector_ValueChanged;

            SaveButton.Click += SaveButton_Click;
            SaveStudentButton.Click += SaveButton_Click;

            JsonFileUpload.FileUploaded += JsonFileUpload_FileUploaded;
            UploadSaveButton.Click += UploadSaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate, CreationTypeEnum.Upload);

            if (Action == "duplicate")
            {
                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;

                if (GradebookIdentifier.HasValue)
                {
                    CopyGradebookSelector.RefreshData();
                    CopyGradebookSelector.Value = GradebookIdentifier.Value.ToString();
                    CopyGradebookSelector.ValueAsGuid = GradebookIdentifier;
                    OnCopyGradebookSelectorSelectedIndexChanged();
                }
            }

            OnCreationTypeSelected();

            EventIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            EventIdentifier.Filter.EventType = "Class";

            if (ClassIdentifier != Guid.Empty)
                EventIdentifier.Value = ClassIdentifier;

            if (ClassIdentifier != Guid.Empty)
            {
                var @event = ServiceLocator.EventSearch.GetEvent(ClassIdentifier);
                if (@event != null)
                    AchievementIdentifier.Value = @event.AchievementIdentifier;
            }

            StandardIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            StandardIdentifier.Filter.StandardTypes = new[] { Shift.Constant.StandardType.Framework };

            CancelButton.NavigateUrl = ClassIdentifier != Guid.Empty
                ? $"/ui/admin/events/classes/outline?event={ClassIdentifier}&panel=gradebooks"
                : "/ui/admin/records/gradebooks/search";

            CancelStudentButton.NavigateUrl = UploadCancelButton.NavigateUrl = CancelButton.NavigateUrl;
        }

        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected()
        {
            var creationType = CreationType.ValueAsEnum;
            var isDuplicate = creationType == CreationTypeEnum.Duplicate;
            var isUpload = creationType == CreationTypeEnum.Upload;

            CopyGradebookField.Visible = isDuplicate;
            NewPanel.Visible = !isUpload;
            UploadSection.Visible = isUpload;
            SaveButton.Visible = !isUpload;

            CheckStudentsButtonVisibility();
        }

        private void IncludeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Scores.Checked || Standards.Checked;
        }

        private void EventIdentifier_ValueChanged(object sender, EventArgs e)
        {
            if (EventIdentifier.HasValue)
            {
                var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value.Value);
                var achievement = @event.AchievementIdentifier.HasValue ? ServiceLocator.AchievementSearch.GetAchievement(@event.AchievementIdentifier.Value) : null;

                if (achievement != null)
                    AchievementIdentifier.Value = achievement.AchievementIdentifier;
            }

            CheckStudentsButtonVisibility();
        }

        private void Standards_CheckedChanged(object sender, EventArgs e)
        {
            StandardField.Visible = Standards.Checked;
        }

        private void AddStudentsButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (!EventIdentifier.HasValue)
                {
                    AlertStatus.AddMessage(AlertType.Error, "Please select Class");
                    return;
                }

                var filter = new QUserFilter
                {
                    OrganizationIdentifiers = new[] { Organization.OrganizationIdentifier },
                    RegistrationEventIdentifier = EventIdentifier.Value,
                    OrderBy = "UserFullName"
                };

                var contacts = ServiceLocator.ContactSearch
                    .Bind(x => x, filter);

                if (contacts.Length > 0)
                {
                    StudentRepeater.DataSource = contacts;
                    StudentRepeater.DataBind();

                    StudentSection.Visible = true;
                    EventIdentifier.Enabled = false;
                }
                else
                    AlertClassStatus.AddMessage(AlertType.Warning, "Selected Class don't have a students. Please select another one.");
            }
        }

        private void CopyGradebookSelector_ValueChanged(object sender, EventArgs e)
        {
            OnCopyGradebookSelectorSelectedIndexChanged();
        }

        private void OnCopyGradebookSelectorSelectedIndexChanged()
        {
            if (CopyGradebookSelector.ValueAsGuid.HasValue)
            {
                var gradebook = ServiceLocator.RecordSearch.GetGradebook(CopyGradebookSelector.ValueAsGuid.Value);

                GradebookTitle.Text = gradebook.GradebookTitle + " - Copy";

                PeriodIdentifier.Value = gradebook.PeriodIdentifier;

                if (ClassIdentifier == Guid.Empty)
                    EventIdentifier.Value = gradebook.EventIdentifier;

                AchievementIdentifier.Value = gradebook.AchievementIdentifier;

                var gradebookType = gradebook.GradebookType.ToEnum<GradebookType>();
                var isStandards = gradebookType == GradebookType.ScoresAndStandards || gradebookType == GradebookType.Standards;

                Standards.Checked = isStandards;

                StandardField.Visible = Standards.Checked;

                StandardIdentifier.Value = isStandards ? gradebook.FrameworkIdentifier : null;
            }
            else
            {
                GradebookTitle.Text = null;
            }
        }

        private void JsonFileUpload_FileUploaded(object sender, EventArgs e)
        {
            var text = JsonFileUpload.ReadFileText(Encoding.UTF8);

            if (string.IsNullOrWhiteSpace(text))
            {
                AlertStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                return;
            }

            JsonInput.Text = text;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var commands = new List<Command>();
            var identifier = UniqueIdentifier.Create();

            if (CreationType.ValueAsEnum == CreationTypeEnum.One)
                CreateNew(identifier, commands);
            else
                Duplicate(identifier, commands);

            AddStudentCommands(identifier, commands);

            ServiceLocator.SendCommands(commands);

            var url = EventIdentifier.Value == ClassIdentifier
                ? CancelButton.NavigateUrl
                : $"/ui/admin/records/gradebooks/outline?id={identifier}";

            HttpResponseHelper.Redirect(url);
        }

        private void UploadSaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                var json = JsonInput.Text;
                var result = GradebookHelper.Deserialize(Organization.OrganizationIdentifier, json);

                ServiceLocator.SendCommands(result.Commands);

                if (result.Warnings.Count > 0)
                    Outline.UploadWarnings = result.Warnings;

                HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/outline?id={result.GradebookIdentifier}&status=uploaded", true);
            }
            catch (JsonReaderException ex)
            {
                AlertStatus.AddMessage(AlertType.Error, $"Your uploaded file has an unexpected format. {ex.Message}");
            }
            catch (Exception ex)
            {
                AlertStatus.AddMessage(AlertType.Error, "Error during import: " + ex.Message);
            }
        }

        private void CreateNew(Guid identifier, List<Command> commands)
        {
            GradebookType gradebookType;
            if (Scores.Checked && Standards.Checked)
                gradebookType = GradebookType.ScoresAndStandards;
            else if (Scores.Checked)
                gradebookType = GradebookType.Scores;
            else
                gradebookType = GradebookType.Standards;

            var standardIdentifier = Standards.Checked ? StandardIdentifier.Value.Value : (Guid?)null;

            var command = new CreateGradebook(identifier, Organization.OrganizationIdentifier, GradebookTitle.Text, gradebookType, EventIdentifier.Value, AchievementIdentifier.Value, standardIdentifier);

            commands.Add(command);

            if (PeriodIdentifier.HasValue)
                commands.Add(new ChangeGradebookPeriod(identifier, PeriodIdentifier.Value.Value));
        }

        private void Duplicate(Guid identifier, List<Command> commands)
        {
            GradebookType gradebookType;
            if (Scores.Checked && Standards.Checked)
                gradebookType = GradebookType.ScoresAndStandards;
            else if (Scores.Checked)
                gradebookType = GradebookType.Scores;
            else
                gradebookType = GradebookType.Standards;

            var standardIdentifier = Standards.Checked ? StandardIdentifier.Value.Value : (Guid?)null;

            commands.Add(new DuplicateGradebook(
                identifier,
                Organization.OrganizationIdentifier,
                CopyGradebookSelector.ValueAsGuid.Value,
                GradebookTitle.Text,
                gradebookType,
                EventIdentifier.Value,
                AchievementIdentifier.Value,
                standardIdentifier
            ));

            if (PeriodIdentifier.HasValue)
                commands.Add(new ChangeGradebookPeriod(identifier, PeriodIdentifier.Value.Value));
        }

        private void AddStudentCommands(Guid identifier, List<Command> commands)
        {
            foreach (RepeaterItem item in StudentRepeater.Items)
            {
                var selectedCheckbox = (ICheckBoxControl)item.FindControl("Selected");
                if (!selectedCheckbox.Checked)
                    continue;

                var studentIdentifierLiteral = (ITextControl)item.FindControl("StudentIdentifier");
                var create = ServiceLocator.RecordSearch.CreateCommandToAddEnrollment(null, identifier, Guid.Parse(studentIdentifierLiteral.Text), null, null, null);
                commands.Add(create);

            }
        }

        private void CheckStudentsButtonVisibility()
        {
            if (CreationType.ValueAsEnum == CreationTypeEnum.Upload)
            {
                AddStudentsButton.Visible = false;
                return;
            }

            var hasContacts = false;

            if (EventIdentifier.HasValue)
            {
                var filter = new QUserFilter
                {
                    OrganizationIdentifiers = new[] { Organization.OrganizationIdentifier },
                    RegistrationEventIdentifier = EventIdentifier.Value,
                    OrderBy = "UserFullName"
                };

                var contacts = ServiceLocator.ContactSearch.Bind(x => x, filter);

                hasContacts = contacts.Length > 0;
            }

            AddStudentsButton.Visible = hasContacts;
        }

    }
}
