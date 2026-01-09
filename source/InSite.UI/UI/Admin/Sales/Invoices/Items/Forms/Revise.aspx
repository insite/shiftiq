<%@ Page Language="C#" CodeBehind="Revise.aspx.cs" Inherits="InSite.Admin.Invoices.Items.Forms.Revise" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Invoice" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-list me-1"></i>
            Revise Item
        </h2>

        <div class="row">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>

                        <div class="row">
                            <div class="col-md-12">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Description
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="ItemDescription" MaxLength="200" />
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-md-6">

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Price ($)
                                                    <insite:RequiredValidator runat="server" ID="PriceValidator" ControlToValidate="ItemPrice" FieldName="Price" ValidationGroup="Invoice" />
                                                </label>
                                                <div>
                                                    <insite:NumericBox runat="server" ID="ItemPrice" MinValue="0" DecimalPlaces="2" />
                                                </div>
                                            </div>

                                        </div>

                                        <div class="col-md-6">

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Quantity
                                                    <insite:RequiredValidator runat="server" ID="QuantityValidator" ControlToValidate="ItemQuantity" FieldName="Quantity" ValidationGroup="Invoice" />
                                                </label>
                                                <div>
                                                    <insite:NumericBox runat="server" ID="ItemQuantity" MinValue="1" NumericMode="Integer"/>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Product
                                            <insite:RequiredValidator runat="server" ID="ProductIDValidator" ControlToValidate="ProductID" FieldName="Product" ValidationGroup="Invoice" />
                                        </label>
                                        <div>
                                            <insite:ProductComboBox runat="server" ID="ProductID" />
                                        </div>
                                    </div>

                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Invoice" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
