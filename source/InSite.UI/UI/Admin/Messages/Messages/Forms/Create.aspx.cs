using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Shift.Common.Timeline.Commands;

using InSite.Application.Messages.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Messages.Messages.Forms
{
    public partial class Create : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        #region Properties

        protected Guid? MessageIdentifier => Guid.TryParse(Request.QueryString["message"], out var result) ? result : (Guid?)null;

        private bool IsNewsletter => Request["newsletter"] == "1";

        private bool IsNotification => Request["notification"] == "1";

        private bool IsInvitation => Request.QueryString["invitation"] == "1";

        string DefaultParameters => $"message={MessageIdentifier}&tab=message";

        private string SearchUrl
        {
            get => (string)ViewState[nameof(SearchUrl)];
            set => ViewState[nameof(SearchUrl)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += (s, a) => OnCreationTypeChanged();

            NewMessageType.AutoPostBack = true;
            NewMessageType.ValueChanged += (s, a) => OnNewMessageTypeChanged();

            NewNotificationName.AutoPostBack = true;
            NewNotificationName.ValueChanged += (s, a) =>
            {
                var notification = OnNewNotificationNameChanged();
                if (notification != null)
                {
                    NewSubject.Text.Clear();
                    NewSubject.Text.Default = notification.Subject;
                }
            };

            UploadJsonFileUploaded.Click += UploadJsonFileUploaded_Click;

            CopyMessageSelector.AutoPostBack = true;
            CopyMessageSelector.ValueChanged += (s, a) => OnCopyMessageChanged();

            SaveButton.Click += (s, a) => { if (Page.IsValid) Save(); };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            Open();
        }

        #endregion

        #region Methods (open)

        private void Open()
        {
            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate, CreationTypeEnum.Upload);

            CopyMessageSelector.Filter.ExcludeTypes = new[] { MessageTypeName.Alert };

            NewNotificationName.ExcludeValues = ServiceLocator.MessageSearch
                .GetMessages(new MessageFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    Type = MessageTypeName.Alert
                })
                .Select(x => x.MessageName)
                .ToArray();
            NewNotificationName.RefreshData();

            SetupMessageType(out var messageType);

            SetupSender();

            var action = Request.QueryString["action"];
            if (action == "duplicate")
            {
                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;

                if (MessageIdentifier.HasValue)
                {
                    CopyMessageSelector.Value = MessageIdentifier;
                    OnCopyMessageChanged();
                }
            }

            OnCreationTypeChanged();

            SearchUrl = $"/ui/admin/messages/messages/search";

            if (messageType.IsNotEmpty())
                SearchUrl += $"?type={messageType}";

            CancelButton.NavigateUrl = GetParentUrl(DefaultParameters);
        }

        private void SetupSender()
        {
            var organization = OrganizationSearch.Select(Organization.Identifier);
            var senderIdentifier = User.UserIdentifier;

            if (organization.AdministratorUserIdentifier != null)
            {
                var administrator = UserSearch.Bind(
                    organization.AdministratorUserIdentifier.Value,
                    x => new { x.Email, x.FullName });

                if (administrator != null)
                {
                    senderIdentifier = organization.AdministratorUserIdentifier.Value;
                }
            }

            NewSenderInput.BindModelToControls(senderIdentifier);

            UploadSenderInput.BindModelToControls(senderIdentifier);
        }

        private void SetupMessageType(out string messageType)
        {
            messageType = Request.QueryString["type"].NullIfEmpty();

            if (Guid.TryParse(Request.QueryString["form"], out var surveyFormId))
            {
                var survey = ServiceLocator.SurveySearch.GetSurveyForm(surveyFormId);

                if (survey != null && survey.OrganizationIdentifier == Organization.Identifier)
                {
                    NewSurveyFormSelector.Value = survey.SurveyFormIdentifier;
                    NewSurveyFormSelector.Enabled = false;

                    if (IsInvitation)
                        messageType = MessageTypeName.Invitation;

                    if (IsNewsletter)
                        messageType = MessageTypeName.Newsletter;

                    if (IsNotification)
                        messageType = MessageTypeName.Notification;
                }
            }

            NewMessageType.Enabled = messageType == null;
            NewMessageType.Value = messageType ?? MessageTypeName.Newsletter;

            OnNewMessageTypeChanged();
        }

        #endregion

        #region Methods (save)

        private void Save()
        {
            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                SaveOne();
            if (value == CreationTypeEnum.Duplicate)
                SaveCopy();
            if (value == CreationTypeEnum.Upload)
                SaveUpload();
        }

        private void SaveOne()
        {
            var model = new VMessage
            {
                MessageType = NewMessageType.Value,
                MessageName = NewName.Text
            };
            var modelContent = new ContentContainer();

            if (model.MessageType == MessageTypeName.Notification)
            {
                model.MessageName = NewSubject.Text.Default;
            }
            else if (model.MessageType == MessageTypeName.Alert)
            {
                model.MessageName = NewNotificationName.Value;

                var count = ServiceLocator.MessageSearch.CountMessages(new MessageFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    Type = MessageTypeName.Alert,
                    Name = model.MessageName,
                });

                if (count > 0)
                {
                    ScreenStatus.AddMessage(AlertType.Error, $"An alert of the '{model.MessageName}' type already exists in the system");
                    return;
                }
            }

            model.SenderIdentifier = NewSenderInput.Value.Value;
            modelContent.Title.Text = NewSubject.Text;

            if (NewSurveyFormSelector.HasValue)
                model.SurveyFormIdentifier = NewSurveyFormSelector.Value;

            if (model.MessageType == MessageTypeName.Notification || model.MessageType == MessageTypeName.Alert)
            {
                var message = ServiceLocator.MessageSearch.GetMessage(new MessageFilter
                {
                    Name = model.MessageName,
                    OrganizationIdentifier = OrganizationIdentifiers.Global
                });

                if (message != null)
                {
                    var messageContent = ServiceLocator.ContentSearch.GetBlock(message.MessageIdentifier, labels: new[] { ContentLabel.Body });
                    modelContent.Body.Set(messageContent.Body);
                }
            }

            model.OrganizationIdentifier = Organization.OrganizationIdentifier;

            // Check to see if there is an existing Form Invitation message assigned to the same form. If so 
            // then display an error message.

            if (model.MessageType == MessageTypeName.Invitation && model.SurveyFormIdentifier.HasValue)
            {
                var filter = new MessageFilter
                {
                    SurveyFormIdentifier = model.SurveyFormIdentifier,
                    Type = MessageTypeName.Invitation
                };

                if (ServiceLocator.MessageSearch.CountMessages(filter) > 0)
                {
                    ScreenStatus.AddMessage(
                        AlertType.Error,
                        "A form invitation message already exists for the form you have selected. Please select a different form, or use the existing message to send invitations for this form.");

                    return;
                }

                modelContent.Body.Text.Default = @"Please complete this form by clicking the **Begin Form** button.

{Survey-Link}

Thank you!"
                ;
            }

            IEnumerable<ICommand> commands;

            if (IsNewsletter || IsNotification)
            {
                var surveyType = Request.QueryString["surveyType"].ToEnum(SurveyMessageType.Undefined);

                commands = MessageHelper.CreateMessage(model, modelContent, true, surveyType);
            }
            else
            {
                commands = MessageHelper.CreateMessage(model, modelContent, false);
            }

            foreach (var cmd in commands)
                ServiceLocator.SendCommand(cmd);

            HttpResponseHelper.Redirect(RedirectOnSave(GetParentUrl(""), model.MessageIdentifier));
        }

        private void SaveCopy()
        {
            var source = CopyMessageSelector.HasValue
                ? ServiceLocator.MessageSearch.GetMessage(CopyMessageSelector.Value.Value)
                : null;

            if (source == null)
            {
                CopyMessageSelector.Value = null;
                return;
            }

            var message = source.Clone();
            var content = ServiceLocator.ContentSearch.GetBlock(source.MessageIdentifier);

            message.MessageName = CopyName.Text;
            content.Title.Text.Set(CopySubjectInput.Text);

            var commands = MessageHelper.CreateMessage(message, content);
            foreach (var cmd in commands)
                ServiceLocator.SendCommand(cmd);

            HttpResponseHelper.Redirect(RedirectOnSave(GetParentUrl(""), message.MessageIdentifier));
        }

        private void SaveUpload()
        {
            try
            {
                if (!MessageHelper.Deserialize(UploadJsonInput.Text, out var message, out var content))
                {
                    ScreenStatus.AddMessage(AlertType.Error, "Wrong JSON file uploaded");
                    return;
                }

                message.OrganizationIdentifier = Organization.OrganizationIdentifier;
                message.SenderIdentifier = UploadSenderInput.Value.Value;
                message.SurveyFormIdentifier = Guid.Empty;
                message.MessageIdentifier = Guid.Empty;

                var commands = MessageHelper.CreateMessage(message, content);
                foreach (var cmd in commands)
                    ServiceLocator.SendCommand(cmd);

                HttpResponseHelper.Redirect(RedirectOnSave(GetParentUrl(""), message.MessageIdentifier));
            }
            catch (JsonReaderException ex)
            {
                ScreenStatus.AddMessage(AlertType.Error, $"Your uploaded file has an unexpected format. {ex.Message}");
            }
            catch (Exception ex)
            {
                ScreenStatus.AddMessage(AlertType.Error, "Error during import: " + ex.Message);
            }
        }

        #endregion

        #region Methods (navigate back)

        string RedirectOnSave(string parent, Guid messageIdentifier)
        {
            if (parent.EndsWith("/search"))
                return GetOutlineUrl(messageIdentifier);

            return GetParentUrl(DefaultParameters);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => GetParentLinkParameters(parent, null).IfNullOrEmpty($"message={MessageIdentifier}");

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        private string GetOutlineUrl(Guid messageIdentifier) =>
            $"/ui/admin/messages/outline?message={messageIdentifier}&tab=message";

        #endregion

        #region Event handlers

        private void OnCreationTypeChanged()
        {
            const string col6 = "col-lg-6";
            const string col12 = "col-12";

            var creationType = CreationType.ValueAsEnum;

            if (creationType == CreationTypeEnum.One)
                MultiView.SetActiveView(ViewNewSection);
            else if (creationType == CreationTypeEnum.Duplicate)
                MultiView.SetActiveView(ViewDuplicateSection);
            else if (creationType == CreationTypeEnum.Upload)
                MultiView.SetActiveView(ViewUploadSection);
            else
                throw ApplicationError.Create("Unsupported creation type: " + creationType.GetName());

            if (creationType == CreationTypeEnum.One || creationType == CreationTypeEnum.Upload)
            {
                MainColumn.Attributes["class"] = col12;
                CreationTypeColumn.Attributes["class"] = col6;
            }
            else
            {
                MainColumn.Attributes["class"] = col6;
                CreationTypeColumn.Attributes["class"] = col12;
            }
        }

        private void OnNewMessageTypeChanged()
        {
            var value = NewMessageType.Value;
            var isInvitation = value == MessageTypeName.Invitation;
            var isNotification = value == MessageTypeName.Alert || value == MessageTypeName.Notification;

            NewSurveyFormField.Visible = isInvitation || IsNewsletter || IsNotification;

            NewAssignmentLimitField.Visible = false;
            NewNameField.Visible = !isNotification;
            NewNotificationField.Visible = isNotification;

            OnNewNotificationNameChanged();
        }

        private Notification OnNewNotificationNameChanged()
        {
            var value = NewMessageType.Value;
            var isNotification = value == MessageTypeName.Alert || value == MessageTypeName.Notification;

            var name = NewNotificationName.Value;
            if (name.IsEmpty() || !Notifications.Contains(name))
                return null;

            var type = name.ToEnum<NotificationType>();
            var alert = Notifications.Select(type);

            if (alert == null)
                return null;

            NewSenderInput.SenderStatus = "Enabled";

            NewNotificationDescription.InnerText = alert.Purpose;

            return alert;
        }

        private void UploadJsonFileUploaded_Click(object sender, EventArgs e)
        {
            if (!UploadJsonFile.HasFile)
            {
                ScreenStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                return;
            }

            using (var stream = UploadJsonFile.OpenFile())
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    UploadJsonInput.Text = reader.ReadToEnd();
            }
        }

        private void OnCopyMessageChanged()
        {
            var message = CopyMessageSelector.HasValue
                ? ServiceLocator.MessageSearch.GetMessage(CopyMessageSelector.Value.Value)
                : null;

            if (message != null)
            {
                var content = ServiceLocator.ContentSearch.GetBlock(message.MessageIdentifier, labels: new[] { ContentLabel.Title });

                CopyName.Text = message.MessageName;
                CopySubjectInput.Text = content.Title.Text;
            }
            else
            {
                CopyName.Text = null;
                CopySubjectInput.Text = null;
            }
        }

        #endregion
    }
}