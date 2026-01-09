<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Utilities.Collections.Controls.Detail" %>

<div class="row">
    <div class="col-lg-4">
            
        <div class="form-group mb-3">
            <label class="form-label">
                Name
                <insite:RequiredValidator runat="server" ControlToValidate="CollectionName" FieldName="Name" ValidationGroup="Collection" />
                <insite:CustomValidator runat="server" ID="CollectionNameUniqueValidator"
                    ControlToValidate="CollectionName" Display="Dynamic" ValidationGroup="Collection"
                    ErrorMessage="The system already contains a collection having the same name." />
            </label>
            <insite:TextBox runat="server" ID="CollectionName" MaxLength="250" />
        </div>

    </div>

    <div class="col-lg-4">

        <div class="form-group mb-3">
            <label class="form-label">
                Toolkit
                <insite:RequiredValidator runat="server" ControlToValidate="CollectionToolSelector" FieldName="Tool" ValidationGroup="Collection" />
                <insite:RequiredValidator runat="server" ControlToValidate="CollectionToolText" FieldName="Tool" ValidationGroup="Collection" />
            </label>

            <insite:MultiField runat="server">

                <insite:MultiFieldView runat="server" ID="CollectionToolSelectorView" Inputs="CollectionToolSelector">
                    <span class="multi-field-input">
                        <insite:CollectionToolComboBox runat="server" ID="CollectionToolSelector" AllowBlank="false" />
                    </span>
                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                        ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                </insite:MultiFieldView>

                <insite:MultiFieldView runat="server" ID="CollectionToolTextView" Inputs="CollectionToolText">
                    <span class="multi-field-input">
                        <insite:TextBox runat="server" ID="CollectionToolText" MaxLength="100" />
                    </span>
                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                        ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                </insite:MultiFieldView>

            </insite:MultiField>

        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Package
            </label>

            <insite:MultiField runat="server">

                <insite:MultiFieldView runat="server" ID="CollectionPackageSelectorView" Inputs="CollectionPackageSelector">
                    <span class="multi-field-input">
                        <insite:CollectionPackageComboBox runat="server" ID="CollectionPackageSelector" AllowBlank="false" />
                    </span>
                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                        ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                </insite:MultiFieldView>

                <insite:MultiFieldView runat="server" ID="CollectionPackageTextView" Inputs="CollectionPackageText">
                    <span class="multi-field-input">
                        <insite:TextBox runat="server" ID="CollectionPackageText" MaxLength="100" />
                    </span>
                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                        ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                </insite:MultiFieldView>

            </insite:MultiField>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Process
                <insite:RequiredValidator runat="server" ControlToValidate="CollectionProcessSelector" FieldName="Process" ValidationGroup="Collection" />
                <insite:RequiredValidator runat="server" ControlToValidate="CollectionProcessText" FieldName="Process" ValidationGroup="Collection" />
            </label>

            <insite:MultiField runat="server">

                <insite:MultiFieldView runat="server" ID="CollectionProcessSelectorView" Inputs="CollectionProcessSelector">
                    <span class="multi-field-input">
                        <insite:CollectionProcessComboBox runat="server" ID="CollectionProcessSelector" AllowBlank="false" />
                    </span>
                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                        ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                </insite:MultiFieldView>

                <insite:MultiFieldView runat="server" ID="CollectionProcessTextView" Inputs="CollectionProcessText">
                    <span class="multi-field-input">
                        <insite:TextBox runat="server" ID="CollectionProcessText" MaxLength="100" />
                    </span>
                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                        ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                </insite:MultiFieldView>

            </insite:MultiField>

        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Type
                <insite:RequiredValidator runat="server" ControlToValidate="CollectionTypeSelector" FieldName="Type" ValidationGroup="Collection" />
                <insite:RequiredValidator runat="server" ControlToValidate="CollectionTypeText" FieldName="Type" ValidationGroup="Collection" />
            </label>

            <insite:MultiField runat="server">

                <insite:MultiFieldView runat="server" ID="CollectionTypeSelectorView" Inputs="CollectionTypeSelector">
                    <span class="multi-field-input">
                        <insite:CollectionTypeComboBox runat="server" ID="CollectionTypeSelector" AllowBlank="false" />
                    </span>
                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                        ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                </insite:MultiFieldView>

                <insite:MultiFieldView runat="server" ID="CollectionTypeTextView" Inputs="CollectionTypeText">
                    <span class="multi-field-input">
                        <insite:TextBox runat="server" ID="CollectionTypeText" MaxLength="20" />
                    </span>
                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                        ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                </insite:MultiFieldView>

            </insite:MultiField>

        </div>

    </div>

    <div class="col-lg-4">

        <div class="form-group mb-3">
            <label class="form-label">
                References
            </label>
            <insite:TextBox runat="server" ID="CollectionReferences" MaxLength="100" />
        </div>

    </div>

</div>