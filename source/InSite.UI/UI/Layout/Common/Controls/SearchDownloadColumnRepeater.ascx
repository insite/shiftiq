<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchDownloadColumnRepeater.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.SearchDownloadColumnRepeater" %>

<asp:HiddenField runat="server" ID="StateInput" />

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate><table id="<%# Repeater.ClientID %>" class="table table-condensed table-sm table-search-download-columns"></HeaderTemplate>
    <FooterTemplate></table></FooterTemplate>
    <ItemTemplate>
        <tbody>
            <tr runat="server" visible='<%# !string.IsNullOrEmpty((string)Eval("Name")) %>' class="column-group"><td colspan="2">
                <%# Eval("Name") %>
            </td></tr>
            <asp:Repeater runat="server" ID="Repeater">
                <HeaderTemplate></HeaderTemplate>
                <FooterTemplate></FooterTemplate>
                <ItemTemplate>
                    <tr class="column-item" data-index='<%# Eval("Index") %>'>
                        <td class="cmd-dwn"></td>
                        <td class="title-dwn"><%# GetTranslatedColumnTitle() %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </ItemTemplate>
</asp:Repeater>