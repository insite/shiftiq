<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SectionDefault.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.ContentEditor.SectionDefault" %>

<%@ Register Src="Field.ascx" TagName="ContentEditorField" TagPrefix="uc" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <uc:ContentEditorField runat="server" ID="EditorField" />
    </ContentTemplate>
</insite:UpdatePanel>
