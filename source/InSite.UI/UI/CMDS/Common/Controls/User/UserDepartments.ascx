<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserDepartments.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.People.Controls.UserDepartments" %>

<style>
    div.form-check label { margin-right: 15px; }
</style>

<div class="row">
    <div class="col-lg-5 mb-3 mb-lg-0">
        <div class="card">
            <div class="card-body">

                <h3 class="mb-0">Assign department</h3>
                <div class="mt-0 mb-3 form-text">
                    This person can be assigned to multiple organizations and departments.
                    Select an organization and department, then click Assign
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Organization
                        <insite:RequiredValidator runat="server" FieldName="Organization" ControlToValidate="OrganizationIdentifier" ValidationGroup="PersonMembership" />
                    </label>
                    <cmds:FindCompany runat="server" ID="OrganizationIdentifier" />
                </div>
                
                <div class="form-group mb-3">
                    <label class="form-label">
                        Department
                    </label>
                    <cmds:FindDepartment ID="DepartmentIdentifier" runat="server" CompanyControl="OrganizationIdentifier" EmptyMessage="All Departments" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        <asp:Literal runat="server" ID="PersonName" />
                    </label>
                    <div>

                        <insite:RadioButton runat="server" ID="MembershipDepartment" GroupName="Membership" Text="is employed by this department" Checked="true" />

                        <insite:RadioButton runat="server" ID="MembershipOrganization" GroupName="Membership" Text="is employed by this organization" />

                        <insite:RadioButton runat="server" ID="MembershipAdministration" GroupName="Membership" Text="has access to this organization/department" />

                    </div>
                </div>

                <div class="mt-3">
                    <insite:Button ID="AddRole" runat="server" Icon="fas fa-plus-circle" Text="Assign" ButtonStyle="Primary" ValidationGroup="PersonMembership" OnClientClick="return confirmNewMembership();" />
                </div>

            </div>
        </div>
    </div>
    <div class="col-lg-7">

                <insite:Grid runat="server" ID="DepartmentUserGrid"  DataKeyNames="DepartmentIdentifier">
                    <Columns>
                                
                        <asp:HyperLinkField HeaderText="Organization" DataTextField="CompanyName" DataNavigateUrlFields="OrganizationIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/organizations/edit?id={0}" />
                        <asp:HyperLinkField HeaderText="Department" DataTextField="DepartmentName" DataNavigateUrlFields="DepartmentIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/departments/edit?id={0}" />
                        <asp:TemplateField HeaderText="Function/Role">
                            <ItemTemplate>
                                <%# (String)Eval("RoleType") == "Administration"
                                    ? "Data Access" 
                                    : ((String)Eval("RoleType") == "Department"
                                    ? "Department Employment" : "Organization Employment") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-Width="20px" ItemStyle-Wrap="false">
                            <ItemTemplate>
                                <cmds:IconButton runat="server"
                                    IsFontIcon="true" CssClass="trash-alt"
                                    ToolTip="Remove this person from this organization"
                                    ConfirmText="Are you sure you want to remove this person from this organization?"
                                    CommandName="Delete"
                                    Visible='<%# IsDeleteVisible(Eval("OrganizationIdentifier")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </insite:Grid>

                <div class="mt-3">
                    
                    <div class="d-flex mb-2">
                        <div class="flex-grow-1 pe-2">
                            <insite:TextBox ID="FilterText" runat="server" EmptyMessage="Organization or Department Name" />
                        </div>
                        <div class="">
                            <insite:FilterButton ID="FilterButton" runat="server" ButtonStyle="OutlinePrimary" />
                        </div>
                    </div>

                    <insite:CheckBoxList ID="RoleTypeSelector" runat="server" RepeatDirection="Horizontal" />

                </div>

    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        function confirmNewMembership() {
            var organizationSelector = document.getElementById("<%= OrganizationIdentifier.ClientID %>");
            var departmentSelector = document.getElementById("<%= DepartmentIdentifier.ClientID %>");

            return organizationSelector.getItem() == null
                || departmentSelector.getItem() != null
                || confirm("Are you sure you want to assign this person to every department in the organization?")
                ;
        }
    </script>
</insite:PageFooterContent>
