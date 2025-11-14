<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Jobs.Dashboard" %>
<%@ Import Namespace="InSite.Common.Web.Infrastructure" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Jobs</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">      

                    <div runat="server" id="CandidateProfilesCounter" class="col">
                        <a runat="server" id="CandidateProfilesLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="CandidateProfilesCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-user-friends fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Candidates</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="EmployerProfilesCounter" class="col">
                        <a runat="server" id="EmployerProfilesLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="EmployerProfilesCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-user-tie fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Employers</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="JobListingsCounter" class="col">
                        <a runat="server" id="JobListingsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="JobListingsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-suitcase fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Job Opportunities</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="JobApplicationsCounter" class="col">
                        <a runat="server" id="JobApplicationsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="JobApplicationsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-folder-open fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Job Applications</h3>
                            </div>
                        </a>
                    </div>

                </div>

            </div>                
        </div>

    </section>

</asp:Content>
