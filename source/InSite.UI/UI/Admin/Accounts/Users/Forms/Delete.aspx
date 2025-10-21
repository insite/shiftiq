<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Accounts.Users.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server">
    <style type="text/css">
        .panel-heading h3 { margin: 0px; padding: 0px; }
    </style>
</insite:PageHeadContent>

<div id="desktop">

    <insite:Container runat="server" ID="ErrorPanel" Visible="false">
        <div class="alert alert-danger" role="alert" style="margin-top:15px;">
             <i class="fas fa-stop-circle"></i>
            <asp:Literal runat="server" ID="ErrorText" />
        </div>
    </insite:Container>

    <div class="row settings">
        <div class="col-md-6">                     
            <div class="settings">
                <div class="card mb-3">
                    <div class="card-body">
                        <h3>User</h3>

                        <dl class="row">
                            <dt class="col-sm-3">User Name:</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="UserName" /></dd>

                            <dt class="col-sm-3">Email:</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="Email" /></dd>
                        </dl>
                    </div>
                </div>

                <div runat="server" id="ConfirmMessage" class="alert alert-danger" role="alert">
                     <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this user?
                </div>

                <p style="padding-bottom:10px;">
                    <insite:DeleteButton runat="server" ID="DeleteButton" />
                    <insite:CancelButton runat="server" ID="CancelButton" />
                    <insite:CloseButton runat="server" ID="CloseButton" Visible="false" />
                </p>
            </div>
        </div>

        <div class="col-md-6">
            <div class="card">
                <div class="card-body">
                    <h3>Impact</h3>

                    <div class="alert alert-warning">
                        <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                        This is a permanent change that cannot be undone. 
                        The user will be deleted from all forms, queries, and reports.
                        Here is a summary of the data that will be erased if you proceed.
                    </div>

                    <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                        <tr>
                            <td>Type</td>
                            <td>Rows</td>
                        </tr>
                        <tr>
                            <td>User</td>
                            <td>1</td>
                        </tr>
                        <tr>
                            <td>
                                Organizations
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="OrganizationsCount" />
                            </td>
                        </tr>

                    </table>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
