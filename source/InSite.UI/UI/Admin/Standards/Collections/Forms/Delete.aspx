<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Standards.Collections.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <div class="row settings">
        <div class="col-md-6">
            <h3>Collection</h3>
                    
            <dl class="row">
                <dt class="col-sm-3">Title</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="TitleOutput" /></dd>
                
                <dt class="col-sm-3">Tag</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Label" /></dd>
            </dl>

            <div runat="server" id="DeleteConfirmationPanel" class="alert alert-danger" role="alert" style="padding-bottom: 10px;">
        
                <i class="fas fa-exclamation-triangle"></i> <strong>Confirm:</strong> 

                Are you sure you want to proceed?

                <div style="margin-top:10px;">
                    <asp:CheckBox runat="server" ID="DeleteConfirmationCheckbox" Text="Delete Collection" />
                </div>

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
                This is a permanent change that cannot be undone. 
                The collection will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Collections
                    </td>
                    <td>
                        1
                    </td>
                </tr>

                <tr>
                    <td>
                        Competencies
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="CompetenciesCount" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        $(document).ready(function () {
            $('#<%= DeleteButton.ClientID %>').click(function (e) {
                if (typeof $(this).attr('disabled') !== 'undefined' && $(this).attr('disabled') !== false)
                    e.preventDefault();
            });

            $('#<%= DeleteConfirmationCheckbox.ClientID %>').change(function () {
                if($(this).is(':checked'))
                    $('#<%= DeleteButton.ClientID %>').removeAttr('disabled');
                else
                    $('#<%= DeleteButton.ClientID %>').attr('disabled', 'disabled');
            });

            $('#<%= DeleteButton.ClientID %>').attr('disabled', 'disabled');
        });

    </script>
</insite:PageFooterContent>
</asp:Content>
