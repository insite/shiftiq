<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganizationTags.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.OrganizationTags" %>

<asp:Repeater runat="server" ID="TagCollectionRepeater">
    <ItemTemplate>
        <div class="fw-bold">
            <asp:Literal runat="server" ID="Name" Text='<%# Eval("Name") %>' />
        </div>
        <asp:Repeater runat="server" ID="TagRepeater">
            <ItemTemplate>
                <div class="ps-3">
                    <asp:CheckBox runat="server" ID="Selected" Text='<%# GetTagText() %>' Checked='<%# Eval("Selected") %>' />
                    <asp:Literal runat="server" ID="TagText" Text='<%# Eval("Text") %>' Visible="false" />
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </ItemTemplate>
</asp:Repeater>