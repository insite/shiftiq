<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Messages.Subscribers.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="../../../Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	

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
            <h3>User</h3>

            <uc:PersonDetail runat="server" ID="PersonDetail" />

            <dl class="row">
                <dt class="col-sm-3">Subscribed:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Subscribed" /></dd>
            </dl>

            <div runat="server" id="ConfirmMessage" class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to remove this subscriber?	
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
                This change cannot be undone. 
                The subscriber listed here must be re-added manually once removed. 
                The subscriber will not be included on any newly scheduled mailouts, 
                although they will continue to receive any pending mailouts.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Subscribers
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="SubscriberCount" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div> 
</asp:Content>
