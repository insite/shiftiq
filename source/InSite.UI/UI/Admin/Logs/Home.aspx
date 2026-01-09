<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Logs.Home" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="HomeStatus" />

    <section class="pb-5 mb-md-2">
        <h2 class="h4 mb-3">Timeline Logs</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-body"> 
                <div class="row row-cols-1 row-cols-lg-4 g-4">
                    <div class="col" runat="server" id="CommandCounter">
                        <a runat="server" id="CommandLink" class="card card-hover card-tile border-0 shadow" href="#">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="CommandCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-terminal fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Commands</h3>
                            </div>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="pb-5 mb-md-2">
        <h2 class="h4 mb-3">Timeline Aggregates</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <asp:Repeater runat="server" ID="AggregateRepeater">
                    <ItemTemplate>
                        
                        <h3 class="h5"><%# Eval("Name") %></h3>

                        <div class="row pb-4 mb-4 border-bottom">
                            <div class="col-lg-6">
                                <strong>Commands</strong>
                                <div>
                                    <asp:Repeater runat="server" ID="CommandRepeater">
                                        <ItemTemplate>
                                            <span class="fs-sm text-muted p-2">
                                                <%# Eval("Type.Name") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                            <div class="col-lg-6">
                                <strong>Events (Changes)</strong>
                                <div>
                                    <asp:Repeater runat="server" ID="ChangeRepeater">
                                        <ItemTemplate>
                                            <span class="fs-sm text-muted p-2">
                                                <%# Eval("Type.Name") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>

                    </ItemTemplate>
                </asp:Repeater>

                <asp:Panel runat="server" ID="WarningPanel">
                    <h3 class="h5">Unregistered Commands</h3>
                    <div class="row pb-4 mb-4 border-bottom">
                        <div class="col-lg-12">
                            <asp:Repeater runat="server" ID="UnregisteredCommandRepeater">
                                <ItemTemplate>
                                    <span class="fs-sm text-danger p-2"><%# Eval("Type.Name") %></span>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </asp:Panel>

            </div>
        </div>
    </section>

    

</asp:Content>