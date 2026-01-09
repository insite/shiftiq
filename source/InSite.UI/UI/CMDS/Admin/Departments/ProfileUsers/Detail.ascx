<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.DepartmentProfileUsers.Controls.Detail" %>

<div id="desktop">
    <insite:ValidationSummary runat="server" ValidationGroup="Profile" />

    <asp:CustomValidator runat="server" ID="DuplicateValidator" ValidationGroup="Profile" Display="None" ErrorMessage="This profile already was acquired by this user" />

    <section runat="server" ID="DetailSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Details
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Organization
                                <insite:RequiredValidator runat="server" ControlToValidate="CompanySelector" FieldName="Company" ValidationGroup="Profile" />
                            </label>
                            <div>
                                <cmds:FindCompany runat="server" ID="CompanySelector" Width="100%" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Department
                                <insite:RequiredValidator runat="server" ControlToValidate="DepartmentIdentifier" FieldName="Department" ValidationGroup="Profile" />
                            </label>
                            <div>
                                <cmds:FindDepartment runat="server" ID="DepartmentIdentifier" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Profile
                                <insite:RequiredValidator runat="server" ControlToValidate="ProfileStandardIdentifier" FieldName="Profile" ValidationGroup="Profile" />
                            </label>
                            <div>
                                <cmds:FindProfile ID="ProfileStandardIdentifier" runat="server" Width="100%" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                    
                        <div class="form-group mb-3">
                            <label class="form-label">
                                User
                                <insite:RequiredValidator runat="server" ControlToValidate="UserIdentifier" FieldName="User" ValidationGroup="Profile" />
                            </label>
                            <div>
                                <cmds:FindPerson runat="server" ID="UserIdentifier" Width="100%" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Options
                            </label>
                            <div>
                                <asp:CheckBox runat="server" ID="IsPrimary" Text="Primary" /><br />
                                <asp:CheckBox runat="server" ID="IsRequired" Text="Required" /><br />
                                <asp:CheckBox runat="server" ID="IsRecommended" Text="Recommended" /><br />
                                <asp:CheckBox runat="server" ID="IsInProgress" Text="In Progress" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

</div>
