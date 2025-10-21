<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignCompanyProfilesToEmployee.ascx.cs" Inherits="InSite.Cmds.Controls.BulkTool.Assign.AssignCompanyProfilesToEmployee" %>

<asp:CustomValidator ID="PrimaryProfileValidator" runat="server" ErrorMessage="Only one primary profile per organization could be specified." Display="None" ValidationGroup="AssignCompanyProfilesToEmployee" Font-Bold="True" ForeColor="Red" />

<div class="row">
    <div class="col-lg-6">

        <div class="mb-3">
            You can assign a profile to a selected person using this form.
            Select the person from the list below, select the profile(s)
            you want the person to acquire, and then click the Save button.
            (Then you can clear the field to select a different person.)
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Person
            </label>
            <cmds:FindPerson ID="PersonIdentifier" runat="server" />
        </div>

        <div runat="server" id="DepartmentField" class="form-group mb-3">
            <label class="form-label">
                Department
            </label>
            <cmds:FindDepartment ID="DepartmentIdentifier" runat="server" />
        </div>

    </div>
</div>
            
<asp:Repeater ID="ProfileRepeater" runat="server">
    <HeaderTemplate>
        <hr/>

        <table class="table table-striped">
            <thead>
                <tr>
                    <th class="text-nowrap" style="width:20px;">Compliance Required</th>
                    <th style="width:170px;">Profile Type</th>
                    <th>Profile</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td class="text-center">
                <asp:CheckBox runat="server" ID="ComplianceRequired" />
            </td>
            <td>
                <insite:ComboBox ID="ProfileType" runat="server">
                    <Items>
                        <insite:ComboBoxOption />
                        <insite:ComboBoxOption Value="Primary" Text="Primary" />
                        <insite:ComboBoxOption Value="Secondary" Text="Secondary" />
                    </Items>
                </insite:ComboBox>
                <asp:Label ID="ProfileStandardIdentifier" runat="server" Text='<%# Eval("ProfileStandardIdentifier") %>' Visible="false" />
            </td>
            <td>
                <%# Eval("ProfileTitle") %>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

<div class="mt-3">
    <insite:SaveButton ID="SaveButton" runat="server" ValidationGroup="AssignCompanyProfilesToEmployee" />
</div>
