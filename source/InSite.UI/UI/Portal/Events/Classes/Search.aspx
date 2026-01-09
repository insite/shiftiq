<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Portal.Events.Classes.Search" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        .main-panel div.activity+div.summary {
            margin-top:30px;
        }

        .summary {
            margin-bottom: 0.25rem;
        }
        .summary p {
            margin:0;
        }

        table.events th:nth-child(1) {
            width:20%;
        }
        table.events th:nth-child(2) {
            width:40%;
        }
        table.events th:nth-child(3) {
            width:20%;
        }
        table.events th:nth-child(4) {
            width:20%
        }

        .status .badge {
            display: block;
            width: fit-content;
        }
        .status .badge + .badge {
            margin-top: 0.25rem;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <div class="d-flex justify-content-between align-items-center mb-3">
        <div class="w-50">
            <insite:InputFilter runat="server" ID="SearchText" EmptyMessage="Search Classes" />
        </div>
        <div>
            <div>
                <insite:CheckSwitch runat="server" ID="HideFullWithNoWaitlist" Text="Hide Full Classes with no Waitlist"/>
            </div>
            <div>
                <insite:CheckSwitch runat="server" ID="HideFullClasses" Text="Hide Full Classes"/>
            </div>
        </div>
    </div>

    <div runat="server" id="MainPanel" class="main-panel">

        <asp:Repeater runat="server" ID="TypeRepeater">
            <ItemTemplate>

                <h2>
                    <%# Eval("Title") %>
                </h2>

                <asp:Repeater runat="server" ID="ResourceRepeater">
                    <ItemTemplate>

                        <div class="card card-hover shadow mb-3">

                            <div class="card-header">
                                <h3 class="m-0 text-primary"><%# Eval("Title") %></h3>
                            </div>

                            <div class="card-body">

                                <asp:Repeater runat="server" ID="SummaryRepeater">
                                    <ItemTemplate>

                                        <div class="row summary">
                                            <div class="col-md-12">
                                                <small>
                                                    <%# Eval("Summary") %>
                                                </small>
                                            </div>
                                        </div>

                                        <asp:Repeater runat="server" ID="ClassRepeater">
                                            <HeaderTemplate>
                                                <table class="table table-striped events">
                                                    <thead>
                                                        <tr>
                                                            <th>Event Date</th>
                                                            <th>Event</th>
                                                            <th>Venue</th>
                                                            <th>Status</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                            </HeaderTemplate>
                                            <FooterTemplate>
                                                    </tbody>
                                                </table>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <%# Eval("EventScheduledText") %>
                                                    </td>
                                                    <td>
                                                        <a
                                                            class="me-2"
                                                            href='<%# Eval("EventIdentifier", "/ui/portal/events/classes/outline?event={0}") %>'
                                                        >
                                                            <%# Eval("EventTitle") %>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <div>
                                                            <%# Eval("VenueLocationName") %>
                                                        </div>
                                                        <div>
                                                            <%# Eval("VenueAddress") %>
                                                        </div>
                                                    </td>
                                                    <td class="status">
                                                        <div runat="server" visible='<%# (bool)Eval("IsFull") && !(bool)Eval("IsLocked") %>' class="badge bg-danger"><insite:Literal runat="server" Text="Full" /></div>
                                                        <div runat="server" visible='<%# Eval("IsWaitlistAvailable") %>' class="badge bg-warning"><insite:Literal runat="server" Text="Waiting list is available" /></div>
                                                        <div runat="server" visible='<%# Eval("IsRegistered") %>' class="badge bg-success"><insite:Literal runat="server" Text="Registered" /></div>
                                                        <div runat="server" visible='<%# Eval("IsWaitlisted") %>' class="badge bg-info"><insite:Literal runat="server" Text="In Waiting List" /></div>
                                                        <div runat="server" visible='<%# Eval("IsInvited") %>' class="badge bg-info"><insite:Literal runat="server" Text="Invitation Sent" /></div>
                                                        <div runat="server" visible='<%# Eval("IsMoved") %>' class="badge bg-danger"><insite:Literal runat="server" Text="Moved" /></div>
                                                        <div runat="server" visible='<%# Eval("IsCancelled") %>' class="badge bg-danger"><insite:Literal runat="server" Text="Cancelled" /></div>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </div>

                        </div>

                    </ItemTemplate>
                </asp:Repeater>

            </ItemTemplate>
        </asp:Repeater>
        
    </div>

</asp:Content>