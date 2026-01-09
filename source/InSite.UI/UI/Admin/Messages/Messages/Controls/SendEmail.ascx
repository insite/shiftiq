<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SendEmail.ascx.cs" Inherits="InSite.UI.Admin.Messages.Messages.Controls.SendEmail" %>

<div class="row">
    <div class="col-lg-4">

        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">
                
                <h4>Schedule</h4>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Date and Time
                    </label>
                    <insite:DateTimeOffsetSelector runat="server" ID="MessageScheduleDate" />
                    <div class="form-text">
                        Messages can be scheduled for delivery a maximum of 3 days (72 hours) in the future.
                    </div>
                </div>

            </div>
        </div>
        
    </div>
    <div class="col-lg-8">
        
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4>Email</h4>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                <insite:UpdatePanel runat="server" ID="UpdatePanel">
                    <ContentTemplate>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Sender
                                    </label>
                                    <insite:SenderComboBox runat="server" ID="MessageSender" SenderType="Mailgun" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Message Template
                                    </label>
                                    <insite:ComboBox runat="server" ID="MessageTemplate" />
                                </div>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Recipient(s)
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="MessageRecipients" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Subject
                            </label>
                            <insite:TextBox runat="server" ID="MessageSubject" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Body
                            </label>
                            <insite:MarkdownEditor runat="server" ID="MessageBody" UploadControl="MessageUpload" />
                            <insite:EditorUpload runat="server" ID="MessageUpload" />
                        </div>

                        <div class="form-group mb-3">
                            <insite:Button runat="server" ID="MessageSendButton" Text="Send" ButtonStyle="Primary" Icon="fas fa-paper-plane" />
                        </div>

                        <div class="form-group mb-3">
                            <insite:Alert runat="server" ID="MessageStatus" />
                        </div>


                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>

    </div>
</div>