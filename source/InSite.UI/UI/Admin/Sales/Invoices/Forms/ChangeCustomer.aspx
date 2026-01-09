<%@ Page Language="C#" CodeBehind="ChangeCustomer.aspx.cs" Inherits="InSite.Admin.Invoices.Forms.ChangeCustomer" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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

                            <div class="col-md-12">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Issued to Company
                                    </label>
                                    <div>
                                        <insite:FindGroup runat="server" ID="BillingCustomerID" />
                                    </div>
                                    <div class="form-text">
                                        The Employer of the Customer for the invoice.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Issued to Contact
                                        <insite:RequiredValidator runat="server" ControlToValidate="CustomerID" FieldName="Issued to Contact" ValidationGroup="Invoice" />
                                    </label>
                                    <div>
                                        <insite:FindPerson runat="server" ID="CustomerID" />
                                    </div>
                                    <div class="form-text">
                                        The Customer for the invoice.
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
