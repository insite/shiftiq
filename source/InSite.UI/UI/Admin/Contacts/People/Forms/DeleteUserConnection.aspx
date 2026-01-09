<%@ Page Language="C#" CodeBehind="DeleteUserConnection.aspx.cs" Inherits="InSite.UI.Admin.Contacts.People.Forms.DeleteUserConnection" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="../Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	

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
            
            <h3>Connection</h3>

            <dl class="row">
                <dt class="col-sm-3">Connected On</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ConnectedOn" /></dd>
            
                <dt class="col-sm-3">Relationship</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Relationship" /></dd>
            </dl>

            <h3>From</h3>
            <uc:PersonDetail runat="server" ID="FromPersonDetail" />

            <h3>To</h3>
            <uc:PersonDetail runat="server" ID="ToPersonDetail" />

            <div runat="server" id="ConfirmMessage" class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this connection?	
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
                The connection will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>
        </div>
    </div>
</div> 
</asp:Content>
