<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Events.Classes.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/RegistrationGrid.ascx" TagName="RegistrationGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/SeatGrid.ascx" TagName="SeatGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/GradebookGrid.ascx" TagName="GradebookGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/MultilingualStringInfo.ascx" TagName="MultilingualStringInfo" TagPrefix="uc" %>
<%@ Register Src="../../Comments/Controls/CommentPanel.ascx" TagName="CommentPanel" TagPrefix="uc" %>
<%@ Register Src="../Controls/PrivacyTab.ascx" TagName="PrivacyTab" TagPrefix="uc" %>
<%@ Register Src="../Controls/ClassNotificationTab.ascx" TagName="ClassNotificationTab" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Nav runat="server">               
        <insite:NavItem runat="server" ID="RegistrationTab" Title="Registrations" Icon="far fa-id-card" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="GridUpdatePanel" />
                                <insite:UpdatePanel runat="server" ID="GridUpdatePanel">
                                    <ContentTemplate>
                                        <uc:RegistrationGrid runat="server" ID="Registrations" />
                                    </ContentTemplate>
                                </insite:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        
        <insite:NavItem runat="server" ID="GradebookTab" Title="Gradebooks" Icon="far fa-spell-check" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <h2 class="h4 mb-1"></h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div class="mb-4">
                                    <div class="d-flex">
                                        <div>
                                            <insite:Button runat="server" ID="AddGradebook" Text="New Gradebook" Icon="fas fa-plus-circle" ButtonStyle="OutlinePrimary" />
                                            <insite:Button runat="server" ID="FindGradebook" Text="Find Gradebook" Icon="fas fa-search" ButtonStyle="OutlinePrimary" />
                                        </div>
                                        <div class="ms-1 flex-fill">
                                            <insite:GradebookComboBox runat="server" ID="FindGradebookIdentifier" Visible="false" />
                                        </div>
                                        <div class="ms-1">
                                            <insite:Button runat="server" ID="AddGradebookIdentifier" Icon="far fa-plus-circle" ButtonStyle="Default" Visible="false" />
                                        </div>
                                    </div>
                                    
                                </div>

                                <uc:GradebookGrid runat="server" ID="GradebookGrid" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SeatTab" Title="Seats" Icon="far fa-money-check-alt" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <h2 class="h4 mb-1"></h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div class="text-end" style="padding-bottom:15px;">
                                    <insite:Button runat="server" ID="AddSeat" Text="Add Seat" Icon="fas fa-plus-circle" ButtonStyle="OutlinePrimary" />
                                </div>

                                <uc:SeatGrid runat="server" ID="SeatsGrid" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ContentTab" Title="Content" Icon="far fa-edit" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <h2 class="h4 mb-1"></h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div style="position:absolute;right:var(--ar-card-spacer-x);z-index:1;">
                                    <div>
                                        <insite:Button runat="server" ID="PreviewLink" Text="Preview" Icon="fas fa-external-link" ButtonStyle="OutlinePrimary" NavigateTarget="_blank" />
                                    </div>
                                </div>

                                <div class="row content-details">
                                    <div class="col-md-12">

                                        <insite:Nav runat="server" ID="ContentNavigation">

                                            <insite:NavItem runat="server" ID="ContentTitleTab" Title="Title">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" id="EditContentTitleLink" ToolTip="Revise Title" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentTitle" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="ContentSummaryTab" Title="Summary">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" id="EditContentSummaryLink" ToolTip="Revise Summary" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentSummary" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="ContentDescriptionTab" Title="Description">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" id="EditContentDescriptionLink" ToolTip="Revise Description"  ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentDescription" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="ContentMaterialsTab" Title="Materials">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" id="EditContentMaterialsLink" ToolTip="Revise Materials for Participation"   ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentMaterials" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="ContentInstructionsTab" Title="Instructions">
                                                <div class="card">
                                                    <div class="card-body">
                                                        <insite:Nav runat="server">
                                                            <insite:NavItem runat="server" ID="ContentInstructionsContactTab" Title="Contact and Support">
                                                                <div class="content-cmds">
                                                                    <insite:Button runat="server" id="EditContentContactLink" ToolTip="Revise Contact and Support Instructions"  ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                                </div>
                                                                <div class="content-string">
                                                                    <uc:MultilingualStringInfo runat="server" ID="ContentContact" />
                                                                </div>
                                                            </insite:NavItem>
                                                            <insite:NavItem runat="server" ID="ContentInstructionsAccommodationTab" Title="Accommodation">
                                                                <div class="content-cmds">
                                                                    <insite:Button runat="server" id="EditContentAccommodationLink" ToolTip="Revise Accommodation Instructions"  ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                                </div>
                                                                <div class="content-string">
                                                                    <uc:MultilingualStringInfo runat="server" ID="ContentAccommodation" />
                                                                </div>
                                                            </insite:NavItem>
                                                            <insite:NavItem runat="server" ID="ContentInstructionsAdditionalTab" Title="Additional">
                                                                <div class="content-cmds">
                                                                    <insite:Button runat="server" id="EditContentAdditionalLink" ToolTip="Revise Additional Instructions"  ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                                </div>
                                                                <div class="content-string">
                                                                    <uc:MultilingualStringInfo runat="server" ID="ContentAdditional" />
                                                                </div>
                                                            </insite:NavItem>
                                                            <insite:NavItem runat="server" ID="ContentInstructionsCancellationTab" Title="Cancellation and Refund">
                                                                <div class="content-cmds">
                                                                    <insite:Button runat="server" id="EditContentCancellationLink" ToolTip="Revise Cancellation and Refund Instructions"  ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                                </div>
                                                                <div class="content-string">
                                                                    <uc:MultilingualStringInfo runat="server" ID="ContentCancellation" />
                                                                </div>
                                                            </insite:NavItem>
                                                            <insite:NavItem runat="server" ID="ContentInstructionsCompletionTab" Title="Registration Completion">
                                                                <div class="content-cmds">
                                                                    <insite:Button runat="server" id="EditContentCompletionLink" ToolTip="Revise Registration Completion Instructions"  ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                                </div>
                                                                <div class="content-string">
                                                                    <uc:MultilingualStringInfo runat="server" ID="ContentCompletion" />
                                                                </div>
                                                            </insite:NavItem>
                                                        </insite:Nav>
                                                    </div>
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="ContentClassLinkTab" Title="Class Link">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" id="EditContentClassLinkLink" ToolTip="Revise Class Link" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentClassLink" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="ContentSurveyTab" Title="Forms">
                                                <div class="row">
                                                    <div class="col-xl-6">
                                                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ContentSurveyUpdatePanel" />

                                                        <insite:UpdatePanel runat="server" ID="ContentSurveyUpdatePanel">
                                                            <ContentTemplate>
                                                                <div class="form-group mb-3">
                                                                    <asp:Label runat="server" AssociatedControlID="MandatorySurveyFormIdentifier" Text="Mandatory Form Submission" CssClass="form-label" />
                                                                    <div>
                                                                        <insite:FindWorkflowForm runat="server" ID="MandatorySurveyFormIdentifier" />
                                                                    </div>
                                                                    <div class="form-text">
                                                                        If a mandatory form is added, users must submit a submission before proceeding with their registration.
                                                                    </div>
                                                                </div>
                                                            </ContentTemplate>
                                                        </insite:UpdatePanel>
                                                    </div>
                                                </div>
                                            </insite:NavItem>
                                        </insite:Nav>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CommentTab" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <h2 class="h4 mb-1"></h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <uc:CommentPanel ID="CommentTabControl" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PrivacyTab" Title="Privacy" Icon="far fa-shield" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <uc:PrivacyTab runat="server" ID="PrivacyTabControl" />
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="NotificationTab" Title="Notifications" Icon="far fa-bell" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <uc:ClassNotificationTab runat="server" ID="NotificationTabControl" />
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ClassTab" Title="Class Setup" Icon="far fa-calendar-alt" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <h2 class="h4 mb-1"></h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <div>
                                    <insite:Button runat="server" ID="NewClassLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/events/classes/create" CssClass="mb-3" />

                                    <insite:ButtonSpacer runat="server" ID="Separator1" />

                                    <insite:Button runat="server" ID="CopyLink" Text="Duplicate" ButtonStyle="Default" Icon="fas fa-copy" CssClass="mb-3" />
                                    <insite:Button runat="server" ID="PublishLink" Text="Publish" Icon="fas fa-upload" ButtonStyle="Default" CssClass="mb-3" />
                                    <insite:Button runat="server" ID="UnpublishLink" Text="Unpublish" Icon="fas fa-eraser" ButtonStyle="Default" CssClass="mb-3" />
                                    <insite:Button runat="server" ID="CancelLink" Text="Cancel" Icon="fas fa-archive" ButtonStyle="Default" CssClass="mb-3" />

                                    <insite:ButtonSpacer runat="server" ID="Separator2" />

                                    <insite:Button runat="server" ID="DownloadJsonLink" Text="Download JSON" icon="fas fa-download" ButtonStyle="Primary" CssClass="mb-3" />
                                    <insite:Button runat="server" ID="ViewHistoryLink"  Text="History" Icon="fas fa-history" ButtonStyle="Default" CssClass="mb-3" />
                                    <insite:DownloadButton runat="server" ID="DownloadLink" Visible="false" CssClass="mb-3" />
                                    <insite:DeleteButton runat="server" ID="DeleteLink" CssClass="mb-3" />
                                </div>

                                <insite:DynamicControl runat="server" ID="ClassTabContent" />

                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .content-details {

        }

            .content-details .content-cmds {
                position: absolute;
                right: var(--ar-card-spacer-x);
            }

            .content-details .content-string {
                padding-right: 80px;
                min-height: 50px;
            }
    </style>
</insite:PageHeadContent>
</asp:Content>
