<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileHierarchy.ascx.cs" Inherits="InSite.Cmds.Controls.Profiles.Profiles.ProfileHierarchy" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>

        <div class="form-group mb-3">
            <label runat="server" id="VisibilityLabel" class="form-label">Visibility</label>
            <cmds:ProfileVisibilitySelector ID="Visibility" runat="server" AllowBlank="false" />
        </div>

        <div runat="server" id="CompanyField" class="form-group mb-3">
            <label class="form-label">
                Organization
                <insite:RequiredValidator ID="CompanyRequired" runat="server" ControlToValidate="Company" FieldName="Company" ValidationGroup="Profile" />
            </label>
            <cmds:FindCompany ID="Company" runat="server" CausesValidation="false" />
        </div>

        <div runat="server" id="ParentField" class="form-group mb-3">
            <label class="form-label">
                Parent Profile
            </label>
            <cmds:FindProfile ID="ParentProfile" runat="server" />
        </div>

    </ContentTemplate>
</insite:UpdatePanel>