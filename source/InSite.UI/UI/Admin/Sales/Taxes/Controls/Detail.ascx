<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.UI.Admin.Sales.Taxes.Controls.Detail" %>

<div class="row">

    <div class="col-md-12">

        <div class="form-group mb-3">
            <label class="form-label">
                Name
                <insite:RequiredValidator runat="server" ControlToValidate="TaxName" FieldName="Name" ValidationGroup="Tax" />
            </label>
            <div>
                <insite:TextBox runat="server" ID="TaxName" MaxLength="20" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Province / State
                <insite:RequiredValidator runat="server" ControlToValidate="RegionCode" FieldName="Province / State" ValidationGroup="Tax" />
            </label>
            <div>
                <insite:ProvinceComboBox ID="RegionCode" runat="server" EnableSearch="true" UseCodeAsValue="true" OnlyCanada="true"/>
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Percent
                <insite:RequiredValidator runat="server" ControlToValidate="TaxPercent" FieldName="Percent" ValidationGroup="Tax" />
            </label>
            <div class="d-flex align-items-center gap-2">
                <insite:NumericBox ID="TaxPercent" runat="server" MinValue="0" MaxValue="100" DecimalPlaces="2" Width="100px" /> %
            </div>
        </div>

    </div>
</div>
