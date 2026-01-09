<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Cmds.Admin.Organizations.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/CategoryGrid.ascx" TagName="CategoryGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/DistrictGrid.ascx" TagName="DistrictGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/DepartmentGrid.ascx" TagName="DepartmentGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/PersonGrid.ascx" TagName="PersonGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/ContactProfileEditor.ascx" TagName="ContactProfileEditor" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementListEditor.ascx" TagName="AchievementListEditor" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/CompanyDetail.ascx" TagName="CompanyDetail" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/CompanyTimeSensitiveCompetencies.ascx" TagName="CompanyTimeSensitiveCompetencies" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="CompanyInfo" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="OrganizationTab" Title="Organization" Icon="far fa-city" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Organization Information
                </h2>
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:CompanyDetail ID="CompanyDetail" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="DepartmentTab" Title="Departments" Icon="far fa-building" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Departments
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:DepartmentGrid ID="DepartmentGrid" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="DivisionTab" Title="Divisions" Icon="far fa-industry" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Divisions
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:DistrictGrid ID="DivisionGrid" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PeopleTab" Title="People" Icon="far fa-user" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    People
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:PersonGrid ID="PersonGrid" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ProfileTab" Title="Profiles" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Profiles
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:ContactProfileEditor ID="ProfileEditor" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CompetencyTab" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Competencies
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="mb-3">
                            <insite:Button runat="server" ID="CompanySkillEditorLink" Text="Edit competency priorities and levels in this organization's departments" Icon="fas fa-pencil" ButtonStyle="Default" />
                        </div>

                        <uc:CompanyTimeSensitiveCompetencies ID="CompanyTimeSensitiveCompetencies" runat="server" />

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AchievementTab" Title="Achievements" Icon="far fa-trophy" IconPosition="BeforeText">
            <section>
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:AchievementListEditor ID="AchievementEditor" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CategoryTab" Title="Categories" Icon="far fa-tag" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Categories
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:CategoryGrid ID="CategoryGrid" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="FileCleanupTab" Title="File Cleanup" Icon="far fa-bug" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    File Cleanup
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="FileCleanupUpdatePanel" />
                        <insite:UpdatePanel runat="server" ID="FileCleanupUpdatePanel">
                            <ContentTemplate>
                                <div class="mb-3">
                                    This system maintenance utility searches all CMDS libraries and 
                                    identifies any uploaded files that are no longer referenced by 
                                    any organization, competency, achievement, or training/education record.
                                    It then deletes these unreferenced files to free more available 
                                    disk space. Note this maintenance utility works on a maximum of 
                                    100 files at one time.
                                </div>

                                <insite:Button runat="server" ID="LoadRedundantFileButton" Text="Load Data" CssClass="mb-3" ButtonStyle="Default" />

                                <div runat="server" id="RedundantFilePanel" visible="false">
                                    <div class="mb-3">
                                        <insite:Button runat="server" ID="DeleteRedundantFileButton" Text="Cleanup" ButtonStyle="Default" />
                                        &nbsp;
                                        <asp:Literal runat="server" ID="FileCleanupCount" />
                                    </div>

                                    <asp:Repeater runat="server" ID="FileCleanupRepeater">
                                        <HeaderTemplate>
                                            <p>
                                                Here are the first 100 files in the list:
                                            </p>
                                            <ol>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <li class="pb-2">
                                                <a target="_blank" href='<%# Eval("Url") %>'><%# Eval("Name") %></a>
                                                (<%# Eval("Size") %>)
                                                <div class="form-text">
                                                    Uploaded <%# Eval("Posted") %> by <%# Eval("Author") %> for <%# Eval("Organization") %> 
                                                </div>
                                            </li>
                                        </ItemTemplate>
                                        <FooterTemplate></ol></FooterTemplate>
                                    </asp:Repeater>
                                    <p>
                                        <asp:Literal runat="server" ID="ResultLiteral" />
                                    </p>
                                </div>
                            </ContentTemplate>
                        </insite:UpdatePanel>
                    </div>
                </div>
            </section>
        </insite:NavItem>
    </insite:Nav>

    <div class="mt-3 float-end">
        
        <insite:Button runat="server" ID="ArchiveButton" Icon="far fa-archive" Text="Archive" ButtonStyle="Danger"
            ConfirmText="Are you sure you want to close this organization account (i.e. archive the organization)?" />

    </div>

    <div class="mt-3">

        <insite:Button runat="server" ID="UnarchiveButton" Icon="far fa-folder-open" Text="Unarchive" ButtonStyle="Default"
            ConfirmText="Are you sure you want to re-open this organization account (i.e. unarchive the organization)?" />

        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="CompanyInfo" />
    
        <insite:CancelButton runat="server" ID="CancelButton" />

    </div>

</asp:Content>