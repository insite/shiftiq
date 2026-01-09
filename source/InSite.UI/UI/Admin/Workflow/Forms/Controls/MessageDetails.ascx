<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MessageDetails.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.MessageDetails" %>

<insite:Alert runat="server" ID="NoInvitationMessage" Indicator="Warning" Visible="false">
    There is no invitation for this form.
</insite:Alert>

<div class="row mb-3">
    <div class="col-md-12">
        <insite:Button runat="server" ID="AddInvitationButton" Text="Create New Invitation" Icon="fas fa-plus-circle" ButtonStyle="Default" />
        <insite:Button runat="server" ID="AddNotificationButton" Text="Create New Notification" Icon="fas fa-plus-circle" ButtonStyle="Default" />
        <insite:Button runat="server" ID="ChangeMessageButton" Text="Configure Workflow" Icon="fas fa-pencil" ButtonStyle="Default" />
    </div>
</div>

<div class="row" runat="server" id="Field">
    <div class="col-md-12">
        <div class="settings">
            <div class="row">
                <div class="col-md-6">

                    <div class="form-group mb-3">
                        <asp:Label runat="server" ID="MessageModifiedLabel" AssociatedControlID="MessageModified" Text="Modified" CssClass="form-label" />
				        <div>
                            <asp:Literal runat="server" ID="MessageModified" />
                        </div>
                    </div>

                    <div class="form-group mb-4">
                        <div class="float-end">
                            <insite:IconLink Name="pencil" runat="server" ID="MessageSubjectLink" Style="padding: 8px" ToolTip="Change Message Subject" />
                        </div>
                        <asp:Label runat="server" ID="MessageSubjectLabel" AssociatedControlID="MessageSubject" Text="Subject" CssClass="form-label" />
                        <div>
                            <div style="float: left;">
                                <asp:HyperLink runat="server" ID="MessageLink">
                                    <asp:Literal runat="server" ID="MessageSubject" />
                                </asp:HyperLink>
                            </div>
                        </div>
                    </div>

                    <div class="form-group pt-2">
                        <div class="float-end">
                            <insite:IconLink Name="pencil" runat="server" ID="MessageContentLink" Style="padding: 8px" ToolTip="Edit Content of the Body of the Message" />
                        </div>
                        <asp:Label runat="server" ID="LabelMessageContent" AssociatedControlID="MessageMailoutCount" Text="Content Preview" CssClass="form-label" Visible="false" />
                        <div>
                            <insite:Alert runat="server" ID="ContentEmpty" />
                            <insite:Button runat="server" ID="EditContentButton" ButtonStyle="Primary" ToolTip="Edit the content for the body of the message" Icon="fas fa-pencil" Text="Edit" Visible="false"/>
                            <div runat="server" id="MessageContent" class="d-none"></div>
                        </div>
                    </div>

                </div>

                <div class="col-md-6">

                    <div class="form-group mb-3">
                        <div class="float-end">
                            <insite:IconLink Name="pencil" runat="server" ID="MessageSenderLink" Style="padding: 8px" ToolTip="Change Message Sender" />
                        </div>
                        <asp:Label runat="server" ID="MessageSenderLabel" AssociatedControlID="MessageSender" Text="Sender" CssClass="form-label" />
                        <div>
                            <asp:Literal runat="server" ID="MessageSender" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <div class="float-end">
                            <insite:IconLink Name="pencil" runat="server" ID="MessageRecipientsLink" Style="padding: 8px" ToolTip="Add Subscribers" />
                        </div>
                        <asp:Label runat="server" ID="MessageRecipientsLabel" AssociatedControlID="MessageRecipients" Text="Recipients" CssClass="form-label" />
                        <div>
                            <asp:Literal runat="server" ID="MessageRecipients" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <asp:Label runat="server" ID="MessageMailoutCountLabel" AssociatedControlID="MessageMailoutCount" Text="Mailouts" CssClass="form-label" />
                        <div>
                            <asp:Literal runat="server" ID="MessageMailoutCount" />
                        </div>
                    </div>

                </div>
            </div>

        </div>
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        inSite.common.contentToIFrame(document.getElementById('<%= MessageContent.ClientID %>'), {
            disableAnchors: true
        });
    </script>
</insite:PageFooterContent>