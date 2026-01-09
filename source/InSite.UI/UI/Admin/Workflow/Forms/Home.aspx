<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Home" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/RecentList.ascx" TagName="RecentList" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Forms</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">      
                        
                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/workflow/forms/search">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="SurveyFormCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-check-square fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Forms</h3>
                            </div>
                        </a>
                    </div>

                </div>

            </div>                
        </div>

    </section>
    
    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Submissions</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div runat="server" id="Div1">                    

                    <div class="row row-cols-1 row-cols-lg-4 g-4">      
                        
                        <div class="col">
                            <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/workflow/forms/submissions/search">
                                <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="ResponseSessionCount" /></span>
                                <div class="card-body text-center">
                                    <i class='far fa-poll-people fa-3x mb-3'></i>
                                    <h3 class='h5 nav-heading mb-2 text-break'>Submissions</h3>
                                </div>
                            </a>
                        </div>

                    </div>

                </div>                
            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2" runat="server" ID="HistoryPanel">
        <h2 class="h4 mb-3">Recent Changes</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-header">
                <ul class="nav nav-tabs card-header-tabs" role="tablist">
                  <li class="nav-item"><a class="nav-link active" href="#result1" data-bs-toggle="tab" role="tab" aria-controls="result1" aria-selected="true">Forms</a></li>
                </ul>
            </div>
            <div class="card-body">
                <uc:RecentList runat="server" ID="RecentChanges" />
            </div>
        </div>
    </section>

</asp:Content>
