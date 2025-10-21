<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementDetailsV2.ascx.cs" Inherits="InSite.UI.Admin.Records.Achievements.Controls.AchievementDetailsV2" %>

<div class="form-group mb-3">
    <label class="form-label">Title</label>
    <div>
        <a runat="server" id="AchievementLink">
            <asp:Literal runat="server" ID="Title" />
        </a>
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Label</label>
    <div>
        <asp:Literal runat="server" ID="Label" />
    </div>
</div>

<div runat="server" id="ExpirationField" class="form-group mb-3">
    <label class="form-label">Expiration</label>
    <div>
        <asp:Literal runat="server" ID="Expiration" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Status</label>
    <div>
        <asp:Literal runat="server" ID="Status" />
    </div>
</div>
