<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryDetail.ascx.cs" Inherits="InSite.UI.Admin.Learning.Categories.Controls.CategoryDetail" %>

<div class="form-group mb-3">
    <label class="form-label">
        Category Folder
        <insite:RequiredValidator runat="server" ControlToValidate="CategoryFolderSelector" FieldName="Category Folder" ValidationGroup="Category" />
        <insite:RequiredValidator runat="server" ControlToValidate="CategoryFolderText" FieldName="Category Folder" ValidationGroup="Category" />
    </label>
    <insite:MultiField runat="server">

        <insite:MultiFieldView runat="server" ID="CategoryFolderSelectorView" Inputs="CategoryFolderSelector">
            <span class="multi-field-input">
                <insite:CollectionItemFolderComboBox runat="server" ID="CategoryFolderSelector" />
            </span>
            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
        </insite:MultiFieldView>

        <insite:MultiFieldView runat="server" ID="CategoryFolderTextView" Inputs="CategoryFolderText">
            <span class="multi-field-input">
                <insite:TextBox runat="server" ID="CategoryFolderText" MaxLength="50" />
            </span>
            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
        </insite:MultiFieldView>

    </insite:MultiField>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Category Name
        <insite:RequiredValidator runat="server" ControlToValidate="CategoryName" FieldName="Name" ValidationGroup="Category" />
        <insite:CustomValidator runat="server" ID="CategoryNameUniqueValidator"
            ControlToValidate="CategoryName" Display="Dynamic" ValidationGroup="Category"
            ErrorMessage="The category name must be unique within the selected folder." />
    </label>
    <div>
        <insite:TextBox runat="server" ID="CategoryName" MaxLength="200" Width="100%" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Category Description
    </label>
    <div>
        <insite:TextBox ID="DescriptionInput" runat="server" MaxLength="800" TextMode="MultiLine" Rows="10" />
    </div>
    <div class="form-text"></div>
</div>
