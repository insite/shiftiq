<%@ Page Language="C#" CodeBehind="AssignDepartment.aspx.cs" Inherits="InSite.Cmds.Actions.BulkTool.Assign.AssignDepartment" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <asp:ValidationSummary runat="server" ValidationGroup="AssignAchievements" />

    <asp:CustomValidator runat="server" ID="AchievementsValidator" ErrorMessage="There are no selected achievements" ValidationGroup="AssignAchievements" Display="None" />
    <asp:CustomValidator runat="server" ID="DepartmentsValidator" ErrorMessage="There are no selected departments" ValidationGroup="AssignAchievements" Display="None" />

    <section class="mb-3">

            <div class="row">
                <div class="col-md-6">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Achievements</h3>

                            <div class="mb-2">
                                <cmds:AchievementTypeSelector ID="SubType" runat="server" EmptyMessage="Achievement Type" NullText="" />
                            </div>
                            <div class="mb-2">
                                <insite:TextBox ID="AchievementTitle" runat="server" EmptyMessage="Achievement Title" />
                            </div>
                            <div class="mb-4">
                                <insite:FilterButton ID="FilterButton" runat="server" />
                                <insite:Button ID="SelectAllButton" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                                <insite:Button ID="UnselectAllButton" runat="server" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                            </div>

                            <asp:Panel ID="AchievementTypesPanel" runat="server" CssClass="mt-3">

                                <asp:Repeater ID="AchievementTypes" runat="server">
                                    <ItemTemplate>
                                        <div class="mt-3 mb-2 fw-bold">
                                            <%# Eval("Name") %>
                                        </div>

                                        <asp:Repeater ID="Achievements" runat="server">
                                            <ItemTemplate>
                                                <div class="mb-2">
                                                    <insite:CheckBox runat="server" ID="Selected" Value='<%# Eval("ID") %>' Text='<%# Eval("Title") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </asp:Panel>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Departments</h3>

                            <asp:Repeater ID="Departments" runat="server">
                                <ItemTemplate>
                                    <div class="mb-2">
                                        <insite:CheckBox runat="server" ID="Selected" Value='<%# Eval("DepartmentIdentifier") %>' Text='<%# Eval("DepartmentName") %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                        </div>
                    </div>

                </div>
            </div>

    </section>

    <div class="mt-4 mb-2">
        Assign the selected achievements to the selected departments.
    </div>

    <insite:SaveButton runat="server" ID="SaveButton"
        Text="Assign"
        ConfirmText="Are you sure you want to assign all of the selected achievements to the selected departments?" />

</asp:Content>
