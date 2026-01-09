<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileTreeViewNode.ascx.cs" Inherits="InSite.UI.Admin.Assets.Files.Controls.FileTreeViewNode" %>

<ul>
    <asp:Repeater runat="server" ID="FileRepeater">
        <ItemTemplate>
            <li>
                <asp:Literal runat="server" ID="FileLink" />

                <div id="ChildrenNode" runat="server">
                    <insite:DynamicControl runat="server" ID="Container" />
                </div>
            </li>
        </ItemTemplate>
    </asp:Repeater>
</ul>