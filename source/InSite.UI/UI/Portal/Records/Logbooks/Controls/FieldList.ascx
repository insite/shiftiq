<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldList.ascx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Controls.FieldList" %>

<asp:Repeater runat="server" ID="FieldRepeater">
    <ItemTemplate>
        <insite:DynamicControl runat="server" ID="Field" />
    </ItemTemplate>
</asp:Repeater>