<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementDetails.ascx.cs" Inherits="InSite.Admin.Achievements.Achievements.Controls.AchievementDetails" %>

<dl class="row">
    <dt class="col-sm-3">Title:</dt>
    <dd class="col-sm-9">
        <a runat="server" id="AchievementLink">
            <asp:Literal runat="server" ID="Title" />
        </a>
    </dd>

    <dt class="col-sm-3">Label:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="Label" /></dd>

    <dt class="col-sm-3">Expiration:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="Expiration" /></dd>

    <dt class="col-sm-3">Status:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="Status" /></dd>
</dl>
