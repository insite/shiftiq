<%@ Page Language="C#" CodeBehind="DeleteItem.aspx.cs" Inherits="InSite.UI.Admin.Assets.Collections.Forms.DeleteItem" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server">
    <style type="text/css">
        .color-example {
            display: inline-block;
            height: 16px;
            width: 16px;
            border-radius: 8px;
            margin: 0 3px 2px 0;
            vertical-align: middle;
        }
    </style>
</insite:PageHeadContent>

<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" ValidationGroup="Product" />

    <div class="row settings">
        <div class="col-md-6">
            <h3>Collection Item</h3>

            <dl class="row">
                <dt class="col-sm-3">Organization:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="OrganizationName" /></dd>

                <dt class="col-sm-3">Folder:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="FolderName" /></dd>

                <dt class="col-sm-3">Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="CollectionItemName" /></dd>

                <dt class="col-sm-3">Description:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Description" /></dd>

                <dt class="col-sm-3">Status:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Status" /></dd>

                <dt class="col-sm-3">HTML Color:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="HTMLColor" /></dd>

                <dt class="col-sm-3">Icon:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="IconExample" /></dd>

                <dt class="col-sm-3">Hours:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ItemHours" /></dd>

                <dt class="col-sm-3">Collection Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="CollectionName" /></dd>
            </dl>

            <div class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this collection item?
            </div>

            <div>
                <insite:DeleteButton runat="server" ID="DeleteButton" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>
        </div>

        <div class="col-md-6">

            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The collection item will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>
                    
        </div>
    </div>
</div>
</asp:Content>
