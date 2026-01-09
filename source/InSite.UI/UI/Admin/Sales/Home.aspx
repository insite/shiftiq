<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Invoices.Dashboard" %>

<%@ Register Src="Reports/Controls/RecentList.ascx" TagPrefix="uc" TagName="RecentList" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="HomeStatus" />

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">e-Commerce</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">      
                        
                    <div runat="server" id="InvoicesCounter" class="col">
                        <a runat="server" id="InvoicesLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="InvoicesCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-file-invoice-dollar fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Invoices</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="PaymentsCounter" class="col">
                        <a runat="server" id="PaymentsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="PaymentsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-credit-card-front fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Payments</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="OrdersCounter" class="col">
                        <a runat="server" id="OrdersLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="OrdersCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-cart-shopping fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Orders</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a class="me-3" href="/ui/admin/sales/reports/event-registration-payments" runat="server" id="A4"><i class="fas fa-file-chart-line me-1"></i>Event Registration Payments</a>
                    </div>
                </div>

            </div>                
        </div>

    </section>

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Inventory Setup</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">      
                        
                    <div runat="server" id="ProductsCounter" class="col">
                        <a runat="server" id="ProductsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="ProductsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-pallet fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Products</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="PackagesCounter" class="col">
                        <a runat="server" id="PackagesLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="PackagesCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-pallet fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Packages</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="DiscountsCounter" class="col" visible="false">
                        <a runat="server" id="DiscountsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="DiscountsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-badge-percent fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Discounts</h3>
                            </div>
                        </a>
                    </div> 

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a class="me-3" href="/ui/admin/sales/taxes/search"><i class="fas fa-badge-percent me-1"></i>Taxes</a>
                    </div>
                </div>

            </div>                
        </div>

    </section>

    <asp:Placeholder runat="server" id="HistoryPanel">
        <section class="pb-4 mb-md-2">
            <h2 class="h4 mb-3">Recent Changes</h2>
            <div class="card border-0 shadow-lg">
                <div class="card-header">
                    <ul class="nav nav-tabs card-header-tabs" role="tablist">
                        <li class="nav-item"><a class="nav-link active" href="#result1" data-bs-toggle="tab" role="tab" aria-controls="result1" aria-selected="true">Payments</a></li>
                    </ul>
                </div>
                <div class="card-body">
                    <uc:RecentList runat="server" ID="RecentList" />
                </div>
            </div>
        </section>
    </asp:Placeholder>

</asp:Content>