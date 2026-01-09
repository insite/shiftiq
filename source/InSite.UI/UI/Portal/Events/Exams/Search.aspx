<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Portal.Events.Exams.Search" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

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

        <asp:Repeater runat="server" ID="DataRepeater">
            <ItemTemplate>

                <h2>
                    <%# Eval("Title") %>
                </h2>

                <div class="mb-4">
                    <asp:Repeater runat="server" ID="ExamFormsRepeater">
                        <ItemTemplate>

                            <div class="card card-hover shadow mb-3">

                                <div class="card-header">
                                    <h3 class="m-0 text-primary">
                                        <insite:Literal runat="server" Text="Exam Format" />:
                                        <%# Translate((string)Eval("Title")) %>
                                    </h3>
                                </div>

                                <div class="card-body">

                                    <asp:Repeater runat="server" ID="ExamTypeRepeater">
                                        <ItemTemplate>

                                            <div class="row summary">
                                                <div class="col-md-12">
                                                    <small>
                                                        <insite:Literal runat="server" Text="Exam Type" />:
                                                        <%# Translate((string)Eval("Title")) %>
                                                    </small>
                                                </div>
                                            </div>

                                            <asp:Repeater runat="server" ID="ExamRepeater">
                                                <ItemTemplate>
                                                    <div class="row activity">
                                                        <div class="col-md-12 mt-2 mb-2">
                                                            <%# Eval("Forms") %>
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
                                                                <a class="me-2" href='<%# Eval("EventIdentifier", "/ui/portal/events/exams/outline?event={0}") %>'><%# Eval("EventTitle") %></a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </div>

                            </div>

                        </ItemTemplate>
                    </asp:Repeater>
                </div>

            </ItemTemplate>
        </asp:Repeater>
        
    </div>

</asp:Content>