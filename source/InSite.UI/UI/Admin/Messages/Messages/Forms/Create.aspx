<%@ Page CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Messages.Messages.Forms.Create" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/FieldInputSender.ascx" TagName="InputSender" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Message" />

    <div class="row mb-3">
        <div runat="server" id="MainColumn">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h4 class="card-title mb-3">
                        <i class="far fa-paper-plane me-1"></i>
                        Message
                    </h4>

                    <div class="row mb-3">
                        <div runat="server" id="CreationTypeColumn">
                            <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Message" />
                        </div>
                    </div>

                    <asp:MultiView runat="server" ID="MultiView">

                        <asp:View runat="server" ID="ViewNewSection">
                            <div class="row">
                                <div class="col-lg-6 mb-3 mb-lg-0">
                                    <div class="card h-100">
                                        <div class="card-body">

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Message Type
                                                </label>
                                                <insite:MessageTypeComboBox runat="server" ID="NewMessageType" AllowBlank="false" />
                                                <div class="form-text">
                                                    The type of message you want to send.
                                                </div>
                                            </div>

                                            <div runat="server" ID="NewSurveyFormField" visible="false" class="form-group mb-3">
                                                <label class="form-label">
                                                    Form
                                                    <insite:RequiredValidator runat="server" ControlToValidate="NewSurveyFormSelector" FieldName="Form" ValidationGroup="Message" />
                                                </label>
                                                <insite:FindWorkflowForm runat="server" ID="NewSurveyFormSelector" EmptyMessage="Select a form in the Forms toolkit" />
                                                <div class="form-text">
                                                    A link to this form must be included in the body of the invitation email.
                                                </div>
                                            </div>

                                            <div runat="server" ID="NewAssignmentLimitField" visible="false" class="form-group mb-3">
                                                <label class="form-label">
                                                    Candidate Limit
                                                </label>
                                                <insite:TextBox runat="server" ID="NewAssignmentLimit" MaxLength="64" />
                                                <div class="form-text">
                                                </div>
                                            </div>

                                            <div runat="server" id="NewNameField" class="form-group mb-3">
                                                <label class="form-label">
                                                    Internal Name
                                                    <insite:RequiredValidator runat="server" ControlToValidate="NewName" FieldName="Internal Name" ValidationGroup="Message" />
                                                </label>
                                                <insite:TextBox ID="NewName" runat="server" MaxLength="256" />
                                                <div class="form-text">
                                                    An internal reference for filing purposes. It is a required field, and it is not visible to message recipients.
                                                </div>
                                            </div>

                                            <div runat="server" id="NewNotificationField" visible="false" class="form-group mb-3">
                                                <label class="form-label">
                                                    Notification Type
                                                    <insite:RequiredValidator runat="server" ControlToValidate="NewNotificationName" FieldName="Notification Trigger" ValidationGroup="Message" />
                                                </label>
                                                <insite:AlertComboBox runat="server" ID="NewNotificationName" />
                                                <div runat="server" id="NewNotificationDescription" class="form-text">
                                                    The application event that triggers this notification.
                                                </div>
                                            </div>

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Subject
                                                    <insite:RequiredValidator runat="server" ControlToValidate="NewSubject" FieldName="Subject" ValidationGroup="Message" />
                                                </label>
                                                <insite:TextBox runat="server" TranslationControl="NewSubject" MaxLength="256" />
                                                <div class="mt-1">
                                                    <insite:EditorTranslation runat="server" ID="NewSubject" />
                                                </div>
                                                <div class="form-text">
                                                    The subject line on the email message sent to recipients.
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="card h-100">
                                        <div class="card-body">

                                            <uc:InputSender runat="server" ID="NewSenderInput" ValidationGroup="Message" />

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="ViewUploadSection">
                            <div class="row">
                                <div class="col-lg-6 mb-3 mb-lg-0">
                                    <div class="card h-100">
                                        <div class="card-body">

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Select and Upload Message JSON File
                                                </label>
                                                <insite:FileUploadV1 runat="server" ID="UploadJsonFile"
                                                    AllowedExtensions=".json"
                                                    LabelText=""
                                                    FileUploadType="Unlimited"
                                                    OnClientFileUploaded="messageCreate.onJsonFileUploaded" />
                                                <asp:Button runat="server" ID="UploadJsonFileUploaded" CssClass="d-none" />
                                            </div>

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Uploaded JSON
                                                    <insite:RequiredValidator runat="server" ControlToValidate="UploadJsonInput" FieldName="Uploaded JSON" Display="Dynamic" ValidationGroup="Message" />
                                                </label>
                                                <insite:TextBox runat="server" ID="UploadJsonInput" TextMode="MultiLine" Rows="15" />
                                            </div>

                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="card h-100">
                                        <div class="card-body">

                                            <uc:InputSender runat="server" ID="UploadSenderInput" ValidationGroup="Message" />

                                        </div>
                                    </div>
                                </div>
                            </div>

                        </asp:View>

                        <asp:View runat="server" ID="ViewDuplicateSection">
                            <div class="card">
                                <div class="card-body">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Message
                                            <insite:RequiredValidator runat="server" ControlToValidate="CopyMessageSelector" FieldName="Message" ValidationGroup="Message" />
                                        </label>
                                        <insite:FindMessage runat="server" ID="CopyMessageSelector" />
                                        <div class="form-text">
                                            This message is use for copy.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Internal Name
                                            <insite:RequiredValidator runat="server" ControlToValidate="CopyName" FieldName="Internal Name" ValidationGroup="Message" />
                                        </label>
                                        <insite:TextBox runat="server" ID="CopyName" MaxLength="256" />
                                        <div class="form-text">
                                            An internal reference for filing purposes. It is a required field, and it is not visible to message recipients.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Subject
                                            <insite:RequiredValidator runat="server" ControlToValidate="CopySubjectInput" FieldName="Subject" ValidationGroup="Message" />
                                        </label>
                                        <insite:TextBox runat="server" TranslationControl="CopySubjectInput" MaxLength="256" />
                                        <div class="mt-1">
                                            <insite:EditorTranslation runat="server" ID="CopySubjectInput" />
                                        </div>
                                        <div class="form-text">
                                            The subject line on the email message sent to recipients.
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </asp:View>

                    </asp:MultiView>

                </div>
            </div>
        </div>
    </div>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Message" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

    <insite:PageFooterContent runat="server">
        <script>
            (function () {
                var instance = window.messageCreate = window.messageCreate || {};

                instance.onJsonFileUploaded = function () {
                    __doPostBack('<%= UploadJsonFileUploaded.UniqueID %>', '')
                };
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
