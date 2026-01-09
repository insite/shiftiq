<%@ Page CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Contacts.People.Forms.Edit" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/EditTabDetails.ascx" TagName="EditTabDetails" TagPrefix="uc" %>
<%@ Register Src="../Controls/EditTabOther.ascx" TagName="EditTabOther" TagPrefix="uc" %>
<%@ Register Src="../Controls/EditTabAttachments.ascx" TagName="EditTabAttachments" TagPrefix="uc" %>
<%@ Register Src="../Controls/EditTabSignIn.ascx" TagName="EditTabSignIn" TagPrefix="uc" %>

<%@ Register Src="../../Addresses/Controls/AddressList.ascx" TagName="AddressList" TagPrefix="uc" %>
<%@ Register Src="../Controls/UserConnectionGrid.ascx" TagName="UserConnectionGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/MembershipGrid.ascx" TagName="MembershipGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Submissions/Controls/SearchResults.ascx" TagName="SurveyResponses" TagPrefix="uc" %>
<%@ Register Src="../Controls/RegistrationGrid.ascx" TagName="RegistrationGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/UserScoreGrid.ascx" TagName="UserScoreGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/LogbookList.ascx" TagName="LogbookList" TagPrefix="uc" %>
<%@ Register Src="../Controls/OutcomeTree.ascx" TagName="OutcomeTree" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Records/Achievements/Controls/CredentialGrid.ascx" TagName="CredentialGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/UserOrganizationList.ascx" TagName="UserOrganizationList" TagPrefix="uc" %>
<%@ Register Src="../../Comments/Controls/CommentRepeater.ascx" TagName="CommentRepeater" TagPrefix="uc" %>
<%@ Register Src="../Controls/UserAuthenticationGrid.ascx" TagName="UserAuthenticationGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/ReferralGrid.ascx" TagName="ReferralGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UpdatePanel runat="server" ID="ScreenStatusUpdatePanel">
        <ContentTemplate>
            <insite:Alert runat="server" ID="ScreenStatus" />
            <insite:ValidationSummary runat="server" ValidationGroup="Person" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="PersonalTab" Title="Person" Icon="far fa-user" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <div class="float-start">
                        <insite:Nav runat="server" ContentRendererID="PersonalNavContent">

                            <insite:NavItem runat="server" Title="Details">
                                <uc:EditTabDetails runat="server" ID="DetailsTabContent" />
                            </insite:NavItem>

                            <insite:NavItem runat="server" Title="Other">
                                <uc:EditTabOther runat="server" ID="OtherTabContent" />
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="DocumentsTab" Title="Documents">
                                <uc:EditTabAttachments runat="server" ID="DocumentsTabContent" />
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="AddressSubTab" Title="Addresses">
                                <uc:AddressList runat="server" ID="AddressList" ContactType="User" ValidationGroup="Person" />
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="CommentSubTab" Title="Comments">
                                <uc:CommentRepeater runat="server" ID="CommentRepeater" />
                            </insite:NavItem>

                        </insite:Nav>
                    </div>
                    <insite:NavContent runat="server" ID="PersonalNavContent" />
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="MembershipTab" Title="Memberships" Icon="far fa-users" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <div class="float-end text-end mb-3">
                        <insite:Button runat="server" ButtonStyle="Default" OnClientClick="personEditor.onViewMembershipHistoryClick(); return false;" Text="History" Icon="fas fa-history" />
                    </div>

                    <insite:Nav runat="server">

                        <insite:NavItem runat="server" ID="GroupSubTab" Title="Group" Icon="far fa-users" IconPosition="BeforeText">
                            <uc:MembershipGrid runat="server" ID="GroupGrid" />
                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="ReferralSubTab" Title="Referrals" Icon="far fa-question" IconPosition="BeforeText">
                            <uc:ReferralGrid runat="server" ID="ReferralGrid" />
                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="PeopleSubTab" Title="People" Icon="far fa-sitemap" IconPosition="BeforeText">
                            <div class="row">
                                <div class="col-md-2 col-sm-3">

                                    <insite:Nav runat="server" ID="PeopleNav" ItemType="Pills" ItemAlignment="Vertical" ContentRendererID="PeopleNavContent">
                                        <insite:NavItem runat="server" ID="PeopleSubTabIncomingTab" Icon="fas fa-level-up-alt">
                                            <uc:UserConnectionGrid runat="server" ID="PeopleIncomingGrid" />
                                        </insite:NavItem>

                                        <insite:NavItem runat="server" ID="PeopleSubTabOutgoingTab" Icon="fas fa-level-down-alt" Title="Downstream">
                                            <uc:UserConnectionGrid runat="server" ID="PeopleOutgoingGrid" />
                                        </insite:NavItem>
                                    </insite:Nav>

                                </div>
                                <div class="col-md-10 col-sm-9">
                                    <div class="card">
                                        <div class="card-body overflow-auto">

                                            <insite:NavContent runat="server" ID="PeopleNavContent" />

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </insite:NavItem>

                    </insite:Nav>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="RecordTab" Title="Records" Icon="far fa-spell-check" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <insite:Nav runat="server">

                        <insite:NavItem runat="server" ID="GradebookSubTab" Title="Gradebooks" Icon="far fa-spell-check" IconPosition="BeforeText">
                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="GradebookUpdatePanel" />

                            <insite:UpdatePanel runat="server" ID="GradebookUpdatePanel">
                                <ContentTemplate>
                                    <uc:UserScoreGrid runat="server" ID="GradebookGrid" />
                                </ContentTemplate>
                            </insite:UpdatePanel>
                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="LogbookSubTab" Title="Logbooks" Icon="far fa-book-open" IconPosition="BeforeText">
                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="LogbookUpdatePanel" />

                            <insite:UpdatePanel runat="server" ID="LogbookUpdatePanel">
                                <ContentTemplate>
                                    <uc:LogbookList runat="server" ID="LogbookList" />
                                </ContentTemplate>
                            </insite:UpdatePanel>
                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="RegistrationSubTab" Title="Registrations" Icon="far fa-id-card" IconPosition="BeforeText">
                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="RegistrationUpdatePanel" />

                            <insite:UpdatePanel runat="server" ID="RegistrationUpdatePanel">
                                <ContentTemplate>
                                    <uc:RegistrationGrid runat="server" ID="RegistrationGrid" />
                                </ContentTemplate>
                            </insite:UpdatePanel>
                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="AchievementSubTab" Title="Achievements" Icon="far fa-trophy" IconPosition="BeforeText">
                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AchievementsUpdatePanel" />

                            <insite:UpdatePanel runat="server" ID="AchievementsUpdatePanel">
                                <ContentTemplate>
                                    <uc:CredentialGrid runat="server" ID="CredentialGrid" />
                                </ContentTemplate>
                            </insite:UpdatePanel>
                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="SurveySubTab" Title="Forms" Icon="far fa-check-square" IconPosition="BeforeText">
                            <div runat="server" id="NoSurveyResponses" class="alert alert-warning" role="alert">
                                There are no form submissions
                            </div>

                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SurveyResponseUpdatePanel" />

                            <insite:UpdatePanel runat="server" ID="SurveyResponseUpdatePanel">
                                <ContentTemplate>
                                    <uc:SurveyResponses runat="server" ID="SurveyResponseGrid" ScreenParams="contact&panel=forms" />
                                </ContentTemplate>
                            </insite:UpdatePanel>
                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="OutcomeSubTab" Title="Outcomes" Icon="far fa-ballot-check" IconPosition="BeforeText">
                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OutcomesUpdatePanel" />

                            <insite:UpdatePanel runat="server" ID="OutcomesUpdatePanel">
                                <ContentTemplate>
                                    <uc:OutcomeTree runat="server" ID="OutcomeTree" />
                                </ContentTemplate>
                            </insite:UpdatePanel>
                        </insite:NavItem>

                    </insite:Nav>
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SystemAccessTab" Title="System Access" Icon="far fa-cogs" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    
                    <div class="float-start">
                        <insite:Nav runat="server" ContentRendererID="SystemAccessNavContent">

                            <insite:NavItem runat="server" ID="SignInSubTab" Title="Sign In" Icon="far fa-user-lock" IconPosition="BeforeText">
                                <uc:EditTabSignIn runat="server" ID="SignInTabContent" />
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="OrganizationSubTab" Title="Organizations" Icon="far fa-city" IconPosition="BeforeText">
                                <div class="clearfix"></div>

                                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OrganizationUpdatePanel" />

                                <insite:UpdatePanel runat="server" ID="OrganizationUpdatePanel">
                                    <ContentTemplate>
                                        <uc:UserOrganizationList runat="server" ID="OrganizationList" />
                                    </ContentTemplate>
                                </insite:UpdatePanel>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="AuthenticationSubTab" Title="Authentications" Icon="far fa-sign-in-alt" IconPosition="BeforeText">
                                <div class="clearfix"></div>

                                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AuthenticationsUpdatePanel" />

                                <insite:UpdatePanel runat="server" ID="AuthenticationsUpdatePanel">
                                    <ContentTemplate>
                                        <uc:UserAuthenticationGrid runat="server" ID="AuthenticationGrid" />
                                    </ContentTemplate>
                                </insite:UpdatePanel>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="SettingsSubTab" Title="Settings" Icon="far fa-cogs" IconPosition="BeforeText">
                                <div class="clearfix"></div>

                                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SettingsUpdatePanel" />

                                <div class="row">
                                    <div class="col-lg-6">

                                        <div class="card h-100">
                                            <div class="card-body">
                                                <insite:UpdatePanel runat="server" ID="SettingsUpdatePanel">
                                                    <ContentTemplate>
                                                        <div runat="server" id="AccountVisibilityField" class="form-group mb-3">
                                                            <label class="form-label">
                                                                Account Visibility
                                                            </label>
                                                            <div>
                                                                <insite:CheckBox ID="IsUserAccountCloaked" runat="server" Text="Account Cloaked" />
                                                            </div>
                                                            <div class="form-text">
                                                                A cloaked user is invisible to administrators (unless they are cloaked themselves).
                                                            </div>
                                                        </div>

                                                        <div runat="server" id="AccountSettingsField" class="form-group mb-3">
                                                            <label class="form-label">
                                                                Account Settings
                                                            </label>
                                                            <div>
                                                                <insite:CheckBox runat="server" ID="IsGroupNavigationItems" Text="Group navigation menu items in the Admin UI" />
                                                            </div>
                                                        </div>
                                                    </ContentTemplate>
                                                </insite:UpdatePanel>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </insite:NavItem>

                        </insite:Nav>
                    </div>

                    <insite:NavContent runat="server" ID="SystemAccessNavContent" />
                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

    <div class="row mt-3">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Person" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

    <insite:Modal runat="server" ID="HistoryViewerWindow" Title="History" Width="710px" MinHeight="520px" />

    <insite:Modal runat="server" ID="ResetPasswordWindow" Title="Reset Password" Width="600px">
        <ContentTemplate>
            <div class="px-1">
                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ResetPasswordUpdatePanel" />

                <insite:UpdatePanel runat="server" ID="ResetPasswordUpdatePanel">
                    <ContentTemplate>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Password
                                <insite:RequiredValidator runat="server" ControlToValidate="ResetPasswordInput" ValidationGroup="ResetPassword" />
                            </label>
                            <insite:TextBox runat="server" ID="ResetPasswordInput" Width="100%" AllowHtml="true" />
                            <div class="form-text">
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Confirm Password
                                <insite:CompareValidator runat="server" ControlToValidate="ResetPasswordConfirm" ControlToCompare="ResetPasswordInput" ValidationGroup="ResetPassword" ErrorMessage="Passwords do not match" />
                            </label>
                            <insite:TextBox runat="server" ID="ResetPasswordConfirm" Width="100%" AllowHtml="true" />
                            <div class="form-text">
                            </div>
                        </div>

                        <div>
                            <insite:SaveButton runat="server" ID="ResetPasswordButton" ValidationGroup="ResetPassword" />
                            <insite:CancelButton runat="server" data-bs-dismiss="modal" />
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>
            </div>
        </ContentTemplate>
    </insite:Modal>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            (function () {
                var personEditor = window.personEditor = window.personEditor || {};

                personEditor.onViewPersonHistoryClick = function () {
                    modalManager.load('<%= HistoryViewerWindow.ClientID %>', '/ui/admin/reports/changes/history?id=<%= UserId %>&type=user');
                };

                personEditor.onViewMembershipHistoryClick = function () {
                    modalManager.load('<%= HistoryViewerWindow.ClientID %>', '/ui/admin/reports/changes/history?id=<%= UserId %>&type=user_membership');
                };

                personEditor.onResetPasswordClick = function () {
                    document.getElementById('<%= ResetPasswordInput.ClientID %>').value = '';
                    document.getElementById('<%= ResetPasswordConfirm.ClientID %>').value = '';
                    document.getElementById('<%= ResetPasswordUpdatePanel.ClientID %>').ajaxRequest('init');
                    modalManager.show('<%= ResetPasswordWindow.ClientID %>');
                };

                Sys.Application.add_load(function () {
                    $('#<%= ResetPasswordInput.ClientID %>')
                        .off('keyup', onPasswordKeyUp)
                        .on('keyup', onPasswordKeyUp);
                });

                function onPasswordKeyUp() {
                    document.getElementById('<%= ResetPasswordConfirm.ClientID %>').value = '';
                }
            })();

        </script>
    </insite:PageFooterContent>

</asp:Content>
