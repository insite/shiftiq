<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuerySearchResults.ascx.cs" Inherits="InSite.Admin.Reports.Queries.Controls.QuerySearchResults" %>

<asp:Label ID="Instructions" runat="server" CssClass="help" Visible="false" />

<asp:GridView runat="server" ID="TempGrid" CssClass="table table-striped table-bordered" />

<div class="d-none">
    <insite:Grid runat="server" ID="Grid" EnablePaging="false" EnableSorting="false" Height="">
       <%-- <clientsettings scrolling-allowscroll="true" scrolling-usestaticheaders="true" scrolling-scrollheight="800px">
        </clientsettings>--%>
    </insite:Grid>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .table-header-int32, .table-value-int32 {
            text-align: right;
        }
    </style>
</insite:PageHeadContent>
