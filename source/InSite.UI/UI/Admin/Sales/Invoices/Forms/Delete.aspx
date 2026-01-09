<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Invoices.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <asp:Panel runat="server" ID="ErrorPanel" Visible="false">	
        <div class="alert alert-danger" role="alert">
             <i class="fas fa-stop-circle"></i>
            <asp:Literal runat="server" ID="ErrorText" />	
        </div>	
    </asp:Panel>

    <div class="row settings">
        <div class="col-md-6">
            <h3>Invoice</h3>

            <dl class="row">
                <dt class="col-sm-3">Customer:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="CustomerName" /></dd>

                <dt class="col-sm-3">Invoice Status:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="InvoiceStatus" /></dd>

                <dt class="col-sm-3">Invoice Drafted:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="InvoiceDrafted" /></dd>

                <dt class="col-sm-3">Invoice Submitted:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="InvoiceSubmitted" /></dd>

                <dt class="col-sm-3">Invoice Paid:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="InvoicePaid" /></dd>
            </dl>

            <div runat="server" id="ConfirmMessage" class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this invoice?	
            </div>	

            <p style="padding-bottom:10px;">	
                <insite:DeleteButton runat="server" ID="DeleteButton" />	
                <insite:CancelButton runat="server" ID="CancelButton" />
            </p>
        </div>

        <div class="col-md-6">
            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The invoice will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Invoices
                    </td>
                    <td>
                        1
                    </td>
                </tr>
                <tr>
                    <td>
                        Items
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="ItemCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Payments
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="PaymentCount" />
                    </td>
                </tr>
            </table>

        </div>
    </div>

</div> 
</asp:Content>
