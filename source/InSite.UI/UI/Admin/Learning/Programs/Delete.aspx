<%@  Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Records.Programs.Delete" %>

<%@ Register Src="Controls/ProgramDetails.ascx" TagName="Details" TagPrefix="uc" %>

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
                
                <h2 class="h4 mb-3">Program</h2>
                
                <uc:Details runat="server" ID="ProgramDetails" />

                <div runat="server" id="ConfirmMessage" class="alert alert-danger mt-4" role="alert">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this Program?

                    <div class="mt-2">
                        <asp:CheckBox runat="server" ID="DeleteConfirmationCheckbox" Text="Delete Program" />
                    </div>
                </div>	

                <div>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />	
                    <insite:CancelButton runat="server" ID="CancelButton" />	
                </div>
                
            </div>

            <div class="col-lg-6">

                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone. 
                    The program will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>


                <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                    <tr>
                        <td>
                            Program
                        </td>
                        <td>
                            1
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Learners
                        </td>
                        <td>
                            <asp:Literal ID="LearnersCount" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Tasks
                        </td>
                        <td>
                            <asp:Literal ID="TasksCount" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </table>

            </div>

        </div>

    </section>
    
        <insite:PageFooterContent runat="server">
            <script type="text/javascript">

                $(document).ready(function () {
                    $('#<%= DeleteButton.ClientID %>').click(function (e) {
                        if (typeof $(this).attr('disabled') !== 'undefined' && $(this).attr('disabled') !== false)
                            e.preventDefault();
                    });

                    $('#<%= DeleteConfirmationCheckbox.ClientID %>').change(function () {
                        if ($(this).is(':checked')) {
                            $('#<%= DeleteButton.ClientID %>')
                                .removeAttr('disabled')
                                .removeClass('disabled');
                        } else {
                            $('#<%= DeleteButton.ClientID %>')
                                .attr('disabled', 'disabled')
                                .addClass('disabled');
                        }
                    });

                    $('#<%= DeleteButton.ClientID %>')
                        .attr('disabled', 'disabled')
                        .addClass('disabled');
                });

            </script>
        </insite:PageFooterContent>
    
</asp:Content>