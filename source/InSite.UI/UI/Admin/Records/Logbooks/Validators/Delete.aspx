<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Validators.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../../../Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    
    <div class="row settings">
        <div class="col-lg-6">
            <h3>Validator</h3>

            <uc:PersonDetail runat="server" ID="PersonDetail" />

            <dl class="row">
                <dt class="col-sm-3">Logbook Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="LogbookName" /></dd>

                <dt class="col-sm-3">Logbook Title:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="LogbookTitle" /></dd>
            </dl>

            <div class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this validator?	
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
                The validator will be deleted from the logbook.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Validators
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
