<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Assets.Labels.Delete" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-5 mb-md-2">
        
        <div class="row">                       

            <div class="col-lg-6">
                
                <h2 class="h4 mb-3">UI Label</h2>
                <dl class="row">
                    <dt class="col-sm-3">Placeholder Name:</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="PlaceHolderName" /></dd>

                    <dt class="col-sm-3">Default Text:</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="DefaultText" /></dd>
                </dl>

                <div runat="server" id="ConfirmMessage" class="alert alert-danger mt-4" role="alert">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this UI Label?	
                </div>	

                <div>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />	
                    <insite:CancelButton runat="server" ID="CancelButton" />	
                    <insite:CloseButton runat="server" ID="CloseButton" Visible="false" />
                </div>
                
            </div>

            <div class="col-lg-6">

                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone. 
                    The UI label will be deleted from all forms, queries, and reports.
                </div>
            </div>
        </div>
  </section>
</asp:Content>
        
