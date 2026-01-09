<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SectionTabs.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.ContentEditor.SectionTabs" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="card">
            <div class="card-body">
                <insite:Nav runat="server" ID="TabsNav">

                </insite:Nav>
            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>
