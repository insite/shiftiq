<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SectionTiles.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.ContentEditor.SectionTiles" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="row">
            <asp:PlaceHolder runat="server" ID="Container" />
        </div>
    </ContentTemplate>
</insite:UpdatePanel>
