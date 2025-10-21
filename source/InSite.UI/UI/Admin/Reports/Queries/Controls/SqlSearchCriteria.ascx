<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SqlSearchCriteria.ascx.cs" Inherits="InSite.Admin.Reports.Queries.Controls.SqlSearchCriteria" %>

<div class="row">
    <div class="col-md-12">
        <div runat="server" id="ErrorPanel" class="alert alert-danger" visible="false"></div>
        <insite:TextBox runat="server" ID="Query" TextMode="MultiLine" Width="100%" CssClass="sql-editor" AllowHtml="true" />
    </div>
</div>

<div class="mt-3">
    <insite:Button runat="server" ID="SearchButton" Text="Execute" Icon="far fa-bolt" />
    <insite:ClearButton runat="server" ID="ClearButton" />
</div>
