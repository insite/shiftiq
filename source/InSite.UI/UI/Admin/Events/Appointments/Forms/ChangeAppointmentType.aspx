<%@ Page Language="C#" CodeBehind="ChangeAppointmentType.aspx.cs" Inherits="InSite.UI.Admin.Events.Appointments.Forms.ChangeAppointmentType" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />

    <section runat="server" ID="RescheduleSection" class="mb-3">

        <h2 class="h4 mb-3">
            Change
        </h2>

        <div class="row">
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <div class="form-group mb-3">
                            <label class="form-label">Event Title</label>
                            <div>
                                <asp:Literal runat="server" ID="EventTitle" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Appointment Type</label>
                            <div>
                                <insite:AppointmentTypeComboBox runat="server" ID="AppointmentType" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Calendar Color</label>
                            <div>
                                <insite:ColorComboBox runat="server" ID="AppointmentCalendarColor" AllowBlank="false" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </section>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Appointment" />
    <insite:CancelButton runat="server" ID="BackButton" />
</asp:Content>