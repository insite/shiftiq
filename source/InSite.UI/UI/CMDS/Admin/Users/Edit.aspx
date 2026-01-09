<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Cmds.Admin.People.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/PersonDetails.ascx" TagName="PersonDetails" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/UserConnections.ascx" TagName="UserConnections" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/UserDepartments.ascx" TagName="UserDepartments" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/EmploymentGrid.ascx" TagName="EmploymentGrid" TagPrefix="uc" %>
<%@ Register Src="./Controls/NotificationsTab.ascx" TagName="NotificationsTab" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="ContactInfo" />
    <insite:ValidationSummary runat="server" ValidationGroup="PersonMembership" />
    <insite:ValidationSummary runat="server" ValidationGroup="PersonRelationship" />
    
    <div runat="server" id="NoPerson" visible="false" class="alert alert-warning" role="alert">
        <i class="fas fa-exclamation-triangle"></i> <strong>Warning</strong>
        <br />
        This person is not a registered user in <b><asp:Literal runat="server" ID="CurrentOrganizationName" /></b>.
        This person is registered in:
        <ul class="mb-0">
            <asp:Repeater runat="server" ID="OrganizationRepeater">
                <ItemTemplate>
                    <li><b><%# (string)Container.DataItem %></b></li>
                </ItemTemplate>
            </asp:Repeater>            
        </ul>
    </div>

    <div runat="server" id="NoRolesWarningPanel" class="alert alert-warning alert-dismissible" role="alert">
        <asp:Literal ID="NoRolesWarning" runat="server" />
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    </div>

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="PersonTab" Title="Person" Icon="far fa-user" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Person
                </h2>

                <div class="mb-3">
                    <insite:Button runat="server" ID="ArchiveButton" Icon="far fa-archive" Text="Archive" ButtonStyle="Danger" 
                        ConfirmText="Are you sure you want to archive this person?" />
                    <insite:Button runat="server" ID="UnarchiveButton" Icon="far fa-folder-open" Text="Unarchive" ButtonStyle="Default"
                        ConfirmText="Are you sure you want to unarchive this person?" />
                    <insite:ButtonSpacer runat="server" ID="ArchiveButtonSpacer" />
                    <insite:Button runat="server" ID="ViewHistoryButton" ButtonStyle="Default" Text="History" Icon="far fa-history" />
                    <insite:ButtonSpacer runat="server" ID="AdditionalLinksSpacer" />
                    <insite:Button runat="server" ID="EducationLinkButton" ButtonStyle="Default" Text="Education" />
                    <insite:Button runat="server" ID="ProfilesLinkButton" ButtonStyle="Default" Text="Profiles" />
                    <insite:Button runat="server" ID="CompetenciesLinkButton" ButtonStyle="Default" Text="Competencies" />
                    <insite:Button runat="server" ID="TrainingPlanLinkButton" ButtonStyle="Default" Text="Training Plan" />
                    <insite:ButtonSpacer runat="server" ID="ButtonSpacer1" />
                    <insite:Button runat="server" ID="ImpersonateLink" Text="Impersonate User" Icon="fas fa-user-secret" ButtonStyle="Default" />
                    <insite:Button runat="server" ID="ResetPasswordButton" Text="Reset Password" Icon="fas fa-sync" ButtonStyle="Danger" />
                </div>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <uc:PersonDetails ID="PersonDetails" runat="server" />

                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ProfileTab" Title="Primary Profiles" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Primary Profiles
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="EmploymentUpdatePanel" />
                        <insite:UpdatePanel runat="server" ID="EmploymentUpdatePanel">
                            <ContentTemplate>
                                <uc:EmploymentGrid ID="EmploymentGrid" runat="server" />
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CompanyTab" Title="Organizations and Departments" Icon="far fa-city" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Organizations and Departments
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CompaniesUpdatePanel" />
	                    <insite:UpdatePanel runat="server" ID="CompaniesUpdatePanel">
	                        <ContentTemplate>
                                <uc:UserDepartments ID="UserDepartments" runat="server" />
	                        </ContentTemplate>
	                    </insite:UpdatePanel>

                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ManagerTab" Title="Reporting Lines" Icon="far fa-users" IconPosition="BeforeText">
            <section>

                <h2 class="h4 mt-4 mb-3">
                    Reporting Lines
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:UserConnections ID="UserConnections" runat="server" />
                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="NotificationTab" Title="Notifications" Icon="far fa-paper-plane" IconPosition="BeforeText">
            <uc:NotificationsTab runat="server" ID="NotificationsTab" />
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="ContactInfo" />
        <insite:DeleteButton runat="server" ID="DeleteButton" CausesValidation="false" ConfirmText="Are you sure you want to delete this person?" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

    <insite:Modal runat="server" ID="HistoryViewerWindow" Title="Change History" Width="710px" MinHeight="520px" />

    <insite:PageFooterContent runat="server">

        <script type="text/javascript">

            function showHistory(entityId) {
                modalManager.load('<%= HistoryViewerWindow.ClientID %>', '/ui/admin/reports/changes/history?id=' + entityId + '&type=user');
            };

        </script>

    </insite:PageFooterContent>
</asp:Content>