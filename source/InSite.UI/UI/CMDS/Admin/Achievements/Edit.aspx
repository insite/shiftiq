<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Cmds.Admin.Achievements.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementDetails.ascx" TagName="AchievementDetails" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Contacts/Groups/Controls/DepartmentChecklist.ascx" TagName="DepartmentChecklist" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Records/Achievements/Controls/CredentialGrid.ascx" TagName="CredentialGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Achievement" />

    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

    <insite:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>
            <insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

                <insite:NavItem runat="server" ID="AchievementSection" Title="Achievement Settings" Icon="far fa-trophy" IconPosition="BeforeText">
                    <section>

                        <h2 class="h4 mt-4 mb-3">
                            Achievement
                        </h2>

                        <uc:AchievementDetails ID="AchievementDetails" runat="server" />

                    </section>
                </insite:NavItem>
                <insite:NavItem runat="server" ID="CompetencySection" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
                    <section>
                        <h2 class="h4 mt-4 mb-3">
                            Competencies
                        </h2>

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <insite:Nav runat="server">

                                    <insite:NavItem runat="server" ID="CompetencyTab" Title="Competencies">
                                        <div runat="server" id="CompetencyPanel" class="mb-3">
                                            <asp:Repeater ID="Competencies" runat="server">
                                                <ItemTemplate>
                                                    <div class="row">
                                                        <div class="col-md-12">
                                                            <asp:Literal ID="CompetencyStandardIdentifier" runat="server" Text='<%# Eval("CompetencyStandardIdentifier") %>' Visible="false" />
                                                            <asp:CheckBox runat="server" ID="Competency" />
                                                            <a href='<%# "/ui/cmds/admin/standards/competencies/edit?id=" + Eval("CompetencyStandardIdentifier") %>'><%# Eval("Number") %></a>
                                                            <asp:Label runat="server" AssociatedControlID="Competency" Text='<%# Eval("Summary") %>' />
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>

                                        <div runat="server" id="CompetencyButtons" class="row">
                                            <div class="col-md-12">
                                                <insite:Button ID="SelectAllButton" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="Default" />
                                                <insite:Button ID="UnselectAllButton" runat="server" Icon="far fa-square" Text="Deselect All" ButtonStyle="Default" />
                                                <insite:DeleteButton ID="DeleteCompetencyButton" runat="server" ButtonStyle="Default" />
                                            </div>
                                        </div>
                                    </insite:NavItem>

                                    <insite:NavItem runat="server" ID="NewCompetencyTab" Title="Add Competencies">
                                        <p>Search for the individual competencies you want to add to this achievement. Check the box next to each one and click the Add button.</p>

                                        <div class="mb-3">
                                            <cmds:FindAchievement2 ID="SearchAchievement" runat="server" EmptyMessage="Select a achievement" CssClass="w-25" />
                                        </div>
                                        <div class="mb-3">
                                            <insite:TextBox ID="SearchText" runat="server" EmptyMessage="Competency Number or Summary" CssClass="w-25" />
                                        </div>
                                        <div class="mb-3">
                                            <insite:FilterButton ID="FilterButton" runat="server" ButtonStyle="Default" />
                                            <insite:ClearButton ID="ClearButton" runat="server" ButtonStyle="Default" />
                                        </div>

                                        <p id="FoundCompetency" runat="server" visible="false"></p>

                                        <div id="CompetencyList" runat="server" visible="false">
                                            <div class="mb-3">
                                                <asp:Repeater ID="NewCompetencies" runat="server">
                                                    <ItemTemplate>
                                                        <div>
                                                            <asp:Literal ID="CompetencyStandardIdentifier" runat="server" Text='<%# Eval("CompetencyStandardIdentifier") %>' Visible="false" />
                                                            <asp:CheckBox ID="Competency" runat="server" />
                                                            <a href='<%# "/ui/cmds/admin/standards/competencies/edit?id=" + Eval("CompetencyStandardIdentifier") %>'><%# Eval("Number") %></a>
                                                            <asp:Label runat="server" AssociatedControlID="Competency" Text='<%# Eval("Summary") %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>

                                            <insite:Button ID="SelectAllButton2" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="Default" />
                                            <insite:Button ID="UnselectAllButton2" runat="server" Icon="far fa-square" Text="Deselect All" ButtonStyle="Default" />
                                            <insite:Button ID="AddCompetencyButton" runat="server" Icon="fas fa-plus-circle" Text="Add" ButtonStyle="Default" />
                                        </div>
                                    </insite:NavItem>

                                    <insite:NavItem runat="server" ID="AddMultipleQualificationsTab" Title="Add Multiple Competencies">
                                        <p>Enter the list of competency numbers you want to add to this achievement then click the Add button.</p>

                                        <div class="mb-3">
                                            <insite:TextBox runat="server" ID="MultipleCompetencyNumbers" TextMode="MultiLine" Height="100" CssClass="w-75" />
                                        </div>

                                        <insite:Button ID="AddMultipleButton" runat="server" Icon="fas fa-plus-circle" Text="Add" ButtonStyle="Default" />
                                    </insite:NavItem>

                                </insite:Nav>
                            </div>
                        </div>
                    </section>
                </insite:NavItem>
                <insite:NavItem runat="server" ID="DepartmentSection" Title="Departments" Icon="far fa-building" IconPosition="BeforeText">
                    <section>
                        <h2 class="h4 mt-4 mb-3">
                            Departments
                        </h2>
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <uc:DepartmentChecklist runat="server" ID="DepartmentChecklist" />
                            </div>
                        </div>
                    </section>
                </insite:NavItem>
                <insite:NavItem runat="server" ID="CredentialSection" Title="Learners" Icon="far fa-users" IconPosition="BeforeText">
                    <section>
                        <h2 class="h4 mt-4 mb-3">
                            Learners
                        </h2>

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <uc:CredentialGrid runat="server" ID="CredentialGrid" />

                                <div class="text-muted fs-sm">
                                    <i class="far fa-lightbulb-on me-1"></i>
                                    A specific achievement assigned to a specific learner is also known as a credential.
                                </div>

                            </div>
                        </div>
                    </section>
                </insite:NavItem>

            </insite:Nav>

        </ContentTemplate>
    </insite:UpdatePanel>

    <div class="mt-4">

    <!-- According to Google's UI Design guidelines, buttons for primary actions should be placed at the bottom of the 
         form, such that primary actions go first (leftmost) in the natural reading flow from left to right. Dismissive 
         actions (Cancel) appear next to the primary action. Destructive actions (Delete) are placed last or visually 
         separated. -->

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Achievement" DisableAfterClick="true" />

    <insite:CancelButton runat="server" ID="CancelButton" />

    <insite:Button runat="server" ID="DuplicateButton" ButtonStyle="Default" Text="Duplicate" Icon="far fa-copy" ConfirmText="Are you sure to copy this Achievement?" DisableAfterClick="true" />

    <insite:Button runat="server" ID="ViewReferencesButton" ButtonStyle="Default" Text="View References" Icon="far fa-network-wired" />

    <div class="float-end">
        <insite:DeleteButton runat="server" ID="DeleteButton" ConfirmText="Are you sure you want to delete this achievement?" DisableAfterClick="true" />
        <asp:Button ID="ConfirmDeleteButton" runat="server" style="display:none;" />
    </div>

    </div>

</asp:Content>
