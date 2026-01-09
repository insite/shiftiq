<%@ Page Language="C#" CodeBehind="ViewHistory.aspx.cs" Inherits="InSite.Admin.Reports.Changes.Forms.ViewHistory" MasterPageFile="~/UI/Layout/Admin/AdminModal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:PageHeadContent runat="server">
        <base target="_parent" />
        <style type="text/css">
            h2 { color: #337ab7; font-size: 22px; }    
        </style>

    </insite:PageHeadContent>

    <asp:Repeater runat="server" ID="HistoryRepeater">
        <ItemTemplate>
            <h2><%# Eval("When") %> - <%# Eval("Who") %></h2>
            <%# Eval("What") %>
            <hr class="mb-3" />
        </ItemTemplate>
    </asp:Repeater>

    <div class="mt-3">
        <insite:CloseButton runat="server" ID="CloseButton" OnClientClick="modalManager.closeModal(); return false;" />
    </div>

    <div class="clearfix"></div>
</asp:Content>
