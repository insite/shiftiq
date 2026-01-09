<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Messages.Outlines.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ContactGrid" Src="../../Subscribers/Controls/ContactGrid.ascx" %>

<%@ Register TagPrefix="uc" TagName="ScheduledGrid" Src="../../Mailouts/Controls/ScheduledGrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="CompletedGrid" Src="../../Mailouts/Controls/CompletedGrid.ascx" %>

<%@ Register TagPrefix="uc" TagName="OutputContent" Src="../../Messages/Controls/FieldOutputContent.ascx" %>
<%@ Register TagPrefix="uc" TagName="ValidPlaceHolderNames" Src="../Controls/ValidPlaceHolderNames.ascx" %>

<%@ Register TagPrefix="uc" TagName="MailLinkGrid" Src="../../Messages/Controls/MailLinkGrid.ascx" %>

<%@ Register TagPrefix="uc" TagName="Details" Src="../../Messages/Controls/OutputDetails.ascx" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Message" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="ContentTab" Title="Content" Icon="far fa-edit" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <insite:Alert runat="server" ID="ContentEmpty" />

                    <div class="row">
                        <div class="col">
                            <insite:Button runat="server" ID="EditContentButton" ButtonStyle="Primary" ToolTip="Edit the content for the body of the message" Icon="fas fa-pencil" Text="Edit" />
                        </div>
                        <div class="col text-end">
                            <insite:Button runat="server" ID="DuplicateButton2" ButtonStyle="Default" Text="Duplicate to New Message" Icon="fas fa-copy" CssClass="mb-3" />
                        </div>
                    </div>

                    <div class="my-3">
                        <uc:OutputContent runat="server" ID="MessageContent" />
                    </div>

                    <uc:ValidPlaceHolderNames runat="server" ID="ValidPlaceHolderNames" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SubscriberTab" Title="Subscribers" Icon="far fa-users" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <uc:ContactGrid runat="server" ID="SubscriberGrid" />

                    <div class="row">
                        <div runat="server" id="DomainGridPanel" class="col-lg-4">
                            <div class="card mb-3">
                                <div class="card-body">
                                    <h3 class="card-title mb-3">Domain Summary</h3>

                                    <insite:Grid runat="server" ID="DomainGrid">
                                        <Columns>
                                            <asp:BoundField HeaderText="Domain" DataField="Name" />
                                            <asp:BoundField HeaderText="Subscribers" DataField="Value" DataFormatString="{0:n0}" ItemStyle-Width="100" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end" />
                                        </Columns>
                                    </insite:Grid>
                                </div>
                            </div>
                        </div>
                        <div runat="server" id="InvitationRespondents" class="col-lg-8" visible="false">
                            <div class="card mb-3">
                                <div class="card-body">
                                    <h3 class="card-title mb-3">Form Respondents</h3>

                                    <insite:DownloadButton runat="server" ID="DownloadRespondentsCsv" />
                                    <insite:DeleteButton runat="server" ID="DeleteRespondents" OnClientClick="return confirm('Are you sure you want to remove from this message the recipients who have answered the form?');" />
                                    &nbsp;
                                    <asp:Literal runat="server" ID="RespondentsCount" />
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="MailoutTab" Title="Mailouts" Icon="far fa-mail-bulk" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <insite:Button runat="server" ID="ScheduleMailoutButton1" ButtonStyle="Default" Text="Schedule Mailout" Icon="fas fa-calendar-alt" CssClass="mb-3" />

                    <div class="mb-3">
                        <uc:ScheduledGrid runat="server" ID="ScheduledGrid" />
                    </div>

                    <div class="mb-3">
                        <uc:CompletedGrid runat="server" ID="CompletedGrid" />
                    </div>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="LinkTab" Title="Links" Icon="far fa-link" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <uc:MailLinkGrid runat="server" ID="MailLinks" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="MessageTab" Title="Message Setup" Icon="far fa-paper-plane" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <div>
                        <insite:Button runat="server" ID="NewMessageButton" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/messages/create" CssClass="mb-3" />

                        <insite:ButtonSpacer runat="server" />

                        <insite:Button runat="server" ID="DuplicateButton" ButtonStyle="Default" Text="Duplicate" Icon="fas fa-copy" CssClass="mb-3" />
                        <insite:Button runat="server" ID="ScheduleMailoutButton2" ButtonStyle="Default" Text="Schedule Mailout" Icon="fas fa-calendar-alt" CssClass="mb-3" />

                        <insite:ButtonSpacer runat="server" ID="DuplicateButtonSpacer" />

                        <insite:Button runat="server" ID="DownloadButton" Text="Download JSON" icon="fas fa-download" ButtonStyle="Default" CssClass="mb-3" />
                        <insite:Button runat="server" ID="ViewHistoryButton" Text="History" Icon="fas fa-history" ButtonStyle="Default" CssClass="mb-3" />
                        <insite:DeleteButton runat="server" ID="DeleteButton" CssClass="mb-3" />
                    </div>

                    <div class="card mb-5">
                        <div class="card-body">
                            <uc:Details runat="server" ID="Details" />
                        </div>
                    </div>

                    <asp:Literal runat="server" ID="ProgressGuide" />
                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>