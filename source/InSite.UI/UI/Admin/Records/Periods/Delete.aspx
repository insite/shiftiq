<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Records.Periods.Delete" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div runat="server" id="NoVoid" class="alert alert-danger" role="alert">
         <i class="fas fa-stop-circle"></i> This period is used by gradebook(s) and therefore cannot be deleted.
    </div>

    <asp:Panel runat="server" ID="ErrorPanel" Visible="false">	
        <div class="alert alert-danger" role="alert">
            <i class="fas fa-stop-circle"></i>
            <asp:Literal runat="server" ID="ErrorText" />	
        </div>	
    </asp:Panel>
        
    <div class="row">                       

        <div class="col-lg-6">                                   
                
            <h2 class="h4 mb-3">Period</h2>
                
            <dl class="row">
                <dt class="col-sm-3">Period Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="PeriodName" /></dd>
                
                <dt class="col-sm-3">Start/End:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="PeriodText" /></dd>
            </dl>

            <div runat="server" id="ConfirmMessage" class="alert alert-danger mt-4" role="alert">
                <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this Period?	
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
                The period will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                <tr>
                    <td>
                        Periods
                    </td>
                    <td>
                        1
                    </td>
                </tr>
                <tr>
                    <td>
                        Gradebooks
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="GradebookCount" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>