<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Messages.Dashboard" %>
<%@ Import Namespace="InSite.Common.Web.Infrastructure" %>

<%@ Register Src="./Mailouts/Controls/CompletedGrid.ascx" TagName="CompletedGrid" TagPrefix="uc" %>
<%@ Register Src="./Mailouts/Controls/ScheduledGrid.ascx" TagName="ScheduledGrid" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Messages</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">      

                    <asp:Repeater runat="server" ID="WidgetRepeater">
	                    <ItemTemplate>
                            <div class="col">
                                <a class="card card-hover card-tile border-0 shadow" href="<%# GetSearchUrl((string)Eval("Name")) %>">
                                    <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="Count" /><%# Eval("Value", "{0:n0}") %></span>
                                    <div class="card-body text-center">
                                        <i class='far <%# Eval("icon") %> fa-3x mb-3'></i>
                                        <h3 class='h5 nav-heading mb-2 text-break'><%# Eval("Name") %></h3>
                                    </div>
                                </a>
                            </div>
	                    </ItemTemplate>
                    </asp:Repeater>

                </div>

            </div>                
        </div>

    </section>

    <section class="pb-4 mb-md-2" runat="server" ID="MailoutPanel">

        <h2 class="h4 mb-3">Deliveries</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4 mb-3">      
                        
                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/messages/mailouts/search">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="MailoutCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-mail-bulk fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 no-wrap'>Mailouts</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col" runat="server" id="EmailCountWidget">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/messages/emails/search">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="EmailCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-mail-bulk fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Emails</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/messages/clicks/search">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="ClickCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-mouse-pointer fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Clicks</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="mt-3">
                    <uc:ScheduledGrid runat="server" ID="ScheduledDeliveryGrid" />
                </div>

                <div class="mt-3">
                    <uc:CompletedGrid runat="server" ID="CompletedDeliveryGrid" />
                </div>

            </div>                
        </div>

    </section>

</asp:Content>
