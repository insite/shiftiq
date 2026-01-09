<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserOrganizationList.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.UserOrganizationList" %>

<div class="mb-3">
    <insite:FindOrganization runat="server" ID="OrganizationCombo" EmptyMessage="Add Organization" Width="300px" ShowFooter="false" />
</div>

<table id="<%# ClientID %>" class="table">
    <thead>
        <tr>
            <th></th>
            <th class="text-center" title="Access Granted"><asp:Label runat="server" ID="AccessCount" /><br />Access Granted</th>
            <th class="text-center" title="Learner"><asp:Label runat="server" ID="LearnerCount" /><br />Learner</th>
            <th class="text-center" title="Administrator"><asp:Label runat="server" ID="AdministratorCount" /><br />Administrator</th>
            <th class="text-center" title="Developer"><asp:Label runat="server" ID="DeveloperCount" /><br />Developer</th>
            <th class="text-center" title="Operator"><asp:Label runat="server" ID="OperatorCount" /><br />Operator</th>
            <th>Organization</th>
            <th>Code</th>
        </tr>
    </thead>
    <tbody>

    <asp:Repeater runat="server" ID="Repeater">
        <ItemTemplate>
            <tr>
                <td style="width:20px;">
                    <insite:IconButton runat="server" Color="Danger"
                        CommandName="Delete" CommandArgument='<%# Eval("OrganizationIdentifier") %>' Name="trash-alt" ToolTip="Delete" 
                        ConfirmText="Are you sure you want to delete this organization?" />
                </td>
                <td class="text-center">
                    <asp:CheckBox runat="server" ID="PersonIsGrantedAccess" AutoPostBack="true" Checked='<%# (bool)Eval("PersonIsGrantedAccess") %>' />
                </td>
                <td class="text-center">
                    <asp:CheckBox runat="server" ID="PersonIsLearner" AutoPostBack="true" Checked='<%# (bool)Eval("PersonIsLearner") %>' />
                    <asp:Literal runat="server" ID="OrganizationIdentifier" Text='<%# Eval("OrganizationIdentifier") %>' Visible="false" />
                </td>
                <td class="text-center">
                    <asp:CheckBox runat="server" ID="PersonIsAdministrator" AutoPostBack="true" Checked='<%# (bool)Eval("PersonIsAdministrator") %>' />
                </td>
                <td class="text-center">
                    <asp:CheckBox runat="server" ID="PersonIsDeveloper" AutoPostBack="true" Checked='<%# (bool)Eval("PersonIsDeveloper") %>' />
                </td>
                <td class="text-center">
                    <asp:CheckBox runat="server" ID="PersonIsOperator" AutoPostBack="true" Checked='<%# (bool)Eval("PersonIsOperator") %>' />
                </td>
                <td>
                    <a href="/ui/admin/accounts/organizations/edit?<%# Eval("OrganizationIdentifier", "organization={0}") %>"><%# Eval("OrganizationName") %></a>
                </td>
                <td>
                    <%# Eval("OrganizationCode") %>
                </td>
                
            </tr>
        </ItemTemplate>
    </asp:Repeater>

</tbody></table>

<div class="form-text">
    <strong>Learners</strong> have access to the learning portal and appear in organizational reports as students, employees, contractors, or members.
    <strong>Administrators</strong> have access to administrative tools and settings within the organization's account.
    <strong>Developers</strong> have access to the API for building integrations and applications.
    <strong>Operators</strong> have complete platform access to provide business and technical support to the organization.
</div>

<asp:HiddenField runat="server" ID="OrganizationCount" />
