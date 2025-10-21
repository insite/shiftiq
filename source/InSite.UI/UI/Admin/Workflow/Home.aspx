<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Issues.Dashboard" %>

<%@ Register Src="./Cases/Controls/RecentList.ascx" TagName="RecentList" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Cases</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">      
                        
                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/workflow/cases/search">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="IssueCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-briefcase fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Cases</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a href="/ui/admin/workflow/cases/reports"><i class="fas fa-fw fa-file-chart-line me-1"></i>Summary Reports</a>
                    </div>
                </div>

            </div>                
        </div>

    </section>

    <section class="pb-4 mb-md-2" runat="server" id="HistoryPanel">
        <h2 class="h4 mb-3">Recent Changes</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <uc:RecentList runat="server" ID="RecentList" />
            </div>
        </div> 
    </section>

</asp:Content>
