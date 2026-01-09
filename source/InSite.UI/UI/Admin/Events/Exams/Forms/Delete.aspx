<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Events.Exams.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Events/Classes/Controls/ClassVenueInfo.ascx" TagName="ClassVenueInfo" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />
            
        <div class="row settings">
            <div class="col-md-6">
                <h3>Exam</h3>

                <dl class="row" runat="server" id="FormatDiv">
                    <dt class="col-sm-3">Exam Format</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="EventFormat" />
                    </dd>
                </dl>

                <dl class="row" runat="server" id="SchedulingStatusDiv">
                    <dt class="col-sm-3">Scheduling Status</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="EventSchedulingStatus" />
                    </dd>
                </dl>

                <dl class="row" runat="server" id="ClassCodeDiv">
                    <dt class="col-sm-3">Class/Session Code</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="EventClassCode" />
                    </dd>
                </dl>

                <uc:ClassVenueInfo runat="server" ID="ClassVenueInfo" AddressDescription="" ShowChangeLink="false" />

                <dl class="row" runat="server" id="VenueRoomField">
                    <dt class="col-sm-3">Building and Room</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="VenueRoom" />
                    </dd>
                </dl>

                <dl class="row" runat="server" id="EventTimeDiv">
                    <dt  class="col-sm-3">Scheduled Start Date and Time</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="EventTime" />
                    </dd>
                </dl>

                <div class="alert alert-danger">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this exam event?
                </div>

                <div>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />	
                    <insite:CancelButton runat="server" ID="CancelButton" />
                </div>
            </div>

            <div class="col-md-6">
                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone. 
                    The event will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                    <tr>
                        <td>
                            Exam Event
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
                            <asp:Literal runat="server" ID="RegistrationCount" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>

</div>
</asp:Content>
