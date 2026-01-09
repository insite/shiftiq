<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Courses.Dashboard" %>

<%@ Register Src="./Reports/Controls/RecentList.ascx" TagName="RecentList" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Courses</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/courses/search">
                            <span runat="server" id="CourseCount" class="badge badge-floating badge-pill bg-primary"></span>
                            <div class="card-body text-center">
                                <i class='far fa-chalkboard-teacher fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Courses</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/learning/catalogs/search">
                            <span runat="server" id="CatalogCount" class="badge badge-floating badge-pill bg-primary"></span>
                            <div class="card-body text-center">
                                <i class='far fa-books fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Catalogs</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/learning/categories/search">
                            <span runat="server" id="CategoryCount" class="badge badge-floating badge-pill bg-primary"></span>
                            <div class="card-body text-center">
                                <i class='far fa-tag fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Categories</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/courses/links/search">
                            <span runat="server" id="LinkCount" class="badge badge-floating badge-pill bg-primary"></span>
                            <div class="card-body text-center">
                                <i class='far fa-plug fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>LTI Links</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a class="me-3" href="#recent-changes"><i class="fas fa-history me-1"></i>Recent Changes</a>
                        <a class="me-3" href="/ui/admin/courses/scorm/upload"><i class="fas fa-upload me-1"></i>Upload SCORM Package</a>
                    </div>
                </div>

            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2" runat="server" id="HistoryPanel">
        <a name="recent-changes" style="scroll-margin-top: 140px;"></a>
        <h2 class="h4 mb-3">Recent Changes</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="tab-content">
                    <div class="tab-pane fade show active" id="result1" role="tabpanel">
                        <uc:RecentList runat="server" ID="RecentList" />
                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
