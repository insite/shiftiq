<%@ Page Language="C#" CodeBehind="DeleteUser.aspx.cs" Inherits="InSite.UI.Admin.Records.Validators.Forms.DeleteUser" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <div class="row settings">

        <div class="col-lg-6">                                   
                
            <h3>User</h3>
                
            <uc:PersonDetail runat="server" ID="PersonDetail" />

            <dl class="row">
                <dt class="col-sm-3">Logbook Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="LogbookName" /></dd>

                <dt class="col-sm-3">Logbook Title:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="LogbookTitle" /></dd>
            </dl>

            <div runat="server" id="ConfirmationPanel" class="alert alert-danger" role="alert" style="padding-bottom: 10px;">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Confirm:</strong> 

                Are you sure you want to proceed?

                <div style="margin-top:10px;">
                    <asp:CheckBox runat="server" ID="DeleteConfirmationCheckbox" Text="Delete Learner" />
                </div>	
            </div>
            <div runat="server" id="ErrorPanel" visible="false" class="alert alert-danger" role="alert" style="padding-bottom: 10px;">
                    <i class="fas fa-exclamation-triangle"></i>
                <strong>The learner cannot be deleted because has log entries in this logbook</strong> 
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
                The logbook's learner will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
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
