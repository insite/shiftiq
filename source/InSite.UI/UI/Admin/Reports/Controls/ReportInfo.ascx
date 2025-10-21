<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportInfo.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.ReportInfo" %>

<dl class="row">
    <dt class="col-sm-3">Report Title:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="ReportTitle" /></dd>

    <dt class="col-sm-3">Report Description:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="ReportDescription" /></dd>
</dl>