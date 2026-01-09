<%@ Page Language="C#" CodeBehind="Reschedule.aspx.cs" Inherits="InSite.Admin.Events.Appointments.Forms.Reschedule" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>


<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Appointment" />

    <section runat="server" ID="RescheduleSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-calendar-alt me-1"></i>
            Reschedule Appointment
        </h2>


        <div class="row settings">
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Identification</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Event Title</label>
                            <div>
                                <asp:Literal runat="server" ID="EventTitle" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Schedule Information</h3>
                
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Start Date and Time
                                <insite:RequiredValidator runat="server" ControlToValidate="EventScheduledStart" FieldName="Start" ValidationGroup="Appointment" />
                            </label>
                            <div>
                                <insite:DateTimeOffsetSelector ID="EventScheduledStart" runat="server" Width="400" />
                            </div>
                            <div class="form-text">The start date and time for this appointment event.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                End Date and Time
                                <insite:RequiredValidator runat="server" ControlToValidate="EventScheduledEnd" FieldName="End" ValidationGroup="Appointment" />
                                <insite:CustomValidator runat="server" ID="EventScheduledEndValidator" ControlToValidate="EventScheduledEnd" ValidationGroup="Appointment" Display="None" />
                            </label>
                            <div>
                                <insite:DateTimeOffsetSelector ID="EventScheduledEnd" runat="server" Width="400" />
                            </div>
                            <div class="form-text">The end date and time for this appointment event.</div>
                        </div>

                    </div>
                </div>
            </div>

        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Appointment" />
            <insite:CancelButton runat="server" ID="BackButton" />
        </div>
    </div>

</div>
</asp:Content>
