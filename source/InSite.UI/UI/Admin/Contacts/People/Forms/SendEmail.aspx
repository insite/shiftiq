<%@ Page CodeBehind="SendEmail.aspx.cs" Inherits="InSite.UI.Admin.Contacts.People.Forms.SendEmail" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="OutputContent" Src="~/UI/Admin/Messages/Messages/Controls/FieldOutputContent.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

    <insite:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>

            <insite:Alert runat="server" ID="ScreenStatus" />
            <insite:ValidationSummary runat="server" ValidationGroup="Compose" />
            <insite:ValidationSummary runat="server" ValidationGroup="Confirm" />

            <insite:Nav runat="server">
                <insite:NavItem runat="server" ID="ComposeTab" Icon="far fa-file-alt" Title="Compose">
                    <section class="my-3">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <div class="row">
                                    <div class="col-lg-6">
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Sender
                                                <insite:RequiredValidator runat="server" ControlToValidate="ComposeSenderIdentifier" FieldName="Sender" ValidationGroup="Compose" />
                                            </label>
                                            <insite:SenderComboBox runat="server" ID="ComposeSenderIdentifier" />
                                            <div class="form-text" runat="server" id="ComposeSenderDescription">
                                            </div>
                                            <div runat="server" id="SenderInstruction" class="form-text"></div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Recipient
                                            </label>
                                            <div runat="server" id="ComposeRecipientOutput" style="line-height: 40px;"></div>
                                        </div>

                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-group mb-3" runat="server" id="MessageTemplateField" visible="false">
                                            <label class="form-label">
                                                Message Template
                                            </label>
                                            <insite:ComboBox runat="server" ID="MessageTemplateCombobox" />
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Subject
                                        <insite:RequiredValidator runat="server" ControlToValidate="ComposeEmailSubject" FieldName="Message Subject" ValidationGroup="Compose" />
                                    </label>
                                    <insite:TextBox runat="server" ID="ComposeEmailSubject" MaxLength="255" />
                                </div>

                                <div>
                                    <insite:RequiredValidator runat="server" ControlToValidate="ComposeEmailBody" FieldName="Message Content" ValidationGroup="Compose" Display="None" RenderMode="Exclamation" />
                                    <insite:MarkdownEditor runat="server" ID="ComposeEmailBody" />
                                </div>

                            </div>
                        </div>
                    </section>

                    <div>
                        <insite:NextButton runat="server" ID="ComposeNextButton" ValidationGroup="Compose" />
                        <insite:CancelButton runat="server" ID="ComposeCancelButton" />
                    </div>
                </insite:NavItem>
                <insite:NavItem runat="server" ID="ConfirmTab" Icon="far fa-envelope-open-text" Title="Confirm" Visible="false">
                    <section class="my-3">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Sender
                                    </label>
                                    <div runat="server" id="ConfirmSenderOutput"></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Recipient
                                    </label>
                                    <div runat="server" id="ConfirmRecipientOutput"></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Subject
                                    </label>
                                    <div runat="server" id="ConfirmSubjectOutput"></div>
                                </div>

                                <div class="card mt-4">
                                    <div class="card-body">
                                        <uc:OutputContent runat="server" ID="ConfirmEmailBody" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </section>

                    <div>
                        <insite:Button runat="server" ID="ConfirmSendButton" Text="Send Message" Icon="fas fa-paper-plane" ButtonStyle="Success" ValidationGroup="Confirm" />
                        <insite:CancelButton runat="server" ID="ConfirmCancelButton" />
                    </div>
                </insite:NavItem>
            </insite:Nav>

        </ContentTemplate>
    </insite:UpdatePanel>

</asp:Content>
