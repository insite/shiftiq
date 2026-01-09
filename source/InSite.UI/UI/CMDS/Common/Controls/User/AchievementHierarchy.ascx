<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementHierarchy.ascx.cs" Inherits="InSite.Cmds.Controls.Training.Achievements.AchievementHierarchy" %>

<style type="text/css">
    .categories {
        display: block;
        max-height: 300px;
        overflow-y: auto;
    }
</style>

<div runat="server" ID="RowCompany" class="form-group mb-3">
    <label class="form-label">
        Organization
    </label>
    <div>
        <asp:Literal runat="server" ID="CompanyName" />
    </div>
</div>
<div runat="server" ID="RowCategory" class="form-group mb-3">
    <label class="form-label">
        Organization-Specific Categories
    </label>
    <div class="categories">
        <asp:Literal runat="server" ID="NoneLiteral" Text="None" />
        <asp:CheckBoxList runat="server" ID="CompanyCategories" />
    </div>
</div>