<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Events.Seats.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <div runat="server" id="NoDelete" class="alert alert-danger" role="alert">
        <i class="fas fa-stop-circle"></i> This seat cannot be deleted until all registration referenced by it will be deleted.
    </div>
            
    <div class="row settings">
        <div class="col-md-6">

            <h3>Seat</h3>

            <dl class="row">
                <dt class="col-sm-3">Seat Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="SeatName" /></dd>

                <dt class="col-sm-3">Seat Description:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="SeatDescription" /></dd>

                <dt class="col-sm-3">Seat Agreement:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="SeatAgreement" /></dd>

                <dt class="col-sm-3">Event Title:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="EventTitle" /></dd>
            </dl>

            <div runat="server" id="AlertPanel" class="alert alert-danger" role="alert">
                <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong> Are you sure you want to proceed?  
                <div>
                    <asp:CheckBox runat="server" ID="DeleteConfirmationCheckbox" Text="Delete Seat and Registrations" />
                </div>
            </div>	

            <p style="padding-bottom:10px;">	
                <insite:DeleteButton runat="server" ID="DeleteButton" />	
                <insite:CancelButton runat="server" ID="CancelButton" />	
            </p>
        </div>

        <div class="col-md-6">

            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The seat will be deleted from all forms, queries, and reports.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Seat
                    </td>
                    <td>
                        1
                    </td>
                </tr>
                <tr>
                    <td>
                        Registrations
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="RegistrationsCount" />
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
                if ($(this).is(':checked'))
                    $('#<%= DeleteButton.ClientID %>').removeAttr('disabled');
                else
                    $('#<%= DeleteButton.ClientID %>').attr('disabled', 'disabled');
            });

            $('#<%= DeleteButton.ClientID %>').attr('disabled', 'disabled');
        });

    </script>
</insite:PageFooterContent>

</asp:Content>
