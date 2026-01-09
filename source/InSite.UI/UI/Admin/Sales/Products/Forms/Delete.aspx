<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Invoices.Products.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" ValidationGroup="Product" />

    
    <div class="row settings">
        <div class="col-md-6">
            <h3>Product</h3>

            <dl class="row">
                <dt class="col-sm-3">Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ProductName" /></dd>

                <dt class="col-sm-3">Description:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ProductDescription" /></dd>

                <dt class="col-sm-3">Type:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ProductType" /></dd>
            </dl>

            <div class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this product?
            </div>

            <div class="col-lg-12">
                <insite:DeleteButton runat="server" ID="DeleteButton" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>
        </div>

        <div class="col-md-6">

            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The product will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>
        </div>
    </div>
</div>
</asp:Content>
