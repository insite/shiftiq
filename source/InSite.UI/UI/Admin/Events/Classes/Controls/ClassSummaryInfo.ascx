<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassSummaryInfo.ascx.cs" Inherits="InSite.UI.Admin.Events.Classes.Controls.ClassSummaryInfo" %>

<div runat="server" id="ClassHeadingDiv" class="form-group mb-3">
    <label class="form-label">
        Achievement
    </label>
    <div>
        <asp:Literal runat="server" ID="AchievementTitle" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Event Title
    </label>
    <div>
        <a runat="server" id="EventLink">
            <asp:Literal runat="server" ID="EventTitle" />
        </a>
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Workflow Status
    </label>
    <div>
        <span runat="server" id="IsPublished" class="badge bg-success float-end">Published</span>
        <asp:Literal runat="server" ID="EventSchedulingStatus" />
    </div>
</div>
