<%@ Page Language="C#" CodeBehind="ChangeInvoiceDetails.aspx.cs" Inherits="InSite.Admin.Invoices.Forms.ChangeInvoiceDetails" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">


    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Invoice" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-file-invoice-dollar me-1"></i>
            Invoice
        </h2>

        <div class="row">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>

                        <div class="row">

                            <div class="col-md-6">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Invoice Status
                                        <insite:RequiredValidator runat="server" ControlToValidate="InvoiceStatus" FieldName="Invoice Status" ValidationGroup="Invoice" />
                                    </label>
                                    <div>
                                        <insite:InvoiceStatusComboBox runat="server" ID="InvoiceStatus" />
                                    </div>
                                    <div class="form-text">
                                        Update Invoice status.
                                    </div>
                                </div>


                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Invoice Date Paid
                                        <insite:RequiredValidator runat="server"
                                            ID="InvoicePaidOnRequired"
                                            ControlToValidate="InvoicePaidOn"
                                            FieldName="Invoice Date Paid"
                                            ValidationGroup="Invoice"
                                        />
                                    </label>
                                    <div>
                                        <label>
                                            <insite:DateTimeOffsetSelector ID="InvoicePaidOn" runat="server" EmptyMessage="None" />
                                        </label>
                                    </div>
                                    <div class="form-text">
                                        Update Invoice Paid Date.
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
