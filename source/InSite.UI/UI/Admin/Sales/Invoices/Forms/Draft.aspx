<%@ Page Language="C#" CodeBehind="Draft.aspx.cs" Inherits="InSite.Admin.Invoices.Forms.Draft" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Invoice" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-file-invoice-dollar me-1"></i>
            Draft Invoice
        </h2>

        <div class="row">

            <div class="col-lg-8">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>

                        <div class="row">

                            <div class="col-md-6">

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
                                        Invoice Status
                                        <insite:RequiredValidator runat="server" ControlToValidate="InvoiceStatus" FieldName="Invoice Status" ValidationGroup="Invoice" />
                                    </label>
                                    <div>
                                        <insite:InvoiceStatusComboBox runat="server" ID="InvoiceStatus" AllowBlank="false" />
                                    </div>
                                    <div class="form-text">
                                        Update Invoice status.
                                    </div>
                                </div>

                            </div>
                            <div class="col-md-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Issued to Contact
                                        <insite:RequiredValidator runat="server" ControlToValidate="EmployerContactID" FieldName="Employer Contact" ValidationGroup="Invoice" />
                                    </label>
                                    <div>
                                        <insite:FindPerson runat="server" ID="EmployerContactID" />
                                    </div>
                                    <div class="form-text">
                                        The Customer for the invoice.
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">

                            <div class="col-md-12">

                                <h3>Add New Item(s)</h3>

                                <div class="float-end pb-3">
                                    <insite:Button runat="server" ID="AddItemButton" Text="Add Item" Icon="fas fa-plus-circle" ButtonStyle="Default" />
                                </div>

                                <asp:Repeater runat="server" ID="ItemRepeater">
                                    <HeaderTemplate>
                                        <table class="table table-striped">
                                            <thead>
                                                <tr>
                                                    <th>#</th>
                                                    <th>Description</th>
                                                    <th>Product</th>
                                                    <th>Quantity</th>
                                                    <th>Unit Price ($)</th>
                                                    <th></th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <FooterTemplate>
                                        </tbody></table>
                                    </FooterTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="align-middle" style="vertical-align: middle;">
                                                <%# Container.ItemIndex + 1 %>
                                            </td>
                                            <td>
                                                <insite:TextBox runat="server" ID="Description" MaxLength="70" />
                                            </td>
                                            <td>
                                                <div class="hstack">
                                                    <insite:ProductComboBox runat="server" ID="ProductID" AutoPostBack="true" CssClass="me-1" />
                                                    <insite:RequiredValidator runat="server" ControlToValidate="ProductID" FieldName="Product" ValidationGroup="Invoice" />
                                                </div>
                                            </td>
                                            <td>
                                                <div class="hstack">
                                                    <insite:NumericBox runat="server" ID="Quantity" MinValue="1" NumericMode="Integer" CssClass="me-1" />
                                                    <insite:RequiredValidator runat="server" ControlToValidate="Quantity" FieldName="Quantity" ValidationGroup="Invoice" />
                                                </div>
                                            </td>
                                            <td>
                                                <div class="hstack">
                                                    <insite:NumericBox runat="server" ID="Price" MinValue="0" DecimalPlaces="2" CssClass="me-1" />
                                                    <insite:RequiredValidator runat="server" ControlToValidate="Price" FieldName="Price" ValidationGroup="Invoice" />
                                                </div>
                                            </td>
                                            <td>
                                                <insite:IconButton runat="server" ID="VoidItemButton"
                                                    Name="trash-alt"
                                                    ToolTip="Void Item"
                                                    ConfirmText="Are you sure you want to delete this item?"
                                                    CommandName="Delete"
                                                    CommandArgument='<%# Container.ItemIndex %>'
                                                    Visible='<%# CanDeleteItem %>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
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
