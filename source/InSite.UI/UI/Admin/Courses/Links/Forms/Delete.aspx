<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Courses.Links.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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
            <h3>Link</h3>

            <dl class="row">
                <dt class="col-sm-3">Title</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="TitleOutput" /></dd>
                
                <dt class="col-sm-3">Description</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Description" /></dd>
                
                <dt class="col-sm-3">URL</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Url" /></dd>
                    
                <dt class="col-sm-3">Code</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Code" /></dd>
                    
                <dt class="col-sm-3">Publisher</dt>
                <dd class="col-sm-9"><asp:Literal ID="Publisher" runat="server" /></dd>
                    
                <dt class="col-sm-3">Subtype</dt>
                <dd class="col-sm-9"><asp:Literal ID="Subtype" runat="server" /></dd>
                    
                <dt class="col-sm-3">Location</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Location" /></dd>
                    
                <dt class="col-sm-3">Key</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Key" /></dd>

                <dt class="col-sm-3">Secret</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Secret" /></dd>
            </dl>

            <div runat="server" id="ConfirmMessage" class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this link?	
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
                The link will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Links
                    </td>
                    <td>
                        1
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div> 
</asp:Content>
