<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutputDetails.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.OutputDetails" %>

<div class="row">
    <div class="col-md-6">

        <div class="form-group mb-3">
            <label class="form-label">
                Sender
                <span class="ms-2">
                    <insite:IconLink Name="pencil" runat="server" ID="SenderEditLink" CssClass="pt-2" ToolTip="Change Message Sender" />
                </span>
            </label>
            <div>
                <asp:Literal runat="server" ID="Sender" />
            </div>
            <div class="form-text">
                Replies sent to this message will be delivered to this mailbox.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Subject
                <span class="ms-2">
                    <insite:IconLink Name="pencil" runat="server" id="SubjectEditLink" CssClass="pt-2" ToolTip="Change Message Subject" />
                </span>
            </label>
            <div>
                <asp:Literal runat="server" ID="Subject" />
            </div>
            <div class="form-text">
                Subject line on the email sent to recipients.
            </div>
        </div>

        <insite:UpdatePanel runat="server" CssClass="form-group mb-3">
            <ContentTemplate>
                <label class="form-label">
                    Message Status
                    <span class="ms-2">
                        <insite:IconButton runat="server" ID="DisableMessageButton" Name="toggle-on" />
                        <insite:IconButton runat="server" ID="EnableMessageButton" Name="toggle-off" />
                    </span>
                </label>
                <div>
                    <asp:Literal runat="server" ID="MessageStatus" />
                </div>
                <div runat="server" ID="MessageStatusDescription" class="form-text">
                </div>
            </ContentTemplate>
        </insite:UpdatePanel>

        <insite:UpdatePanel runat="server" ID="AutoBccSubscribersPanel" CssClass="form-group mb-3">
            <ContentTemplate>
                <label class="form-label">
                    Automatically BCC Subscribers
                    <span class="ms-2">
                        <insite:IconButton runat="server" ID="EnableAutoBccSubscribers" Name="toggle-off" />
                        <insite:IconButton runat="server" ID="DisableAutoBccSubscribers" Name="toggle-on" />
                    </span>
                </label>
                <div>
                    <asp:Literal runat="server" ID="AutoBccSubscribers" />
                </div>
                <div class="form-text">
                    Add subscribers to the BCC list whenever this message is sent.
                </div>
            </ContentTemplate>
        </insite:UpdatePanel>

    </div>
    <div class="col-md-6">

        <div class="form-group mb-3">
            <label class="form-label">
                Message Type
            </label>
            <div>
                <asp:Literal runat="server" ID="MessageType" />
            </div>
            <div class="form-text">
                The type of message you want to send.
            </div>
        </div>

        <div runat="server" id="NameField" class="form-group mb-3">
            <label class="form-label">
                Message Name
                <span class="ms-2">
                    <insite:IconLink Name="pencil" runat="server" ID="NameEditLink" CssClass="pt-2" ToolTip="Change Message Name" />
                </span>
            </label>
            <div>
                <asp:Literal runat="server" ID="Name" />
            </div>
            <div class="form-text">
                The message name is used as an internal reference for filing purposes.
            </div>
        </div>

        <div runat="server" id="NotificationField" class="form-group mb-3">
            <label class="form-label">
                Application Change Type
            </label>
            <div>
                <asp:Literal runat="server" ID="NotificationName" />
            </div>
            <div runat="server" id="NotificationDescription" class="form-text">
                The application change that triggers this notification message.
            </div>
        </div>

        <div runat="server" ID="SurveyField" class="form-group mb-3" visible="false">
            <label class="form-label">
                Form
            </label>
            <div>
                <asp:Literal runat="server" ID="SurveyName" />
            </div>
            <div class="form-text">
                Select the form that invitation recipients are expected to submit. 
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Message Identifier
            </label>
            <div>
                <asp:Literal runat="server" ID="MessageIdentifierOutput" />
            </div>
            <div class="form-text">
                A globally unique identifier for this form.
            </div>
        </div>

    </div>
</div>
