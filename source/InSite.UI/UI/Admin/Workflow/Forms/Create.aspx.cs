using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Workflow.Forms.Utilities;
using InSite.Application.Surveys.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms
{
    public partial class CreateSurveyForm : AdminBasePage
    {
        private string Action => Request.QueryString["action"];
        protected Guid? SurveyFormIdentifier => Guid.TryParse(Request.QueryString["form"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += CreationType_ValueChanged;

            NameDuplicateValidator.ServerValidate += NameDuplicateValidator_ServerValidate;
            CopyNameDuplicateValidator.ServerValidate += CopyNameDuplicateValidator_ServerValidate;

            CopySurveyFormSelector.AutoPostBack = true;
            CopySurveyFormSelector.ValueChanged += CopySurveyFormSelector_ValueChanged;

            JsonFileUploadExtensionValidator.ServerValidate += JsonFileUploadExtensionValidator_ServerValidate;
            JsonFileUploadButton.Click += JsonFileUploadButton_Click;

            SaveButton.Click += SaveButton_Click;
            CopyButton.Click += CopyButton_Click;
            UploadSaveButton.Click += UploadSaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                HttpResponseHelper.Redirect("/ui/admin/workflow/forms/search");

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this, null);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate, CreationTypeEnum.Upload);

            if (Action == "duplicate")
            {
                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;

                if (SurveyFormIdentifier.HasValue)
                {
                    CopySurveyFormSelector.Value = SurveyFormIdentifier;
                    OnCopySurveyFormSelectorSelectedIndexChanged();
                }
            }

            OnCreationTypeSelected();

            CancelButton.NavigateUrl = "/ui/admin/workflow/forms/search";

            CopyCancelButton.NavigateUrl = CancelButton.NavigateUrl;
            UploadCancelButton.NavigateUrl = CancelButton.NavigateUrl;
        }

        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected()
        {
            var creationType = CreationType.ValueAsEnum;

            NewSection.Visible = creationType == CreationTypeEnum.One;
            CopySection.Visible = creationType == CreationTypeEnum.Duplicate;
            UploadSection.Visible = creationType == CreationTypeEnum.Upload;
        }

        private void NameDuplicateValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !ServiceLocator.SurveySearch.IsDuplicate(Organization.OrganizationIdentifier, Name.Text);
        }

        private void CopyNameDuplicateValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !ServiceLocator.SurveySearch.IsDuplicate(Organization.OrganizationIdentifier, CopyName.Text);
        }

        private void CopySurveyFormSelector_ValueChanged(object sender, EventArgs e)
        {
            OnCopySurveyFormSelectorSelectedIndexChanged();
        }

        private void OnCopySurveyFormSelectorSelectedIndexChanged()
        {
            if (CopySurveyFormSelector.HasValue)
            {
                var survey = ServiceLocator.SurveySearch.GetSurveyForm(CopySurveyFormSelector.Value.Value);
                survey.SurveyMessageInvitation = null;
                survey.SurveyMessageResponseCompleted = null;
                survey.SurveyMessageResponseConfirmed = null;
                survey.SurveyMessageResponseStarted = null;

                CopyName.Text = survey.SurveyFormName;
            }
            else
            {
                CopyName.Text = null;
            }
        }

        private void JsonFileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = e.Value == null || e.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
        }

        private void JsonFileUploadButton_Click(object sender, EventArgs e)
        {
            if (JsonFileUpload.PostedFile == null || JsonFileUpload.PostedFile.ContentLength == 0)
            {
                CreatorStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                return;
            }

            string text;
            using (var reader = new StreamReader(JsonFileUpload.FileContent, Encoding.UTF8))
                text = reader.ReadToEnd();

            JsonInput.Text = text;
        }

        private void UploadSaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                var (id, commands) = CreateCommands();

                foreach (var command in commands)
                    ServiceLocator.SendCommand(command);

                HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={id}");
            }
            catch (Newtonsoft.Json.JsonReaderException ex)
            {
                CreatorStatus.AddMessage(AlertType.Error, $"Your uploaded file has an unexpected format. {ex.Message}");
            }
            catch (ApplicationError apperr)
            {
                if (apperr.Message == "Unexpected JSON object type")
                    CreatorStatus.AddMessage(AlertType.Error, $"Your uploaded file is of a wrong type.");
                else
                    throw;
            }
        }

        private (Guid Identifier, ICommand[] Commands) CreateCommands()
        {
            var result = new FormDeserializer().Deserialize(JsonInput.Text);
            var id = result.Survey.Identifier;
            result.Survey.Asset = Sequence.Increment(result.Survey.Tenant, SequenceType.Asset);
            result.Survey.Source = "Uploaded";
            result.Survey.Name += " - Copy";
            result.Survey.Status = SurveyFormStatus.Drafted;
            result.Survey.Opened = null;
            result.Survey.Closed = null;

            if (result.Survey.Content == null)
                result.Survey.Content = new Shift.Common.ContentContainer();

            result.Survey.Questions = result.Questions;

            return (id, new SurveyCommandGenerator().GetCommands(null, result.Survey, true));
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!CopySurveyFormSelector.HasValue)
            {
                CreatorStatus.AddMessage(AlertType.Error, "Please select an existing Form from the drop-down list.");
                return;
            }

            var surveyID = CopySurveyFormSelector.Value.Value;
            var newSurveyID = UniqueIdentifier.Create();

            var commands = CreateCopyCommands(surveyID, newSurveyID);

            foreach (var command in commands)
            {
                if (command is ChangeSurveyFormMessages)
                {
                    var changeMessagesCommand = command as ChangeSurveyFormMessages;
                    var messages = changeMessagesCommand.Messages.Where(x => x.Type != SurveyMessageType.Invitation).ToArray();

                    var modifiedCommand = new ChangeSurveyFormMessages(changeMessagesCommand.AggregateIdentifier, messages);
                    ServiceLocator.SendCommand(modifiedCommand);
                    continue;
                }
                ServiceLocator.SendCommand(command);
            }

            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={newSurveyID}");
        }

        private ICommand[] CreateCopyCommands(Guid surveyID, Guid newSurveyID)
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(surveyID);
            var form = survey.Form;

            form.Identifier = newSurveyID;
            form.Asset = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset);
            form.Name = CopyName.Text;
            form.Status = SurveyFormStatus.Drafted;
            form.Opened = null;
            form.Closed = null;

            if (form.Content == null)
                form.Content = new Shift.Common.ContentContainer();

            form.Content.Title.Text[form.Language] = CopyName.Text;

            var map = new Dictionary<Guid, Guid>();
            foreach (var question in form.Questions)
            {
                var newId = UniqueIdentifier.Create();
                map.Add(question.Identifier, newId);
                question.Identifier = newId;
            }

            foreach (var question in form.Questions)
            {
                foreach (var optionList in question.Options.Lists)
                {
                    optionList.Identifier = UniqueIdentifier.Create();

                    foreach (var optionItem in optionList.Items)
                    {
                        optionItem.Identifier = UniqueIdentifier.Create();

                        if (optionItem.BranchToQuestionIdentifier.HasValue)
                            optionItem.BranchToQuestionIdentifier = map[optionItem.BranchToQuestionIdentifier.Value];

                        if (optionItem.MaskedQuestionIdentifiers != null)
                        {
                            for (int i = 0; i < optionItem.MaskedQuestionIdentifiers.Count; i++)
                                optionItem.MaskedQuestionIdentifiers[i] = map[optionItem.MaskedQuestionIdentifiers[i]];
                        }
                    }
                }
            }

            return new SurveyCommandGenerator().GetCommands(null, form);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            // Create a unique identifier and asset number.

            var survey = UniqueIdentifier.Create();
            var assetNumber = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset);

            // Create the form.

            var create = new Application.Surveys.Write.CreateSurveyForm(survey, "Authored", Organization.OrganizationIdentifier, assetNumber, Name.Text, SurveyFormStatus.Drafted, "en");
            ServiceLocator.SendCommand(create);

            if (!string.IsNullOrWhiteSpace(SurveyTitle.Text))
            {
                var content = new Shift.Common.ContentContainer();
                content.SetText("Title", "en", SurveyTitle.Text);
                ServiceLocator.SendCommand(new Application.Surveys.Write.ChangeSurveyFormContent(survey, content));
            }

            // Apply default form settings for this organization.

            bool requireUserIdentification;
            int? responseLimitPerUser;

            if (IsSubmissionsLimitedSelector.ValueAsBoolean.Value)
            {
                responseLimitPerUser = 1;
                requireUserIdentification = true;
            }
            else
            {
                responseLimitPerUser = null;
                requireUserIdentification = EnableAnonymousSave.ValueAsBoolean == false;
            }

            var change = new Application.Surveys.Write.ChangeSurveyFormSettings(
                survey,
                UserFeedbackType.Summary,
                EnableAnonymousSave.ValueAsBoolean.Value,
                false,
                responseLimitPerUser,
                null,
                requireUserIdentification);

            ServiceLocator.SendCommand(change);

            // Redirect to the outline.

            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={survey}");
        }
    }
}
