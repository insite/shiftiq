<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Events.Comments.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <div class="row settings">
        <div class="col-md-6">
            <h3>Comment</h3>

            <dl class="row">
                <dd class="col-sm-12"><asp:Literal runat="server" ID="CommentText" /></dd>

                <dt class="col-sm-3">Author:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="PostedBy" /></dd>

                <dt class="col-sm-3">Posted On:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="PostedOn" /></dd>

                <dt class="col-sm-3">Event Title:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="EventTitle" /></dd>
            </dl>

            <div class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this comment?	
            </div>

            <p>	
                <insite:DeleteButton runat="server" ID="DeleteButton" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </p>
        </div>

        <div class="col-md-6">

            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                <asp:Literal runat="server" ID="WarningText" />
            </div>

        </div>
    </div>

</div>
</asp:Content>
