<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CollectionInfo.ascx.cs" Inherits="InSite.UI.Admin.Assets.Collections.Controls.CollectionInfo" %>

<dl class="row">
    <dt class="col-sm-3">Name:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="CollectionName" /></dd>

    <dt class="col-sm-3">Toolkit:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="ToolkitName" /></dd>

    <dt class="col-sm-3">Package:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="Package" /></dd>

    <dt class="col-sm-3">Process:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="Process" /></dd>

    <dt class="col-sm-3">Type:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="Type" /></dd>

    <dt class="col-sm-3">References:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="References" /></dd>
</dl>
