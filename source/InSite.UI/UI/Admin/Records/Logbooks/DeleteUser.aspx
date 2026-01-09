<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="DeleteUser.aspx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.DeleteUser" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <section class="pb-5 mb-md-2">

        <asp:Panel runat="server" ID="ErrorPanel" Visible="false">	
            <div class="alert alert-danger" role="alert">
                <i class="fas fa-stop-circle"></i>
                <asp:Literal runat="server" ID="ErrorText" />	
            </div>	
        </asp:Panel>
        
        <div class="row">                       

            <div class="col-lg-6">                                   
                
                <h3>User</h3>
                
                <uc:PersonDetail runat="server" ID="PersonDetail" />

                <dl class="row">
                    <dt class="col-sm-3">Logbook Name:</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="LogbookName" /></dd>

                    <dt class="col-sm-3">Logbook Title:</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="LogbookTitle" /></dd>
                </dl>

                <div class="alert alert-danger mt-4" role="alert">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to proceed?

                    <div class="mt-2">
                        <asp:CheckBox runat="server" ID="DeleteConfirmationCheckbox" Text="Delete Learner" />
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
                    The Logbook's learner will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                    <tr>
                        <td>
                            Learners
                        </td>
                        <td>
                            1
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Log Entries
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="EntryCount" />
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


