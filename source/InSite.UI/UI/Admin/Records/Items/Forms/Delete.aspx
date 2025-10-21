<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Records.Items.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />
    <div runat="server" id="NoDelete" class="alert alert-danger" role="alert">
        <i class="fas fa-stop-circle"></i> This gradebook item cannot be deleted until all children and scores are deleted from the gradebook.
    </div>
            
    <div class="row settings">

        <div class="col-lg-6">

            <h3>Item</h3>

            <dl class="row">
                <dt class="col-sm-3">Code</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ItemCode" /></dd>
            
                <dt class="col-sm-3">Name</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ItemName" /></dd>
            
                <dt class="col-sm-3">Parent Item</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ParentItemName" /></dd>
            
                <dt class="col-sm-3">Achievement</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="AchievementName" /></dd>

                <dt class="col-sm-3">Gradebook</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="GradebookName" /></dd>
            </dl>

            <div runat="server" id="DeleteWarning" class="alert alert-danger" role="alert">
                <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong> Are you sure you want to proceed?  
                <div>
                    <asp:CheckBox runat="server" ID="DeleteChildrenCheckbox" Text="Delete Item with all its children" />
                </div>
                <div runat="server" id="ScoresPanel">
                    <asp:CheckBox runat="server" ID="DeleteScoresCheckBox" Text="Delete Scores for the Item" />
                </div>
            </div>

            <p style="padding-bottom:10px;">	
                <insite:DeleteButton runat="server" ID="DeleteButton" />	
                <insite:CancelButton runat="server" ID="CancelButton" />	
            </p>	
        </div>

        <div class="col-lg-6">

            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The gradebook item will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Score Item
                    </td>
                    <td>
                        1
                    </td>
                </tr>
                <tr>
                    <td>
                        Children
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="ChildrenCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Scores
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="ScoresCount" Text="0" />
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

            $('#<%= DeleteChildrenCheckbox.ClientID %>').change(function () {

                if ($(this).is(':checked')
                    && (
                        $('#<%= DeleteScoresCheckBox.ClientID %>').is(':checked')
                        || !$('#<%= DeleteScoresCheckBox.ClientID %>').is(':visible')
                    )
                ) {
                    $('#<%= DeleteButton.ClientID %>').removeAttr('disabled');
                }
                else
                    $('#<%= DeleteButton.ClientID %>').attr('disabled', 'disabled');
            });
            $('#<%= DeleteScoresCheckBox.ClientID %>').change(function () {
                if (($(this).is(':checked'))&& ($('#<%= DeleteChildrenCheckbox.ClientID %>').is(':checked')))
                    $('#<%= DeleteButton.ClientID %>').removeAttr('disabled');
                else
                    $('#<%= DeleteButton.ClientID %>').attr('disabled', 'disabled');
            });

            $('#<%= DeleteButton.ClientID %>').attr('disabled', 'disabled');
        });

    </script>
</insite:PageFooterContent>

</asp:Content>
