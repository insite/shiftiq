<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Portal.Events.Exams.Outline" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="../Classes/Controls/Venue.ascx" TagName="Venue" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <div runat="server" id="MainPanel">

        <div class="card card-hover shadow mb-3">
            <div class="card-header"><insite:Literal runat="server" Text="Date, Time &amp; Location" /></div>
            <div class="card-body pt-2">

                <h5 class="mt-2 mb-1"><insite:Literal runat="server" Text="Date" /></h5>
                <div><asp:Literal runat="server" ID="Date" /></div>

                <uc:Venue runat="server" ID="Venue" />

            </div>
        </div>

    </div>

    <div class="mt-3">
        <insite:Button runat="server" ID="ReturnButton" ButtonStyle="Primary" Text="Return to Exams Search" Icon="fas fa-calendar-alt" NavigateUrl="/ui/portal/events/exams/search" />
        <insite:Button runat="server" ID="ReturnToCalendar" ButtonStyle="Primary" Text="Return to Calendar" Icon="fas fa-calendar-alt" />
    </div>

</asp:Content>