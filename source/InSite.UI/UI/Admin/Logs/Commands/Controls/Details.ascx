<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Details.ascx.cs" Inherits="InSite.Admin.Logs.Commands.Controls.Details" %>

<div class="form-group mb-3">
    <label class="form-label">
        Aggregate Type
        <insite:RequiredValidator runat="server" FieldName="Aggregate Type" ControlToValidate="AggregateType" ValidationGroup="Command" />
    </label>
    <div>
        <insite:ComboBox runat="server" ID="AggregateType" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Aggregate ID
        <insite:RequiredValidator runat="server" FieldName="Aggregate ID" ControlToValidate="AggregateIdentifier" ValidationGroup="Command" />
        <asp:CustomValidator runat="server" ID="AggregateIdentifierValidator" ErrorMessage="The system can't find an aggregate for ID you entered." ValidationGroup="Command" Display="None" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="AggregateIdentifier" MaxLength="36" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Command Type
        <insite:RequiredValidator runat="server" FieldName="Command Type" ControlToValidate="CommandTypeSelector" ValidationGroup="Command" />
        <insite:RequiredValidator runat="server" FieldName="Command Type" ControlToValidate="CommandTypeText" ValidationGroup="Command" />
        <asp:CustomValidator runat="server" ID="CustomCommandTypeValidator" ErrorMessage="Command Type you entered is not valid." ValidationGroup="Command" Display="None" />
    </label>
    <insite:MultiField runat="server">

        <insite:MultiFieldView runat="server" ID="CommandTypeSelectorView" Inputs="CommandTypeSelector">
            <span class="multi-field-input">
                <insite:ComboBox runat="server" ID="CommandTypeSelector" EnableSearch="true" AllowBlank="true" />
            </span>
            <insite:Button runat="server" ID="CommandTypeSelectorNextView"
                OnClientClick='inSite.common.multiField.nextView(this); return false;'
                ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
        </insite:MultiFieldView>

        <insite:MultiFieldView runat="server" ID="CommandTypeTextView" Inputs="CommandTypeText">
            <span class="multi-field-input">
                <insite:TextBox runat="server" ID="CommandTypeText" MaxLength="100" />
            </span>
            <insite:Button runat="server" ID="CommandTypeTextNextView"
                OnClientClick='inSite.common.multiField.nextView(this); return false;'
                ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
        </insite:MultiFieldView>

    </insite:MultiField>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Data JSON
        <insite:RequiredValidator runat="server" FieldName="Data JSON" ControlToValidate="CommandData" ValidationGroup="Command" />
        <asp:CustomValidator runat="server" ID="DataJsonValidator" ValidationGroup="Command" Display="None" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="CommandData" TextMode="MultiLine" Rows="5" style="resize:vertical;" />
        <asp:CheckBox runat="server" ID="DataJsonValidatorEnabled" Text="Validate JSON" Checked="true" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Description
    </label>
    <div>
        <insite:TextBox runat="server" ID="CommandDescription" TextMode="MultiLine" Rows="5" />
    </div>
</div>
