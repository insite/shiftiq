<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Invoices.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/InvoiceItemGrid.ascx" TagName="InvoiceItemGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/PaymentGrid.ascx" TagName="PaymentGrid" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section class="pb-5 mb-md-2">

        <div class="row">
            <div class="col-lg-8">

                <h2 class="h4 mb-3">Invoice</h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="row button-group mb-4">
                            <div class="col-lg-12">
                                <insite:Button runat="server" ID="NewInvoiceLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/sales/invoices/draft" />

                                <insite:ButtonSpacer runat="server" />

                                <insite:Button runat="server" ID="PrintInvoiceEventButton" ButtonStyle="Default" Text="Print Invoice" Icon="fas fa-print" />

                                <insite:ButtonSpacer runat="server" ID="HideSpacer" />

                                <insite:Button runat="server" ID="ViewHistoryLink" Text="History" Icon=" fas fa-history" ButtonStyle="Default" />

                                <insite:DeleteButton runat="server" ID="DeleteButton" Text="Delete Invoice" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-4">
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="ChangeCustomer" Style="padding: 8px" ToolTip="Change Issued to Contact" />
                                    </div>
                                    <asp:Label ID="EmployerContactNameLabel" runat="server" Text="Issued to Contact" CssClass="form-label" AssociatedControlID="EmployerContactName" />
                                    <div>
                                        <asp:Literal runat="server" ID="EmployerContactName" />
                                    </div>
                                    <div class="form-text">The customer for this invoice.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="UpdateInvoiceStatus" Style="padding: 8px" ToolTip="Update Invoice Status" />
                                    </div>
                                    <asp:Label ID="InvoiceStatusLabel" runat="server" Text="Invoice Status" CssClass="form-label" AssociatedControlID="InvoiceStatus" />
                                    <div>
                                        <asp:Literal runat="server" ID="InvoiceStatus" />
                                    </div>
                                    <div class="form-text">The status for this invoice.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Label ID="InvoiceIdentifierLabel" runat="server" Text="Invoice Identifier" CssClass="form-label" AssociatedControlID="InvoiceIdentifier" />
                                    <div>
                                        <asp:Literal runat="server" ID="InvoiceIdentifier" />
                                    </div>
                                    <div class="form-text">A globally unique identifier for this invoice.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Label ID="InvoiceNumberLabel" runat="server" Text="Invoice Number" CssClass="form-label" AssociatedControlID="InvoiceNumber" />
                                    <div>
                                        <asp:Literal runat="server" ID="InvoiceNumber" />
                                    </div>
                                    <div class="form-text">A unique number for this invoice.</div>
                                </div>
                            </div>
                            <div class="col-4">
                                <div class="form-group mb-3" runat="server">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="ChangeIssuedToCompany" Style="padding: 8px" ToolTip="Change Issued to Company" />
                                    </div>
                                    <asp:Label ID="BusinessCustomerNameLabel" runat="server" Text="Issued to Company" CssClass="form-label" AssociatedControlID="BusinessCustomerNameLabel" />
                                    <div>
                                        <asp:Literal runat="server" ID="BusinessCustomerName" />
                                    </div>
                                </div>
                            </div>
                            <div class="col-4">
                                <div class="form-group mb-3">
                                    <asp:Label ID="InvoiceDraftedLabel" runat="server" Text="Invoice Drafted" CssClass="form-label" AssociatedControlID="InvoiceDrafted" />
                                    <div>
                                        <asp:Literal runat="server" ID="InvoiceDrafted" />
                                    </div>
                                    <div class="form-text">The invoice has been drafted.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Label ID="InvoiceSubmittedLabel" runat="server" Text="Invoice Submitted" CssClass="form-label" AssociatedControlID="InvoiceSubmitted" />
                                    <div>
                                        <asp:Literal runat="server" ID="InvoiceSubmitted" />
                                    </div>
                                    <div class="form-text">The invoice has been submitted.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="UpdateInvoicePaidDate" Style="padding: 8px" ToolTip="Update Invoice Paid Date" />
                                    </div>
                                    <asp:Label ID="InvoicePaidLabel" runat="server" Text="Invoice Paid" CssClass="form-label" AssociatedControlID="InvoicePaid" />
                                    <div>
                                        <asp:Literal runat="server" ID="InvoicePaid" />
                                    </div>
                                    <div class="form-text">The invoice has been paid.</div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

            </div>
        </div>

    </section>

    <section runat="server" id="ItemSection" class="pb-5 mb-md-2">

        <div class="row">
            <div class="col-lg-12">

                <h2 class="h4 mb-3">Line Items</h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <uc:InvoiceItemGrid runat="server" ID="InvoiceItemGrid" />

                    </div>
                </div>

            </div>
        </div>

    </section>

    <section runat="server" id="PaymentSection" class="pb-5 mb-md-2">

        <div class="row">
            <div class="col-lg-12">

                <h2 class="h4 mb-3">Payments</h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <uc:PaymentGrid runat="server" ID="PaymentGrid" />

                    </div>
                </div>

            </div>
        </div>

    </section>

    <section runat="server" id="NavigationSection" class="pb-5 mb-md-2" visible="false">
        <div>
            <insite:CancelButton runat="server" ID="CancelButton" Text="Close" />
        </div>
    </section>

</asp:Content>
