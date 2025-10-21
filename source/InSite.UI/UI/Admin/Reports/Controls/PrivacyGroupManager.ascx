<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrivacyGroupManager.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.PrivacyGroupManager" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="mb-3">
            <insite:FindGroup runat="server" ID="FindGroup" Output="List" MaxSelectionCount="0" />
        </div>
    </ContentTemplate>
</insite:UpdatePanel>
