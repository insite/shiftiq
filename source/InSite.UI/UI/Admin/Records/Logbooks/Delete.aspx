<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Delete" %>

<%@ Register Src="~/UI/Admin/Records/Logbooks/Controls/LogbookDetails.ascx" TagName="Details" TagPrefix="uc" %>	

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <div runat="server" id="NoVoid" class="alert alert-danger" role="alert">
         <i class="fas fa-stop-circle"></i> This logbook cannot be deleted until all entries are deleted from the logbook.
    </div>

    <section class="pb-5 mb-md-2">

        <asp:Panel runat="server" ID="ErrorPanel" Visible="false">	
            <div class="alert alert-danger" role="alert">
                <i class="fas fa-stop-circle"></i>
                <asp:Literal runat="server" ID="ErrorText" />	
            </div>	
        </asp:Panel>
        
        <div class="row">                       

            <div class="col-lg-6">                            
                
                <h2 class="h4 mb-3">Logbook</h2>
                
                <uc:Details runat="server" ID="LogbookDetail" />

                <div runat="server" class="alert alert-danger mt-4" role="alert">
                     <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong> Are you sure you want to proceed?
                        Are you sure you want to delete this logbook?
                    <div>
                        <asp:CheckBox runat="server" ID="DeleteJournalsCheckBox" Text="Delete All Entries with this logbook" />
                    </div>
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
                    The logbook will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                    <tr>
                        <td>
                            Logbooks
                        </td>
                        <td>
                            1
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Fields
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="FieldCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Competencies
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="CompetencyCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Learners
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="UserCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Entries
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="JournalCount" />
                        </td>
                    </tr>
                </table>     
            </div>
       </div>
    </section>
</asp:Content>