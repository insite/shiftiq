<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Accounts.Senders.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">
    
    <div class="row settings">
        <div class="col-md-6">
            <div class="card mb-3">
                <div class="card-body">
                    <h3>Sender</h3>

                    <dl class="row">
                        <dt class="col-sm-3">Nickname:</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="Nickname" /></dd>

                        <dt class="col-sm-3">Type:</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="SenderType" /></dd>

                        <dt class="col-sm-3">From Name:</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="FromName" /></dd>

                        <dt class="col-sm-3">From Email (Reply To):</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="FromEmail" /></dd>

                        <dt class="col-sm-3">System Mailbox:</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="SystemMailbox" /></dd>

                        <dt class="col-sm-3">Sender Status:</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="SenderStatus" /></dd>
                    </dl>
                </div>
            </div>

            <div class="alert alert-danger" role="alert" runat="server" ID="ConfirmText">
                <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this sender?
            </div>

            <div>
                <insite:DeleteButton runat="server" ID="DeleteButton" />
                <insite:CancelButton runat="server" ID="CancelButton" Identifier="Cancel" />
            </div>
        </div>

        <div class="col-md-6">
            <div class="card">
                <div class="card-body">
                    <h3>Impact</h3>

                    <div class="alert alert-warning">
                        <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                        This is a permanent change that cannot be undone. 
                        The sender will be deleted from all forms, queries, and reports.
                        Here is a summary of the data that will be erased if you proceed.
                    </div>

                    <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                        <tr>
                            <td>
                                Type
                            </td>
                            <td>
                                Rows
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Sender
                            </td>
                            <td>
                                1
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Organizations
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="OrganizationsCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Messages
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="MessagesCount" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
