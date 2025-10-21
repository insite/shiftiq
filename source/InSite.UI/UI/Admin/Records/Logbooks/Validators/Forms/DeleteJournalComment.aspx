<%@ Page Language="C#" CodeBehind="DeleteJournalComment.aspx.cs" Inherits="InSite.UI.Admin.Records.Validators.Forms.DeleteJournalComment" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <div class="row settings">
        <div class="col-lg-6">
            <h3>Comment</h3>
            <dl class="row">
                <dt class="col-sm-3">Posted</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Posted" /></dd>

                <dt class="col-sm-3">Text</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Text" /></dd>

                <dt class="col-sm-3">User Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="UserName" /></dd>

                <dt class="col-sm-3">Email:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="UserEmail" /></dd>

                <dt class="col-sm-3">Logbook Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="LogbookName" /></dd>

                <dt class="col-sm-3">Logbook Title:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="LogbookTitle" /></dd>
            </dl>

            <div runat="server" class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong> Are you sure you want to proceed?
                 Are you sure you want to delete this comment?
            </div>
            <p>	
                <insite:DeleteButton runat="server" ID="DeleteButton" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </p>
        </div>

        <div class="col-lg-6">

            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The comment will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Comments
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
