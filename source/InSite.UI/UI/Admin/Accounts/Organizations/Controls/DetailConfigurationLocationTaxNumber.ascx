<%@ Control Language="C#" CodeBehind="DetailConfigurationLocationTaxNumber.ascx.cs" Inherits="InSite.UI.Admin.Accounts.Organizations.Controls.DetailConfigurationLocationTaxNumber" %>

<div class="form-group mb-3">
    <label class="form-label">
        Tax Number Label
        <insite:CustomValidator runat="server" ID="TaxNumberLabelValidator" ValidationGroup="Organization" Display="None"
            ErrorMessage="Tax Number Label is a required field when Tax Number has a value" />
    </label>
    <insite:TextBox runat="server" ID="TaxNumberLabel" MaxLength="20" />
    <div class="form-text">The label for your tax number (e.g. GST/HST, VAT), shown on invoices and receipts.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Tax Number</label>
    <insite:TextBox runat="server" ID="TaxNumberValue" MaxLength="50" />
    <div class="form-text">Tax registration number shown on invoices and receipts. Leave blank to hide.</div>
</div>
