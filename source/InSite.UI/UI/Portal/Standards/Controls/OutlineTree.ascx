<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutlineTree.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.OutlineTree" %>

<%@ Register Src="OutlineNode.ascx" TagName="OutlineNode" TagPrefix="uc" %>

<div id='<%= ClientID %>' style="visibility: hidden;">
    <uc:OutlineNode runat="server" ID="OutlineNode" />
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (() => {
            const treeObj = window.outlineTree.register('<%= ClientID %>', '<%= StandardIdentifier %>', <%= AllowSaveState.ToString().ToLower() %>);
            if (treeObj)
                window.outlineTree.<%= ClientID %> = treeObj;
        })();
    </script>
</insite:PageFooterContent>