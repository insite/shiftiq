<%@ Page Language="C#" CodeBehind="DeleteMembership.aspx.cs" Inherits="InSite.Admin.Contacts.People.Forms.DeleteMembership" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <div class="row settings">
                        
        <div class="col-md-6">
            <dl class="row">
                <dt class="col-sm-3">Assigned On</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="AssignedOn" /></dd>
                    
                <dt class="col-sm-3">Membership Function</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="RoleType" /></dd>

                <dt class="col-sm-3">User Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="FullName" /></dd>

                <dt class="col-sm-3">Group Name:</dt>   
                <dd class="col-sm-9">
                    <a runat="server" id="GroupLink">
                        <asp:Literal runat="server" ID="GroupName" />
                    </a>
                </dd>
            </dl>

            <div runat="server" id="ConfirmMessage" class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this person's membership?	
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
                The membership will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                <tr>
                    <td>Membership</td>
                    <td>1</td>
                </tr>
                <tr>
                    <td>Number of Reasons</td>
                    <td><asp:Literal runat="server" ID="ReasonCount" /></td>
                </tr>
            </table>
        </div>
    </div>
</div>
</asp:Content>