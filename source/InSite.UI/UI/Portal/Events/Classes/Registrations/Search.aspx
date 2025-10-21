<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Portal.Events.Classes.Registrations.Search" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <asp:Panel runat="server" ID="CommandsPanel" CssClass="mb-3">
        <insite:Button runat="server" ID="ShowPastEventsButton" ButtonStyle="OutlineSecondary" Text="Show Past Events" />
        <insite:Button runat="server" ID="ShowCurrentEventsButton" ButtonStyle="OutlineSecondary" Text="Show Current Events" />
    </asp:Panel>

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Accordion runat="server" ID="MainAccordion" IsFlush="false">
        <insite:AccordionPanel runat="server" ID="MyRegistrationsPanel" Icon="far fa-user" Title="My Registrations">

            <div class="row">
                <div class="col-md-12">
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="MyRegistrationsUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="MyRegistrationsUpdatePanel">
                        <ContentTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Event Date</th>
                                        <th>Event</th>
                                        <th>Date Booked</th>
                                        <th>Booked By</th>
                                        <th>Approval</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater runat="server" ID="MyRegistrationsRepeater">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# GetLocalTime("EventStartDate") %></td>
                                                <td><%# Eval("EventTitle") %></td>
                                                <td><%# GetLocalDate("RegistrationRequestedOn") %></td>
                                                <td><%# Eval("RegistrationRequestedBy") %></td>
                                                <td><%# Eval("ApprovalStatus") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                                <insite:Container runat="server" ID="MyRegistrationsFooter">
                                    <tfoot>
                                        <tr>
                                            <td colspan="6">
                                                <insite:Pagination runat="server" ID="MyRegistrationsPagination" />
                                            </td>
                                        </tr>
                                    </tfoot>
                                </insite:Container>
                            </table>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>
            </div>
        </insite:AccordionPanel>

        <insite:AccordionPanel runat="server" ID="OtherRegistrationsPanel" Icon="far fa-users" Title="Registered Attendees">
            <div class="row">
                <div class="col-md-12">
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OtherRegistrationsUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="OtherRegistrationsUpdatePanel">
                        <ContentTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Event Date</th>
                                        <th>Event</th>
                                        <th>Date Booked</th>
                                        <th>Attendee</th>
                                        <th>Approval</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater runat="server" ID="OtherRegistrationsRepeater">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# GetLocalTime("EventStartDate") %></td>
                                                <td><%# Eval("EventTitle") %></td>
                                                <td><%# GetLocalDate("RegistrationRequestedOn") %></td>
                                                <td><%# Eval("CandidateName") %></td>
                                                <td><%# Eval("ApprovalStatus") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                                <insite:Container runat="server" ID="OtherRegistrationsFooter">
                                    <tfoot>
                                        <tr>
                                            <td colspan="6">
                                                <insite:Pagination runat="server" ID="OtherRegistrationsPagination" />
                                            </td>
                                        </tr>
                                    </tfoot>
                                </insite:Container>
                            </table>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>
            </div>
        </insite:AccordionPanel>
    </insite:Accordion>

</asp:Content>
