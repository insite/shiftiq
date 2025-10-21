<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LogbookDetails.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Controls.LogbookDetails" %>

<dl runat="server" id="List" class="row">

    <dt class="col-sm-3">Logbook Name:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="JournalSetupName5" /></dd>

    <dt class="col-sm-3">Logbook Title:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="Title5" /></dd>

    <dt class="col-sm-3">Class:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="ClassTitle5" /></dd>

    <dt class="col-sm-3"></dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="ClassScheduled5" /></dd>

    <dt class="col-sm-3">Achievement:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="AchievementTitle5" /></dd>

    <dt class="col-sm-3">Framework:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="FrameworkTitle5" /></dd>

</dl>