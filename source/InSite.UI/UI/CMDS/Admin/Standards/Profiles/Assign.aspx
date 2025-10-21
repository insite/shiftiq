<%@ Page Language="C#" CodeBehind="Assign.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.Profiles.PersonProfile" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/AssignCompanyProfilesToEmployee.ascx" TagName="AssignCompanyProfilesToEmployee" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AssignSelectedProfilesToEmployees.ascx" TagName="AssignSelectedProfilesToEmployees" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AssignProfileToSelectedEmployees.ascx" TagName="AssignProfileToSelectedEmployees" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="ScreenStatus" />

            <insite:ValidationSummary runat="server" ValidationGroup="AssignCompanyProfilesToEmployee" />
            <insite:ValidationSummary runat="server" ValidationGroup="Department" />
        </ContentTemplate>
    </insite:UpdatePanel>


    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" Title="Assign Organization Profiles to Person" Icon="far fa-user" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Assign Organization Profiles to Person
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ProfilesToEmployeeUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="ProfilesToEmployeeUpdatePanel">
                            <ContentTemplate>
                                <uc:AssignCompanyProfilesToEmployee ID="AssignCompanyProfilesToEmployee" runat="server" />
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" Title="Assign Secondary Profiles to People" Icon="far fa-users" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Assign Secondary Profiles to People
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ProfilesToEmployeesUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="ProfilesToEmployeesUpdatePanel">
                            <ContentTemplate>
                                <uc:AssignSelectedProfilesToEmployees ID="AssignSelectedProfilesToEmployees" runat="server" />
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" Title="Assign Organization Profiles to Multiple People" Icon="far fa-cog" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Assign Organization Profiles to Multiple People
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SelectedEmployeesUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="SelectedEmployeesUpdatePanel">
                            <ContentTemplate>
                                <uc:AssignProfileToSelectedEmployees ID="AssignProfileToSelectedEmployees" runat="server" />
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>
    
    <div class="mt-3">
        <insite:CloseButton runat="server" NavigateUrl="/ui/admin/tools" />
    </div>

</asp:Content>
