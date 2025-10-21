<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Records.Achievements.Delete" %>
<%@ Register Src="Controls/AchievementDetails.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="Status" />

    <section class="pb-5 mb-md-2">

        <asp:Panel runat="server" ID="ErrorPanel" Visible="false">	
            <div class="alert alert-danger" role="alert">
                <i class="fas fa-stop-circle"></i>
                <asp:Literal runat="server" ID="ErrorText" />	
            </div>	
        </asp:Panel>
        
        <div class="row">                       

            <div class="col-lg-6">                                   
                
                <h2 class="h4 mb-3">Achievement</h2>
                
                <uc:Details runat="server" ID="AchievementDetails" />

                <div runat="server" id="ConfirmMessage" class="alert alert-danger mt-4" role="alert">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this achievement?	
                </div>	

                <div runat="server" id="DeleteError" class="alert alert-danger" role="alert">
        
                    <i class="fas fa-exclamation-triangle"></i> <strong>This achievement can't be removed.</strong>
        
                    <asp:Literal runat="server" ID="LockedMessage" Text="This achievement is locked. You must unlock this achievement before deleting it." />
                    <asp:Literal runat="server" ID="GradebookUsage" />

                </div>
                <div runat="server" id="DeleteConfirmationPanel" class="alert alert-danger" role="alert" style="padding-bottom: 10px;">
        
                    <i class="fas fa-exclamation-triangle"></i> <strong>Confirm:</strong> 

                    <asp:Literal runat="server" ID="CredentialUsage" />

                    Are you sure you want to proceed?

                    <div style="margin-top:10px;">
                        <asp:CheckBox runat="server" ID="DeleteConfirmationCheckbox" Text="Delete Achievement" />
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
                    The achievement will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                    <tr>
                        <td>
                            Achievement
                        </td>
                        <td>
                            1
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Events
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="EventCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Credentials
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="CredentialCount" />
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