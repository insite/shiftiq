<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Portal.Events.Appointments.Search" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

    <style>
        .main-panel div.activity+div.summary {
            margin-top:30px;
        }
    </style>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <div runat="server" id="MainPanel" class="main-panel">

        <asp:Repeater runat="server" ID="EventRepeater">
            <ItemTemplate>

                <div class="mb-4">

                    <div class="card card-hover shadow mb-3">

                        <div class="card-header">
                            <h3 class="m-0 text-primary">
                                <%# Eval("EventTitle") %>
                                <span class="form-text">
                                    <%# Eval("AppointmentType") %>
                                </span>
                            </h3>
                        </div>

                        <div class="card-body">


                            <div class="row summary">
                                <div class="col-md-12">
                                    <small>
                                        <%# Eval("Summary") %>
                                    </small>
                                </div>
                            </div>

                            <div class="row activity">
                                <div class="col-md-6">
                                    <strong>
                                        <%# Eval("EventScheduledText") %>
                                    </strong>
                                    <div>
                                        <%# Eval("VenueLocationName") %>
                                    </div>
                                    <div>
                                        <%# Eval("VenueAddress") %>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div>
                                        <a class="me-2" href='<%# Eval("EventIdentifier", "/ui/portal/events/appointments/outline?event={0}") %>'><%# Eval("EventTitle") %></a>
                                    </div>
                                </div>
                            </div>

                        </div>

                    </div>

                </div>

            </ItemTemplate>
        </asp:Repeater>
        
    </div>

</asp:Content>
