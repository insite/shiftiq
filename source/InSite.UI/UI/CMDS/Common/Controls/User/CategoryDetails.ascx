<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryDetails.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Categories.CategoryDetails" %>

<div class="row">
    <div class="col-md-6">
        <div class="settings">
            <div class="form-group mb-3">
                <label class="form-label">
                    Category Name
                    <insite:RequiredValidator runat="server" FieldName="CategoryName" ControlToValidate="CategoryName" ValidationGroup="CategoryInfo" />
                </label>
                <div>
                    <insite:TextBox ID="CategoryName" runat="server" MaxLength="120" />
                </div>
            </div>
            <div class="form-group mb-3">
                <label class="form-label">
                    Achievement Type
                    <insite:RequiredValidator runat="server" FieldName="ResourceType" ControlToValidate="AchievementType" ValidationGroup="CategoryInfo" />
                </label>
                <div>
                    <cmds:AchievementTypeSelector runat="server" ID="AchievementType" Width="350px" NullText="" />
                </div>
            </div>
        </div>
    </div>
</div>