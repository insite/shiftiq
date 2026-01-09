<%@ Page Language="C#" CodeBehind="DeleteCompetency.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.DeleteCompetency" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <div class="row settings">
        <div class="col-lg-6">
                                    
            <h3>Logbook's Competency</h3>

            <dl class="row">
                <dt class="col-sm-3">Name</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Name" /></dd>

                <dt class="col-sm-3"><insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/></dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Hours" /></dd>

                <dt class="col-sm-3">Number of Log Entries</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="JournalItems" /></dd>

                <dt class="col-sm-3">Skill Rating</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="SkillRating" /></dd>

                <dt class="col-sm-3">Logbook Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="LogbookName" /></dd>

                <dt class="col-sm-3">Logbook Title:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="LogbookTitle" /></dd>
            </dl>

            <div runat="server" id="DeleteConfirmationPanel" class="alert alert-danger" role="alert" style="padding-bottom: 10px;">
        
                <i class="fas fa-exclamation-triangle"></i> <strong>Confirm:</strong> 

                Are you sure you want to proceed?

                <div style="margin-top:10px;">
                    <asp:CheckBox runat="server" ID="DeleteConfirmationCheckbox" Text="Delete Competency" />
                </div>

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
                The logbook's competency will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Competencies
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
                        <asp:Literal runat="server" ID="DeleteJournalItems" />
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
