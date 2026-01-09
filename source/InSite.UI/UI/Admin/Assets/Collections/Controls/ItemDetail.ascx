<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemDetail.ascx.cs" Inherits="InSite.Admin.Utilities.Collections.Controls.ItemDetail" %>

<div class="row">
    <div class="col-md-6">

        <div class="form-group mb-3">
            <label class="form-label">
                Organization
                <insite:RequiredValidator runat="server" ControlToValidate="OrganizationIdentifier" FieldName="Organization" ValidationGroup="CollectionItem" />
            </label>
            <div>
                <insite:FindOrganization runat="server" ID="OrganizationIdentifier" />
            </div>
        </div>
        
        <div class="form-group mb-3">
            <label class="form-label">
                Folder
            </label>
            <insite:MultiField runat="server">

                <insite:MultiFieldView runat="server" ID="ItemFolderSelectorView" Inputs="ItemFolderSelector">
                    <span class="multi-field-input">
                        <insite:CollectionItemFolderComboBox runat="server" ID="ItemFolderSelector" AllowBlank="false" />
                    </span>
                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                        ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                </insite:MultiFieldView>

                <insite:MultiFieldView runat="server" ID="ItemFolderTextView" Inputs="ItemFolderText">
                    <span class="multi-field-input">
                        <insite:TextBox runat="server" ID="ItemFolderText" MaxLength="50" />
                    </span>
                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                        ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                </insite:MultiFieldView>

            </insite:MultiField>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Name
                <insite:RequiredValidator runat="server" ControlToValidate="ItemName" FieldName="Name" ValidationGroup="CollectionItem" />
                <insite:CustomValidator runat="server" ID="ItemNameUniqueValidator"
                    ControlToValidate="ItemName" Display="Dynamic" ValidationGroup="CollectionItem"
                    ErrorMessage="The collection already contains an item having the same name within selected organization/folder." />
            </label>
            <insite:TextBox runat="server" ID="ItemName" MaxLength="200" Width="100%" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Description</label>
            <insite:TextBox runat="server" ID="ItemDescription" Width="100%" TextMode="MultiLine" Rows="3" MaxLength="100" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Status</label>
            <div>
                <asp:CheckBox runat="server" ID="ItemIsDisabled" Text="Disabled" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Color</label>
            <div>
                <insite:ColorComboBox runat="server" ID="ItemColor" CssClass="w-50" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Icon</label>
            <div>
                <insite:TextBox runat="server" ID="ItemIcon" MaxLength="32" Width="100%" />
                <asp:Literal runat="server" ID="IconExample" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Hours</label>
            <div>
                <insite:NumericBox runat="server" ID="ItemHours" DecimalPlaces="2" MinValue="0" />
            </div>
        </div>

    </div>
</div>