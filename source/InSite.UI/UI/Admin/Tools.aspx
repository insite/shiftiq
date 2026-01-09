<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" CodeBehind="Tools.aspx.cs" Inherits="InSite.UI.CMDS.Tools" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-5 mb-md-2">
        
        <h2 class="h4 mb-3">Accounts</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">               
                
                <div class="row row-cols-1 row-cols-md-4 g-4">

                    <div runat="server" id="SearchPeopleLink" class="col">
                        <a class="card card-hover card-tile border-0 shadow" href='/ui/cmds/admin/users/search'>
                            <div class="card-body text-center">
                                <i class='far fa-user fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>People</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="SearchOrganizationsLink" class="col">
                        <a class="card card-hover card-tile border-0 shadow" href='/ui/cmds/admin/organizations/search'>
                            <div class="card-body text-center">
                                <i class='far fa-city fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Organizations</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a class="me-3" href="/ui/cmds/admin/messages/assign-subscribers"><i class="fas fa-envelope-open-text me-1"></i>Notification Subscribers</a>
                        <a class="me-3" href="/ui/cmds/admin/messages/assign-followers"><i class="fas fa-user-headset me-1"></i>Notification Followers</a>
                        <a runat="server" id="OverrideCompetenciesLink" class="me-3" href="/ui/cmds/admin/validations/competencies/search"><i class="fas fa-ruler-triangle me-1"></i>Override Competencies</a>
                    </div>
                </div>

            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Libraries</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">               
                
                <div class="row row-cols-1 row-cols-md-4 g-4">

                    <div runat="server" id="SearchAchievementsLink" class="col">
                        <a class="card card-hover card-tile border-0 shadow" href='/ui/cmds/admin/achievements/search'>
                            <div class="card-body text-center">
                                <i class='far fa-trophy fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Achievements</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="SearchCompetenciesLink" class="col">
                        <a class="card card-hover card-tile border-0 shadow" href='/ui/cmds/admin/standards/competencies/search'>
                            <div class="card-body text-center">
                                <i class='far fa-ruler-triangle fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Competencies</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="SearchProfilesLink" class="col">
                        <a class="card card-hover card-tile border-0 shadow" href='/ui/cmds/admin/standards/profiles/search'>
                            <div class="card-body text-center">
                                <i class='far fa-id-badge fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Profiles</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="SearchProgramsLink" class="col">
                        <a class="card card-hover card-tile border-0 shadow" href='<%= InSite.Admin.Records.Programs.Search.NavigateUrl %>'>
                            <div class="card-body text-center">
                                <i class='far fa-map-marked-alt fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Programs</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        
                        <a runat="server" id="SearchCategoriesLink" class="me-3" href="/ui/admin/learning/categories/search"><i class="fas fa-tags me-1"></i>Achievement Categories</a>
                        
                        <a runat="server" id="CompareProfilesLink" class="me-3" href="/ui/cmds/admin/standards/profiles/compare"><i class="fas fa-id-badge me-1"></i>Compare Profiles</a>

                        <asp:HyperLink runat="server" id="ScoopLibraryLink" CssClass="me-3" NavigateUrl="#"></asp:HyperLink>
                        
                    </div>
                </div>

            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Libraries (Field Admins)</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">               
                
                <div class="row row-cols-1 row-cols-md-4 g-4">

                    <div runat="server" id="FieldSearchAchievementsLink" class="col">
                        <a class="card card-hover card-tile border-0 shadow" href='/ui/cmds/design/achievements/search'>
                            <div class="card-body text-center">
                                <i class='far fa-trophy fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Achievements</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="FieldSearchProfilesLink" class="col">
                        <a class="card card-hover card-tile border-0 shadow" href='/ui/cmds/design/standards/profiles/search'>
                            <div class="card-body text-center">
                                <i class='far fa-id-badge fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Profiles</h3>
                            </div>
                        </a>
                    </div>

                </div>

            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Tools</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">               
                
                <div class="row row-cols-1 row-cols-md-3 g-4">

                    <div class="col">
                        <div class="card card-hover card-tile shadow">
                            <div class="card-body text-center">
                                <i class='far fa-edit fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Bulk Update</h3>
                            </div>
                            <ul class="list-group list-group-flush">
                                <li runat="server" id="AssignDepartmentsLink1" class="list-group-item"><a href="/ui/cmds/admin/achievements/credentials/assign-department">Assign Achievements to Departments</a></li>
                                <li runat="server" id="BulkAssignCertificatesLink" class="list-group-item"><a href="/ui/cmds/admin/achievements/credentials/assign-certificate">Assign Certificates to Learners</a></li>
                                <li runat="server" id="BulkSetupforPolicySignOffs" class="list-group-item"><a href="/ui/cmds/admin/achievements/credentials/assign">Assign Education &amp; Training to Learners</a></li>
                                <li runat="server" id="BulkAssignEmployeestoManagers" class="list-group-item"><a href="/ui/cmds/admin/departments/assign-managers">Assign People to Managers</a></li>
                                <li runat="server" id="AssignDepartmentsLink2" class="list-group-item"><a href="/ui/cmds/admin/departments/assign-users">Assign People to Departments</a></li>
                                <li runat="server" id="AssignDepartmentsLink3" class="list-group-item"><a href="/ui/cmds/admin/departments/profile-competencies/assign">Assign Profiles to Departments</a></li>
                                <li runat="server" id="AssignProfilestoPeople" class="list-group-item"><a href="/ui/cmds/admin/standards/profiles/assign">Assign Profiles to People</a></li>
                                <li runat="server" id="AssignProgramsLink" class="list-group-item"><a href="/ui/admin/learning/programs/enrollments/assign">Assign Programs to Learners</a></li>
                                <li runat="server" id="BulkEmployeeAchievementExpiry" class="list-group-item"><a href="/ui/cmds/admin/achievements/credentials/expire">Expire Achievements</a></li>
                                <li runat="server" id="BulkCompetencyExpiry" class="list-group-item"><a href="/ui/cmds/admin/validations/competencies/expire">Expire Competencies</a></li>
                                <li runat="server" id="ValidateCompetenciesLink" class="list-group-item"><a href="/ui/cmds/admin/validations/competencies/validate">Validate Competencies</a></li>
                            </ul>
                        </div>
                    </div>

                    <div class="col">
                        <div class="card card-hover card-tile shadow">
                            <div class="card-body text-center">
                                <i class='far fa-laptop-medical fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Troubleshooting</h3>
                            </div>
                            <ul runat="server" id="DeveloperTools" class="list-group list-group-flush">
                                <li class="list-group-item"><a href="/ui/cmds/admin/departments/profile-users/search">Department Profile Learners</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/user-fees">Invoice Addendum</a></li>
                                <li class="list-group-item"><a href="/ui/admin/reporting/login-history">Login History</a></li>
                                <li class="list-group-item"><a href="/ui/admin/reporting/phone-list">Phone List</a></li>
                            </ul>
                            <ul runat="server" id="TroubleshootingTools" class="list-group list-group-flush">
                                <li class="list-group-item"><a href="/ui/cmds/admin/validations/search">Validations</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/validations/changes/search">Validation Changes</a></li>
                            </ul>
                        </div>
                    </div>

                    <div runat="server" id="AnalyticsPanel" class="col">
                        <div class="card card-hover card-tile shadow">
                            <div class="card-body text-center">
                                <i class='far fa-analytics fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Analytics</h3>
                            </div>
                            <ul class="list-group list-group-flush">
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/executive-summary-on-achievement-status">Achievement Summary</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/executive-summary-on-competency-status">Competency Summary</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/department-user-status">Learner Summary</a></li>
                            </ul>
                        </div>
                    </div>

                </div>

            </div>
        </div>

    </section>

</asp:Content>