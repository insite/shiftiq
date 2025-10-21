<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignSelectedProfilesToEmployees.ascx.cs" Inherits="InSite.Cmds.Controls.BulkTool.Assign.AssignSelectedProfilesToEmployees" %>

<div class="row">
    <div class="col-lg-6">

        <div class="mb-3">
            You can assign all of the profiles in this department to all of the people in this department.
            Select the department from the list below, and then click the Save button.
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Department
                <insite:RequiredValidator runat="server" ControlToValidate="DepartmentIdentifier" FieldName="Department" ValidationGroup="Department" />
            </label>
            <cmds:FindDepartment ID="DepartmentIdentifier" runat="server" />
        </div>

    </div>
</div>

<div class="row" runat="server" id="DepartmentRow" visible="false">
    <div class="col-lg-6">

        <h3>Department Profiles</h3>
        
        <asp:Repeater ID="DepartmentProfiles" runat="server">
            <HeaderTemplate>
                <table><tbody>
            </HeaderTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td class="p-1">
                        <asp:Literal ID="ProfileStandardIdentifier" runat="server" Text='<%# Eval("ProfileStandardIdentifier") %>' Visible="false" />
                        <asp:CheckBox runat="server" ID="IsSelected" Checked='<%# Eval("IsSelected") %>' />
                    </td>
                    <td class="p-1 text-nowrap">
                        <a href='<%# "/ui/cmds/admin/standards/profiles/edit?id=" + Eval("ProfileStandardIdentifier") %>'><%# Eval("ProfileNumber") %></a>
                    </td>
                    <td class="p-1">
                        <asp:Label runat="server" AssociatedControlID="IsSelected" Text='<%# Eval("ProfileTitle") %>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

    </div>
    <div class="col-lg-6">

        <h3>People</h3>

        <asp:Repeater ID="DepartmentEmployees" runat="server">
            <HeaderTemplate>
                <table><tbody>
            </HeaderTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <a href='/ui/cmds/admin/users/edit?userID=<%# Eval("UserIdentifier") %>'>
                            <%# Eval("FullName") %>
                        </a>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

    </div>
</div>

<div class="mt-3">
    <insite:SaveButton ID="SaveButton" runat="server" ValidationGroup="Department"
        ConfirmText="Are you sure you want to assign all of the profiles in this department to all of the people in this department?" />
</div>