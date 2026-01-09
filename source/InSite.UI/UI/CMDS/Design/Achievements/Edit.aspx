<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Cmds.Admin.Achievements.Forms.Edit2" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementDetails.ascx" TagName="AchievementDetails" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Contacts/Groups/Controls/DepartmentChecklist.ascx" TagName="DepartmentChecklist" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <asp:ValidationSummary runat="server" ValidationGroup="ContactInfo" />

    <insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

        <insite:NavItem runat="server" ID="AchievementSection" Title="Achievement" Icon="far fa-trophy" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Achievement
                </h2>

                <div class="mb-3">
                    <insite:Button runat="server" ID="ViewReferencesButton" ButtonStyle="Default" Text="View References" Icon="far fa-network-wired" />
                </div>

                <uc:AchievementDetails ID="AchievementDetails" runat="server" />
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CompetencySection" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Competencies
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CompetencyUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="CompetencyUpdatePanel">
                            <ContentTemplate>
                                <insite:Nav runat="server">

                                    <insite:NavItem runat="server" ID="CompetencyTab" Title="Competencies">
                                        <div runat="server" id="CompetencyPanel">
                                            <asp:Repeater ID="Competencies" runat="server">
                                                <ItemTemplate>
                                                    <div class="mb-2">
                                                        <asp:Literal ID="CompetencyStandardIdentifier" runat="server" Text='<%# Eval("CompetencyStandardIdentifier") %>' Visible="false" />
                                                        <asp:CheckBox runat="server" ID="Competency" />
                                                        <a href='<%# "/ui/cmds/admin/standards/competencies/edit?id=" + Eval("CompetencyStandardIdentifier") %>'><%# Eval("Number") %></a>
                                                        <asp:Label runat="server" AssociatedControlID="Competency" Text='<%# Eval("Summary") %>' />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>

                                        <div runat="server" id="CompetencyButtons">
                                            <insite:Button ID="SelectAllButton" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                                            <insite:Button ID="UnselectAllButton" runat="server" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                                            <insite:DeleteButton ID="DeleteCompetencyButton" runat="server" />
                                        </div>
                                    </insite:NavItem>

                                    <insite:NavItem runat="server" ID="NewCompetencyTab" Title="Add Competencies">
                                        <p>Search for the individual competencies you want to add to this achievement. Check the box next to each one and click the Add button.</p>

                                        <div class="mb-3 w-25">
                                            <cmds:FindAchievement2 ID="SearchAchievement" runat="server" EmptyMessage="Select an achievement" />
                                        </div>
                                        <div class="mb-3 w-25">
                                            <insite:TextBox ID="SearchText" runat="server" EmptyMessage="Competency Number or Summary" />
                                        </div>
                                        <div class="mb-3">
                                            <insite:FilterButton ID="FilterButton" runat="server" ButtonStyle="Default" />
                                            <insite:ClearButton ID="ClearButton" runat="server" ButtonStyle="Default" />
                                        </div>

                                        <p id="FoundCompetency" runat="server" visible="false"></p>

                                        <div id="CompetencyList" runat="server" visible="false">
                                            <asp:Repeater ID="NewCompetencies" runat="server">
                                                <ItemTemplate>
                                                    <div class="mb-2">
                                                        <asp:Literal ID="CompetencyStandardIdentifier" runat="server" Text='<%# Eval("CompetencyStandardIdentifier") %>' Visible="false" />
                                                        <asp:CheckBox ID="Competency" runat="server" />
                                                        <a href='<%# "/ui/cmds/admin/standards/competencies/edit?id=" + Eval("CompetencyStandardIdentifier") %>'><%# Eval("Number") %></a>
                                                        <asp:Label runat="server" AssociatedControlID="Competency" Text='<%# Eval("Summary") %>' />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>

                                            <insite:Button ID="SelectAllButton2" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                                            <insite:Button ID="UnselectAllButton2" runat="server" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                                            <insite:Button ID="AddCompetencyButton" runat="server" Icon="fas fa-plus-circle" Text="Add" ButtonStyle="Success" DisableAfterClick="true" />
                                        </div>
                                    </insite:NavItem>

                                    <insite:NavItem runat="server" ID="AddMultipleQualificationsTab" Title="Add Multiple Competencies">
                                        <p>Enter the list of competency numbers you want to add to this achievement then click the Add button.</p>

                                        <div class="mb-3 w-75">
                                            <insite:TextBox runat="server" ID="MultipleCompetencyNumbers" TextMode="MultiLine" Height="100" />
                                        </div>

                                        <insite:Button ID="AddMultipleButton" runat="server" Icon="fas fa-plus-circle" Text="Add" DisableAfterClick="true" />
                                    </insite:NavItem>

                                </insite:Nav>
                            </ContentTemplate>
                        </insite:UpdatePanel>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="DepartmentSection" Title="Departments" Icon="far fa-building" IconPosition="BeforeText">
            <section>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="DepartmentUpdatePanel" />

                <insite:UpdatePanel runat="server" ID="DepartmentUpdatePanel">
                    <ContentTemplate>

                        <h2 class="h4 mt-4 mb-3">Departments
                        </h2>
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <uc:DepartmentChecklist runat="server" ID="DepartmentChecklist" />
                            </div>
                        </div>

                    </ContentTemplate>
                </insite:UpdatePanel>
                
            </section>
        </insite:NavItem>

    </insite:Nav>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="ContactInfo" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
