<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppointmentInfo.ascx.cs" Inherits="InSite.Admin.Events.Appointments.Controls.AppointmentInfo" %>

<div class="card shadow">
    <div class="card-body">

        <h5 class="card-title">Appointment</h5>
        <dl class="row mb-0">

            <dt class="col-sm-3">Title</dt>
            <dd class="col-sm-9"><asp:Literal runat="server" ID="EventTitle" /></dd>

            <dt class="col-sm-3">Appointment Type</dt>
            <dd class="col-sm-9"><asp:Literal runat="server" ID="AppointmentType" /></dd>

            <dt runat="server" id="EventScheduledStartLabel" class="col-sm-3">Start Time</dt>
            <dd runat="server" id="EventScheduledStartValue" class="col-sm-9">
                <asp:Literal runat="server" ID="EventScheduledStart" />
            </dd>

            <dt runat="server" id="EventScheduledEndLabel" class="col-sm-3">End Time</dt>
            <dd runat="server" id="EventScheduledEndValue" class="col-sm-9">
                <asp:Literal runat="server" ID="EventScheduledEnd" />
            </dd>

        </dl>

    </div>
</div>