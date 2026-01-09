<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignProfileToSelectedEmployees.ascx.cs" Inherits="InSite.Cmds.Controls.BulkTool.Assign.AssignProfileToSelectedEmployees" %>

<div class="row">
    <div class="col-lg-6 mb-3 mb-lg-0">

        <div class="form-group mb-3">
            <label class="form-label">
                Department
            </label>
            <cmds:FindDepartment ID="DepartmentIdentifier" runat="server" />
        </div>

        <div runat="server" ID="RowProfile" class="form-group mb-3">
            <label class="form-label">
                Profile
            </label>
            <cmds:FindProfile ID="ProfileIdentifier" runat="server" />
        </div>

        <div runat="server" ID="RowProfileType" class="form-group mb-3">
            <label class="form-label">
                Profile Type
            </label>
            <insite:ComboBox ID="ProfileType" runat="server">
                <Items>
                    <insite:ComboBoxOption Text="Primary" Value="Primary" />
                    <insite:ComboBoxOption Text="Secondary" Value="Secondary" Selected="true" />
                </Items>
            </insite:ComboBox>
        </div>

        <div runat="server" ID="ComplianceRequiredField" class="form-group mb-3">
            <label class="form-label">
                Compliance Required
            </label>
            <div>
                <asp:CheckBox runat="server" ID="ComplianceRequired" />
            </div>
        </div>

    </div>
    <div class="col-lg-6" runat="server" id="EmployeesColumn" visible="false">

        <h3>Department Employees</h3>
        
        <asp:Label runat="server" ID="ErrorMessage"></asp:Label>

        <asp:Repeater ID="DepartmentEmployees" runat="server">
            <HeaderTemplate>
                <table><tbody>
            </HeaderTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td class="p-1">
                        <asp:Literal ID="UserIdentifier" runat="server" Text='<%# Eval("UserIdentifier") %>' Visible="false" />
                        <asp:CheckBox runat="server" ID="IsSelected" Checked='<%# Eval("IsSelected") %>' />
                    </td>
                    <td class="p-1">
                        <asp:Label runat="server" AssociatedControlID="IsSelected" Text='<%# Eval("FullName") %>' style="font-weight:normal;" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

        <div class="mt-3">
            <insite:SaveButton ID="SaveButton" runat="server" />
        </div>

    </div>
</div>
