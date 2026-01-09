<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Events.Appointments.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        li.disabled a span.text { color: silver; }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />
    <insite:ValidationSummary runat="server" ValidationGroup="Appointment" />

    <asp:Panel runat="server" ID="NewSection">

        <div class="row mb-3">
            <div class="col-lg-6">
                <div class="form-group mb-3">
                    <label class="form-label">
                        Schedule
                    </label>
                    <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Appointment" />
                </div>
            </div>
        </div>

        <div class="row mb-3">

            <div class="col-lg-6">

                <div class="card">

                    <div class="card-body">

                        <h4 class="card-title mb-3">Appointment</h4>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Title
                                <insite:RequiredValidator runat="server" ControlToValidate="EventTitle" FieldName="Appointment Title" ValidationGroup="Appointment" />
                            </label>
                            <insite:TextBox runat="server" ID="EventTitle" MaxLength="400" />
                            <div class="form-text">The descriptive title for this appointment.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Appointment Type
                            </label>
                            <insite:AppointmentTypeComboBox runat="server" ID="AppointmentType" />
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Calendar Color</label>
                            <div>
                                <insite:ColorComboBox runat="server" ID="CalendarColor" AllowBlank="false" Value="Info" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Start Date and Time
                                <insite:RequiredValidator runat="server" ControlToValidate="EventScheduledStart" FieldName="Start" ValidationGroup="Appointment" />
                            </label>
                            <div>
                                <insite:DateTimeOffsetSelector ID="EventScheduledStart" runat="server" />
                            </div>
                            <div class="form-text">The start date and time for this appointment event.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                End Date and Time
                                <insite:RequiredValidator runat="server" ControlToValidate="EventScheduledEnd" FieldName="End" ValidationGroup="Appointment" />
                                <insite:CustomValidator runat="server" ID="EventScheduledEndValidator" ControlToValidate="EventScheduledEnd" ValidationGroup="Appointment" />
                            </label>
                            <div>
                                <insite:DateTimeOffsetSelector ID="EventScheduledEnd" runat="server" />
                            </div>
                            <div class="form-text">The end date and time for this appointment event.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Description
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="AppointmentDescription" TextMode="MultiLine" Rows="5" />
                            </div>
                            <div class="form-text">Short description for this appointment.</div>
                        </div>

                    </div>
            
                </div>

            </div>

        </div>
        
        <div class="row">
            <div class="col-lg-6">
                <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Appointment" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>
        </div>

    </asp:Panel>

</asp:Content>
