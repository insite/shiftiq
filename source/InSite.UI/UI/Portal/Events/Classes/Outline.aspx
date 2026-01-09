<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Portal.Events.Classes.Outline" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="Controls/Venue.ascx" TagName="Venue" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <div runat="server" id="AlertRegistered" class="alert alert-success" role="alert">
        <i class="far fa-check-circle fs-xl"></i> <insite:Literal runat="server" Text="Registration Complete" />
    </div>

    <div runat="server" id="AlertInvited" class="alert alert-warning" role="alert">
        <i class="far fa-exclamation-triangle fs-xl"></i> <strong><insite:Literal runat="server" Text="Warning:" /></strong>
        <insite:Literal runat="server" Text="Invitation for registration has been sent." />
    </div>

    <div runat="server" id="AlertInvitationExpired" class="alert alert-warning" role="alert">
        <i class="far fa-exclamation-triangle fs-xl"></i> <strong><insite:Literal runat="server" Text="Warning:" /></strong>
        <insite:Literal runat="server" Text="Your invitation for registration is expired." />
    </div>

    <div runat="server" id="AlertWaiting" class="alert alert-success" role="alert">
        <i class="far fa-check-circle fs-xl"></i> <strong><insite:Literal runat="server" Text="Confirmed:" /></strong>
        <insite:Literal runat="server" Text="You are currently waitlisted for this class." />
    </div>

    <div runat="server" id="AlertMoved" class="alert alert-warning" role="alert">
        <i class="far fa-exclamation-triangle fs-xl"></i> <strong><insite:Literal runat="server" Text="Warning:" /></strong>
        <insite:Literal runat="server" Text="You were moved from this class." />
    </div>

    <div runat="server" id="AlertCancelled" class="alert alert-warning" role="alert">
        <i class="far fa-exclamation-triangle fs-xl"></i> <strong><insite:Literal runat="server" Text="Warning:" /></strong>
        <insite:Literal runat="server" Text="Your registration was cancelled." />
    </div>

    <div runat="server" id="AlertFull" class="alert alert-warning" role="alert">
        <i class="far fa-exclamation-triangle fs-xl"></i> <strong><insite:Literal runat="server" Text="Warning:" /></strong>
        <insite:Literal runat="server" Text="No seats are available." />
        <insite:Literal runat="server" ID="WaitlistNote" Text="You may add your name to the waiting list, and we'll contact you if a seat opens up." />
    </div>

    <div runat="server" id="AlertClosed" class="alert alert-warning" role="alert">
        <i class="far fa-exclamation-triangle fs-xl"></i> <strong><insite:Literal runat="server" Text="Warning:" /></strong>
        <p>
            <strong><insite:Literal runat="server" Text="Online Registration is Now Closed." /></strong>
        </p>
        <p><asp:Literal runat="server" ID="ClosedDetails" /></p>
    </div>

    <div runat="server" id="RegistrationCompetedCard" visible="false" class="card card-hover shadow mb-3">
        <div class="card-body">
            <asp:Literal runat="server" ID="RegistrationCompeted" />
        </div>
    </div>

    <div class="row mb-3">
        <div class="col-md-12">
            <div class="float-end">
                <insite:Button runat="server" ID="RegisterEmployeeLink" ButtonStyle="Success" Icon="fa fa-users-medical" Text="Register Employees" />
                <insite:Button runat="server" ID="AddEmployeeToWaitingListLink" ButtonStyle="Default" Icon="fa fa-users-medical" Text="Add Employee to Waiting List" />
                <insite:Button runat="server" ID="RegisterButton" ButtonStyle="Success" Icon="fa fa-user-plus" Text="Self-Register" />
                <insite:Button runat="server" ID="AddToWaitingListButton" ButtonStyle="Default" Icon="fa fa-user-plus" Text="Add to Waiting List" />
                <insite:Button runat="server" ID="PrintReceiptButton" ButtonStyle="Default" Icon="fa fa-print" Text="Print Receipt" />
            </div>
        </div>
    </div>

    <div runat="server" id="MainPanel">

        <div runat="server" id="SummaryPanel" class="card card-hover shadow mb-3">
            <div class="card-header h2 text-primary"><insite:Literal runat="server" Text="Summary" /></div>
            <div class="card-body pt-2">
                <asp:Literal runat="server" ID="Summary" />
            </div>
        </div>

        <div class="card card-hover shadow mb-3">
            <div class="card-header h2 text-primary"><insite:Literal runat="server" Text="Date, Time &amp; Location" /></div>
            <div class="card-body pt-2">

                <h5 class="mt-2 mb-1"><insite:Literal runat="server" Text="Date" /></h5>
                <div><asp:Literal runat="server" ID="Date" /></div>

                <uc:Venue runat="server" ID="Venue" />

                <div runat="server" id="RegistrationStartField">
                    <h5 class="mt-2 mb-1"><insite:Literal runat="server" Text="Registration Start" /></h5>
                    <div>
                        <asp:Literal runat="server" ID="RegistrationStart" />
                    </div>
                </div>

                <h5 class="mt-2 mb-1"><insite:Literal runat="server" Text="Registration Deadline" /></h5>
                <div>
                    <asp:Literal runat="server" ID="RegistrationDeadline" />
                </div>

                <div class="mt-3">
                    <asp:Literal runat="server" ID="AddToOfficeLink" />
                    <asp:Literal runat="server" ID="AddToGoogleLink" />
                    <asp:Literal runat="server" ID="DownloadIcsLink" />
                </div>

            </div>
        </div>

        <div runat="server" id="DescriptionPanel" class="card card-hover shadow mb-3">
            <div class="card-header h2 text-primary"><insite:Literal runat="server" Text="Description" /></div>
            <div class="card-body pt-2">
                <asp:Literal runat="server" ID="Description" />
            </div>
        </div>

        <div runat="server" id="MaterialsForParticipationCard" class="card card-hover shadow mb-3">
            <div class="card-header h2 text-primary"><insite:Literal runat="server" Text="Materials for Participation" /></div>
            <div class="card-body pt-2">
                <asp:Literal runat="server" ID="MaterialsForParticipation" />
            </div>
        </div>

        <div runat="server" id="ContactPanel" class="card card-hover shadow mb-3">
            <div class="card-header h2 text-primary"><insite:Literal runat="server" Text="Contact &amp; Support" /></div>
            <div class="card-body pt-2">
                <asp:Literal runat="server" ID="ContactInstruction" />
            </div>
        </div>

        <div runat="server" id="PricesPanel" class="card card-hover shadow mb-3">
            <div class="card-header h2 text-primary"><insite:Literal runat="server" Text="Prices" /></div>
            <div class="card-body pt-2">
                <asp:Repeater runat="server" ID="SeatRepeater">
                    <HeaderTemplate>
                        <div class="form-group">
                            <div>
                                <div class="pb-2">
                                    <small class="text-body-secondary"><insite:Literal runat="server" Text="All prices in Canadian dollars" /></small>
                                </div>

                                <table class="table table-striped">
                                    <tbody>
                                        <tr>
                                            <th runat="server" visible='<%# ShowFirstPriceColumn %>'><insite:Literal runat="server" Text="Class Registration" /></th>
                                            <th style="text-align:right;"><insite:Literal runat="server" Text="Tuition" /></th>
                                        </tr>
                    </HeaderTemplate>
                    <FooterTemplate>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr>
                            <td runat="server" visible='<%# ShowFirstPriceColumn %>'>
                                <b><%# Eval("SeatTitle") %></b>
                                <p><%# GetDescription(Container.DataItem) %></p>
                            </td>
                            <td style="text-align:right;">
                                <b><insite:Literal ID="FreePrice" runat="server" Text="Free" /></b>
                                <b><asp:Literal runat="server" ID="SinglePrice" /></b>

                                <asp:Repeater runat="server" ID="MultiplePrice">
                                    <ItemTemplate>
                                        <div style="clear: both;">
                                            <p style="float:left;"><%# Eval("Name") %>: </p>
                                            <p style="float:right;"><b><%# Eval("Amount", "{0:c2}") %></b></p>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div runat="server" id="AccommodationsPanel" class="card card-hover shadow mb-3">
            <div class="card-header h2 text-primary"><insite:Literal runat="server" Text="Travel Accommodations" /></div>
            <div class="card-body pt-2">
                <asp:Literal runat="server" ID="AccommodationInstruction" />
            </div>
        </div>

        <div runat="server" id="AdditionalInstructionPanel" class="card card-hover shadow mb-3">
            <div class="card-header h2 text-primary"><insite:Literal runat="server" Text="Additional Information" /></div>
            <div class="card-body pt-2">
                <asp:Literal runat="server" ID="AdditionalInstruction" />
            </div>
        </div>

        <div runat="server" id="CancellationInstructionPanel" class="card card-hover shadow mb-3">
            <div class="card-header h2 text-primary"><insite:Literal runat="server" Text="Cancellation &amp; Refund Policy" /></div>
            <div class="card-body pt-2">
                <asp:Literal runat="server" ID="CancellationInstruction" />
            </div>
        </div>

    </div>

    <div class="mt-3">
        <insite:Button runat="server" ID="ReturnButton" ButtonStyle="Primary" Text="Return to Classes Search" Icon="fas fa-calendar-alt" NavigateUrl="/ui/portal/events/classes/search" />
        <insite:Button runat="server" ID="ReturnToCalendar" ButtonStyle="Primary" Text="Return to Calendar" Icon="fas fa-calendar-alt" />
    </div>

</asp:Content>