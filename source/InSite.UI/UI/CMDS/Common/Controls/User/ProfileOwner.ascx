<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileOwner.ascx.cs" Inherits="InSite.Cmds.Controls.Profiles.Profiles.ProfileOwner" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>

        <div class="form-group mb-3">
            <label class="form-label">Owner</label>
            <cmds:OrganizationScopeSelector ID="OrganizationScope" runat="server" AllowBlank="false" />
        </div>

        <div runat="server" id="ParentField" class="form-group mb-3">
            <label class="form-label">
                Parent Profile
            </label>
            <cmds:FindProfile ID="ParentProfile" runat="server" />
        </div>

    </ContentTemplate>
</insite:UpdatePanel>
