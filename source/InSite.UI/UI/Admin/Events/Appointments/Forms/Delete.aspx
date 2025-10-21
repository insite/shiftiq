<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Events.Appointments.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="../Controls/AppointmentInfo.ascx" TagName="AppointmentInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

    <style type="text/css">
        span.smart-checkbox { display: table; }
        span.smart-checkbox > input { display: table-cell; }
        span.smart-checkbox > label { display: table-cell; vertical-align: top; padding-left: 4px; }
    </style>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />
    
    <div class="row mb-3">
        <div class="col-lg-12">
            <insite:DeleteConfirmAlert runat="server" Name="Appointment" />
        </div>
    </div>

    <div class="row">

        <div class="col-lg-6">

            <uc:AppointmentInfo runat="server" ID="AppointmentDetails" />

            <div class="card shadow mt-4">
                <div class="card-body text-danger">
                    <h5 class="card-title">Action Required</h5>
                    <div class="card-text">
                        <asp:CheckBox runat="server" ID="DeleteCheck" CssClass="smart-checkbox" Text="Yes, I understand the consequences, delete this appointment." />
                    </div>
                </div>
            </div>

            <div class="mt-4">	
                <insite:DeleteButton runat="server" ID="DeleteButton" Enabled="false" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>

        </div>

        <div class="col-md-6">
            <div class="card shadow">
                <div class="card-body">

                    <h5 class="card-title">Consequences</h5>

                    <insite:DeleteWarningAlert runat="server" Name="Appointment" />

                    <table class="table table-striped table-bordered table-metrics">
                        <tr>
                            <th>Data</th>
                            <th class="text-end" style="width:80px;">Items</th>
                        </tr>
                        <tr>
                            <td>Appointments</td>
                            <td class="text-end"><asp:Literal runat="server" ID="AppointmentsCount" Text="1"/></td>
                        </tr>
                        <tr>
                            <td>Registrations</td>
                            <td class="text-end"><asp:Literal runat="server" ID="RegistrationsCount" Text="0"/></td>
                        </tr>
                    </table>

                </div>
            </div>
        </div>

    </div>

</asp:Content>
