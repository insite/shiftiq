<%@ Page Language="C#" CodeBehind="AssignManagers.aspx.cs" Inherits="InSite.Cmds.Actions.BulkTool.Assign.AssignManagers" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section runat="server" ID="ManagerSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-users me-1"></i>
            People
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                
                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                <insite:UpdatePanel runat="server" ID="UpdatePanel">
                    <ContentTemplate>

                        <div class="row">
                            <div class="col-lg-6">

                                <h3>Manager</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Department
                                    </label>
                                    <cmds:FindDepartment ID="Department" runat="server" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Membership
                                    </label>
                                    <div>
                                        <asp:CheckBoxList ID="RoleTypeSelector" runat="server">
                                            <asp:ListItem Value="Organization" Text="Organization Employment" />
                                            <asp:ListItem Value="Department" Text="Department Employment" Selected="True" />
                                            <asp:ListItem Value="Administration" Text="Data Access" />
                                        </asp:CheckBoxList>
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Category
                                    </label>
                                    <div>
                                        <asp:RadioButtonList ID="SubType" runat="server">
                                            <asp:ListItem Value="Manager" Selected="True" />
                                            <asp:ListItem Value="Supervisor" />
                                            <asp:ListItem Value="Validator" />
                                        </asp:RadioButtonList>
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Person
                                    </label>
                                    <cmds:FindPerson ID="ManagerSelector" runat="server" />
                                </div>

                            </div>
                            <div runat="server" id="EmployeesColumn" class="col-lg-6">

                                <h3>People</h3>

                                <div id="NoEmployees" runat="server"><strong>No People</strong></div>

                                <asp:PlaceHolder ID="SelectEmployeesPanel" runat="server">

                                    <asp:Repeater ID="Employees" runat="server">
                                        <HeaderTemplate>
                                            <table class="table table-hover mb-3">
                                                <tr>
                                                    <th class="text-center" style="width:100px;">Assigned</th>
                                                    <th>Person</th>
                                                </tr>
                                        </HeaderTemplate>
                                        <FooterTemplate>
                                            </table>
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="text-center">
                                                    <asp:Literal ID="UserIdentifier" runat="server" Text='<%# Eval("UserIdentifier") %>' Visible="false" />
                                                    <asp:CheckBox ID="Assigned" runat="server" Checked='<%# Eval("IsAssigned") %>' />
                                                </td>
                                                <td>
                                                    <a href='/ui/cmds/admin/users/edit?userID=<%# Eval("UserIdentifier") %>'>
                                                        <%# Eval("FullName") %>
                                                    </a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                
                                    <div>
                                        <cmds:CmdsButton ID="SelectAllButton" runat="server" Text="<i class='far fa-check-square me-1'></i> Select All" />
                                        &nbsp;
                                        <cmds:CmdsButton ID="UnselectAllButton" runat="server" Text="<i class='far fa-square me-1'></i> Deselect All" />
                                    </div>
            
                                </asp:PlaceHolder>

                            </div>
                        </div>

                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ConfirmText="Are you sure you want to assign/unassign the selected people for the selected manager?" />
        <insite:CancelButton runat="server" NavigateUrl="/ui/admin/tools" />
    </div>
</asp:Content>
