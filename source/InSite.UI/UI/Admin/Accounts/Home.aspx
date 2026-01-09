<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" 
    Inherits="InSite.UI.Admin.Accounts.Home"
    MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Organizations</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div runat="server" id="OrganizationCounter" class="col">
                        <a runat="server" id="OrganizationLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="OrganizationCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-city fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Organizations</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="DepartmentCounter" class="col">
                        <a runat="server" id="DepartmentLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="DepartmentCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-building fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Departments</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="DivisionCounter" class="col">
                        <a runat="server" id="DivisionLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="DivisionCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-industry fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Divisions</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <asp:LinkButton runat="server" ID="ClearOrganizationCache"><i class="fas fa-undo me-1"></i>Clear Cache</asp:LinkButton>
                    </div>
                </div>

            </div>
        </div>

    </section>

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Individuals</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div runat="server" id="UserCounter" class="col">
                        <a runat="server" id="UserLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="UserCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-user fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Users</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="SenderCounter" class="col">
                        <a runat="server" id="SenderLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="SenderCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-mailbox fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Senders</h3>
                            </div>
                        </a>
                    </div>

                </div>

            </div>
        </div>

    </section>

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Security</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div runat="server" id="PermissionCounter" class="col">
                        <a runat="server" id="PermissionLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="PermissionCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-key fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Permissions</h3>
                            </div>
                        </a>
                    </div>
                </div>
                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a class="me-3" href="/ui/admin/accounts/permissions/matrix"><i class="fas fa-table me-1"></i>Matrix</a>                      
                    </div>
                </div>
            </div>
        </div>

    </section>

</asp:Content>