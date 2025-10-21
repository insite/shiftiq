<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Entries.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">	

    <section class="pb-5 mb-md-2">

        <asp:Panel runat="server" ID="ErrorPanel" Visible="false">	
            <div class="alert alert-danger" role="alert">
                <i class="fas fa-stop-circle"></i>
                <asp:Literal runat="server" ID="ErrorText" />	
            </div>	
        </asp:Panel>
        
        <div class="row">                       

            <div class="col-lg-6">                                   
                
                <h2 class="h4 mb-3">Summary</h2>
                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <dl class="row">
                            <dt class="col-sm-4">Learner:</dt>
                            <dd class="col-sm-8"><asp:Literal runat="server" ID="LearnerName" /> (<asp:Literal runat="server" ID="LearnerEmail" />)</dd>
                        </dl>

                        <dl class="row">
                            <dt class="col-sm-4">Logbook:</dt>
                            <dd class="col-sm-8"><asp:Literal runat="server" ID="LogbookName" /></dd>
                        </dl>
                        
                        <dl class="row">
                            <dt class="col-sm-4 no-wrap">Entry Number:</dt>
                            <dd class="col-sm-8"><asp:Literal runat="server" ID="EntryNumber" /></dd>
                        </dl>

                        <dl class="row">
                            <dt class="col-sm-4 no-wrap">Entry Created:</dt>
                            <dd class="col-sm-8"><asp:Literal runat="server" ID="EntryCreated" /></dd>
                        </dl>

                    </div>
                </div>

                <div runat="server" id="ConfirmMessage" class="alert alert-danger mt-4" role="alert">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this logbook entry?	
                </div>	

                <div>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />	
                    <insite:CancelButton runat="server" ID="CancelButton" />	
                    <insite:CloseButton runat="server" ID="CloseButton" Visible="false" />
                </div>

            </div>

            <div class="col-lg-6">

                <h3>Impact</h3>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="alert alert-warning">
                            <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                            This is a permanent change that cannot be undone. 
                            The logbook entry will be deleted from all forms, queries, and reports.
                            Here is a summary of the data that will be erased if you proceed.
                        </div>

                        <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                            <tr>
                                <td>Logbook Entry</td>
                                <td>1</td>
                            </tr>
                        </table>

                    </div>
                </div>

            </div>

        </div>

    </section>

</asp:Content>