<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Assets.Collections.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/CollectionInfo.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" ValidationGroup="Product" />

    <div class="row settings">
        <div class="col-md-6">
            <h3>Collection</h3>
            <uc:Detail runat="server" ID="Detail" />

            <div class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this collection?
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
                The collection will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Collections
                    </td>
                    <td>
                        1
                    </td>
                </tr>
                <tr>
                    <td>
                        Collection Items
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="ItemsCount" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
</asp:Content>
