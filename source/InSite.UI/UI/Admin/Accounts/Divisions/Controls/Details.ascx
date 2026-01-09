<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Details.ascx.cs" Inherits="InSite.Admin.Identities.Divisions.Controls.Details" %>

<div class="card border-0 shadow-lg">
    <div class="card-body">

        <h4 class="card-title mb-3">
            <i class="far fa-industry me-1"></i>
            Division
        </h4>

        <div class="row">
            <div class="col-lg-12">
                <div class="form-group mb-3">
                    <label class="form-label">
                        Organization
                        <insite:RequiredValidator runat="server" ControlToValidate="OrganizationIdentifier" FieldName="Organization" ValidationGroup="Division" />
                    </label>
                    <insite:FindOrganization runat="server" ID="OrganizationIdentifier" Enabled="false" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Name
                        <insite:RequiredValidator runat="server" ControlToValidate="DivisionName" FieldName="Division Name" ValidationGroup="Division" />
                    </label>
                    <insite:TextBox runat="server" ID="DivisionName" Width="100%" MaxLength="90" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Code
                    </label>
                    <insite:TextBox runat="server" ID="DivisionCode" Width="100%" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Description
                    </label>
                    <insite:TextBox runat="server" ID="DivisionDescription" Width="100%" TextMode="MultiLine" Rows="5" />
                </div>
            </div>
        </div>

    </div>
</div>
