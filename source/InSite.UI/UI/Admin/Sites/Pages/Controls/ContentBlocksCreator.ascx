<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentBlocksCreator.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.ContentBlocksCreator" %>

<div class="row">
    <div class="col-md-8">
        <insite:ValidationSummary runat="server" ID="ValidationSummary" />
    </div>
</div>

<div class="row settings">
    <div class="col-md-4">

        <div class="form-group mb-3">
            <label class="form-label">
                Block Type
                <insite:RequiredValidator runat="server" ID="ControlRequiredValidator" ControlToValidate="ControlSelector" FieldName="Control" />
            </label>
            <div>
                <insite:ComboBox runat="server" ID="ControlSelector" Width="100%" />
            </div>
        </div>

    </div>
    <div class="col-md-4">

        <div class="form-group mb-3">
            <label class="form-label">
                Block Title
            </label>
            <div>
                <insite:TextBox ID="Title" runat="server" MaxLength="128" Width="100%" />
            </div>
        </div>

    </div>
</div>

<div class="mt-3">
    <insite:Button runat="server" ID="CreateButton" Text="Add" Icon="fas fa-plus-circle" ButtonStyle="Success" />
</div>
