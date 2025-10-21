<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Details.ascx.cs" Inherits="InSite.Admin.Payments.Discounts.Controls.Details" %>

<div class="row">

    <div class="col-md-12">

        <div class="form-group mb-3">
            <label class="form-label">
                Code
                <insite:RequiredValidator runat="server" ControlToValidate="DiscountCode" FieldName="Discount Code" ValidationGroup="Discount" />
            </label>
            <div>
                <insite:TextBox runat="server" ID="DiscountCode" Width="100%" MaxLength="20" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Percent
            </label>
            <div>
                <insite:NumericBox ID="DiscountPercent" runat="server" MinValue="0" MaxValue="100" DecimalPlaces="2" Width="100px" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Description
            </label>
            <div>
                <insite:TextBox runat="server" ID="DiscountDescription" Width="100%" TextMode="MultiLine" Rows="5" />
            </div>
        </div>

    </div>
</div>
