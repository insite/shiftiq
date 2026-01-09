<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Details.ascx.cs" Inherits="InSite.Admin.Identities.Departments.Controls.Details" %>

<div class="card border-0 shadow-lg">
    <div class="card-body">

        <h4 class="card-title mb-3">
            <i class="far fa-building me-1"></i>
            Department
        </h4>

        <div class="row">
            <div class="col-md-6">
                <div class="form-group mb-3">
                    <label class="form-label">
                        Organization
                        <insite:RequiredValidator runat="server" ControlToValidate="OrganizationIdentifier" FieldName="Organization" ValidationGroup="Department" />
                    </label>
                    <insite:FindOrganization runat="server" ID="OrganizationIdentifier" Enabled="false" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Name
                        <insite:RequiredValidator runat="server" ControlToValidate="DepartmentName" FieldName="Department Name" ValidationGroup="Department" />
                    </label>
                    <insite:TextBox runat="server" ID="DepartmentName" Width="100%" MaxLength="90" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Code
                    </label>
                    <insite:TextBox runat="server" ID="DepartmentCode" Width="100%" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Description
                    </label>
                    <insite:TextBox runat="server" ID="DepartmentDescription" Width="100%" TextMode="MultiLine" Rows="5" />
                </div>
            </div>
            <div class="col-md-6">
                <div class="form-group mb-3">
                    <label class="form-label">
                        Division
                    </label>
                    <insite:DivisionComboBox runat="server" ID="DivisionIdentifier" />
                </div>
            </div>
        </div>

    </div>
</div>
    