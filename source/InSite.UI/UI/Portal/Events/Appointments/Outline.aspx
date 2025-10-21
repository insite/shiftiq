<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Portal.Events.Appointments.Outline" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <div runat="server" id="MainPanel">

        <div runat="server" id="DescriptionPanel" class="card card-hover shadow mb-3">
            <div class="card-header"><insite:Literal runat="server" Text="Description" /></div>
            <div class="card-body pt-2">
                <asp:Literal runat="server" ID="Description" />
            </div>
        </div>

        <div class="card card-hover shadow mb-3">
            <div class="card-header"><insite:Literal runat="server" Text="Date &amp; Time" /></div>
            <div class="card-body pt-2">

                <h5 class="mt-2 mb-1"><insite:Literal runat="server" Text="Date" /></h5>
                <div><asp:Literal runat="server" ID="Date" /></div>
            </div>
        </div>

    </div>

    <div class="mt-3">
        <insite:Button runat="server" ID="ReturnButton" ButtonStyle="Primary" Text="Return to Appointment Search" Icon="fas fa-calendar-alt" NavigateUrl="/ui/portal/events/appointments/search" />
        <insite:Button runat="server" ID="ReturnToCalendar" ButtonStyle="Primary" Text="Return to Calendar" Icon="fas fa-calendar-alt" />
    </div>

</asp:Content>