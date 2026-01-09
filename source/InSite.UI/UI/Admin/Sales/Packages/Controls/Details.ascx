<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Details.ascx.cs" Inherits="InSite.UI.Admin.Sales.Packages.Controls.Details" %>

<div class="form-group mb-3">
    <label class="form-label">
        Package Title
        <insite:RequiredValidator runat="server" ControlToValidate="ProductName" FieldName="Package Title" ValidationGroup="Package" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="ProductName" MaxLength="100" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Description
    </label>
    <div>
        <insite:TextBox runat="server" ID="ProductDescription" TextMode="MultiLine" Rows="5" MaxLength="2000" />
    </div>
</div>

<div class="row">

    <div class="col-md-4">
        <div class="form-group mb-3">
            <label class="form-label">
                Number of Products
            </label>
            <div>
                <insite:NumericBox runat="server" ID="ProductQuantity" NumericMode="Integer" MinValue="0" />
            </div>
        </div>
    </div>

    <div class="col-md-4">
        <div class="form-group mb-3">
            <label class="form-label">
                Price
            </label>
            <div>
                <insite:NumericBox runat="server" ID="ProductPrice" DecimalPlaces="2" MaxValue="999999.99" MinValue="0" />
            </div>
        </div>
    </div>

    <div class="col-md-4">
        <div class="form-group mb-3">
            <label class="form-label d-none d-md-inline-block">
                &nbsp;
            </label>
            <div class="mt-md-2">
                <insite:CheckBox runat="server" ID="IsTaxable" Text="Package is taxable" />
            </div>
        </div>
    </div>

</div>

<div class="form-group mb-3">
    <label class="form-label">
        Package Image
    </label>
    <div>
        <insite:FileUploadV2 runat="server" ID="PackageImageUpload" AllowedExtensions=".jpg,.png" FileUploadType="Image" />
    </div>
</div>

<div class="form-group mb-3">
    <asp:Literal ID="PackageImage" runat="server"></asp:Literal>
</div>
