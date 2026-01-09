<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomeCmds.ascx.cs" Inherits="InSite.UI.HomeCmds" %>

<%@ Register Src="~/UI/Admin/Foundations/Controls/MaintenanceToast.ascx" TagName="MaintenanceToast" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/RelatedPersons.ascx" TagName="RelatedPersons" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/ImpersonationGrid.ascx" TagName="ImpersonationGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/UpcomingSessionList.ascx" TagName="UpcomingSessionList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Home/Controls/Authentications.ascx" TagName="Authentications" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Home/Controls/CmdsCompetencyDashboard.ascx" TagName="CmdsCompetencyDashboard" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Home/Controls/CmdsComplianceSummary.ascx" TagName="CmdsComplianceSummary" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Home/Controls/CmdsLearningSummary.ascx" TagName="CmdsLearningSummary" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Home/Controls/CmdsOrientationSummary.ascx" TagName="CmdsOrientationSummary" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/PendingApprovalUserGrid.ascx" TagName="PendingApprovalUserGrid" TagPrefix="uc" %>

<style>
    .circular-progress { --ar-progress-thickness: 2rem; }
</style>

<section class="pb-5 mb-md-2">

    <insite:Toast runat="server" ID="InfoToast" Icon="fas fa-bell-on" Indicator="Information" Title="Important Reminder" />
    <uc:MaintenanceToast runat="server" ID="MaintenanceToast" />

    <div class="btn-toolbar mb-4" role="toolbar" aria-label="Shortcut toolbar">
        <div class="btn-group me-2 mb-2" role="group" aria-label="Shortcut buttons">

            <a runat="server" id="ElearningLink" href="/ui/portal/learning/catalog" class="btn btn-success"><i class="fa-solid fa-books me-2"></i>eLearning</a>
            <a runat="server" id="CustomCatalogLink" href="#" class="btn btn-warning"></a>
            <a runat="server" id="OrientationAnchor" class="btn btn-primary"><i class="fa-solid fa-passport me-2"></i>Orientations</a>

            <a runat="server" id="TrainingPlanLink1" href="/ui/portal/learning/plan" class="btn btn-secondary"><i class="fa-solid fa-map-marked-alt me-2"></i>Training Plan</a>
            <a runat="server" id="EducationAndTrainingLink" href="/ui/cmds/portal/achievements/credentials/search" class="btn btn-secondary"><i class="fa-solid fa-graduation-cap me-2"></i>Education &amp; Training</a>

        </div>
    </div>

    <div class="row">

        <div class="col-lg-12">

            <div class="card border-0 shadow-lg">

                <div class="card-body">

                    <insite:Nav runat="server" ID="Nav">

                        <insite:NavItem runat="server" ID="NavDashboard" Title="Dashboard">

                            <uc:CmdsCompetencyDashboard runat="server" ID="CmdsCompetencyDashboard" />

                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="NavProfiles" Title="Profiles">

                            <div class="row">

                                <div runat="server" class="col-md-4" visible="false">

                                    <h3 class="m-t-0">
                                        <asp:Literal ID="CompanyName" runat="server" />
                                        Departments</h3>

                                    <asp:Repeater ID="Departments" runat="server">
                                        <SeparatorTemplate>
                                            <br />
                                        </SeparatorTemplate>
                                        <ItemTemplate>
                                            <%# Eval("DepartmentName") %>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <br />
                                            <br />
                                        </FooterTemplate>
                                    </asp:Repeater>

                                </div>
                                <div class="col-md-12">

                                    <h3 class="mt-0">Profiles</h3>

                                    <div class="row">
                                        <div class="col-lg-4 mb-3 mb-lg-0">
                                            <div class="card h-100">
                                                <div class="card-body">

                                                    <h4 class="card-title mb-1 h6">Primary Profile</h4>

                                                    <small class="text-muted mb-2 d-block">Required for compliance</small>

                                                    <div runat="server" id="PrimaryProfile">
                                                        <asp:HyperLink runat="server" ID="PrimaryProfileLink">
                                                            <asp:Literal ID="ProfileName" runat="server" />
                                                        </asp:HyperLink>
                                                    </div>

                                                    <div runat="server" id="NoPrimaryProfile" visible="false" class="alert alert-info">
                                                        You have no primary profile assigned.
                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 mb-3 mb-lg-0">
                                            <div class="card h-100">
                                                <div class="card-body">

                                                    <h4 class="card-title mb-1 h6">Secondary Profiles</h4>

                                                    <small class="text-muted mb-2 d-block">Required for compliance</small>

                                                    <asp:Repeater ID="SecondaryProfilesForCompliance" runat="server">
                                                        <ItemTemplate>
                                                            <div class="mb-3">
                                                                <asp:HyperLink runat="server" NavigateUrl='<%# "/ui/cmds/portal/validations/competencies/search?profile=" + Eval("ProfileStandardIdentifier") + "&userID=" + Eval("UserIdentifier") + "&department=" + Eval("DepartmentIdentifier") %>'>
                                                        <%# Eval("ProfileName") %>
                                                        <%# Eval("ProfileStatusName") != DBNull.Value ? "(" + Eval("ProfileStatusName") + ")": "" %>
                                                                </asp:HyperLink>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>

                                                    <div runat="server" id="NoSecondaryProfilesForCompliance" visible="false" class="alert alert-info">
                                                        You have no secondary profiles assigned.
                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 mb-3 mb-lg-0">
                                            <div class="card h-100">
                                                <div class="card-body">

                                                    <h4 class="card-title mb-1 h6">Other Profiles</h4>

                                                    <small class="text-muted mb-2 d-block">Not required for compliance</small>

                                                    <asp:Repeater ID="SecondaryProfilesNotForCompliance" runat="server">
                                                        <ItemTemplate>
                                                            <div class="mb-3">
                                                                <%# Eval("ProfileName") %>
                                                                <%# Eval("ProfileStatusName") != DBNull.Value ? "(" + Eval("ProfileStatusName") + ")": "" %>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>

                                                    <div runat="server" id="NoSecondaryProfilesNotForCompliance" visible="false" class="alert alert-info">
                                                        You have no other profiles assigned.
                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                            </div>

                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="NavContacts" Title="Contacts">

                            <div class="row mb-5">
                                <div runat="server" id="MyContactsPanel" class="col-lg-7 mb-3 mb-lg-0">
                                    <h3>My Contacts</h3>
                                    <uc:RelatedPersons ID="RelatedPersons" runat="server" />
                                </div>
                                <div class="col-lg-5 mb-3 mb-lg-0">
                                    <h3 class="mt-0">My Roles</h3>
                                    <ul>
                                        <asp:Repeater ID="Roles" runat="server">
                                            <ItemTemplate>
                                                <li><%# Container.DataItem %></li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-12">

                                    <h3 class="">My Login History</h3>
                                    <uc:Authentications runat="server" ID="Authentications" />

                                </div>
                            </div>

                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="NavUsers" Title="New Users">

                            <h3>New Users<span class="fs-sm text-body-secondary"><i class="fas fa-caret-right ms-2 me-2"></i>Accounts pending administrator approval</span></h3>

                            <uc:PendingApprovalUserGrid runat="server" ID="PendingApprovalUsers" />

                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="NavImpersonations" Title="Impersonations">

                            <h3>Impersonations</h3>

                            <uc:ImpersonationGrid runat="server" ID="ImpersonationGrid" />

                        </insite:NavItem>

                    </insite:Nav>
                </div>

            </div>

        </div>

    </div>

</section>

<section runat="server" id="LearningPortalSection" class="pb-5 mb-md-2">

    <h2 class="h4 mb-3">Learning Portal</h2>

    <div class="card border-0 shadow-lg">
        <div class="card-body">

            <div class="row row-cols-1 row-cols-md-3 g-4">

                <div runat="server" id="ProfilesAndCertificatesPanel" class="col">

                    <div runat="server" id="EventCard" class="card card-tile shadow mb-4">
                        <div class="card-body text-center">
                            <i class='far fa-users-class fa-3x mb-3 text-primary'></i>
                            <h3 class='h5 nav-heading mb-2 text-break'>Upcoming Training Sessions</h3>
                        </div>
                        <uc:UpcomingSessionList runat="server" ID="UpcomingSessions" />
                    </div>

                    <div runat="server" id="CollegeCertificatesCard" class="card card-tile shadow">
                        <div class="card-body text-center">
                            <i class='far fa-university fa-3x mb-3 text-primary'></i>
                            <h3 class='h5 nav-heading mb-2 text-break'>College Certificates</h3>
                        </div>
                        <ul class="list-group list-group-flush">
                            <li class="list-group-item"><a href="/ui/cmds/portal/validations/college-certificates/search">View My Eligibility</a></li>
                            <li runat="server" id="SearchCollegeCertificatesLink" class="list-group-item"><a href="/ui/cmds/admin/validations/college-certificates/search">Search College Certificates</a></li>
                        </ul>
                    </div>

                </div>

                <div runat="server" id="CompetenciesCard" class="col">
                    <div class="card card-tile shadow">
                        <div class="card-body text-center">
                            <i class='far fa-ruler-triangle fa-3x mb-3 text-primary'></i>
                            <h3 class='h5 nav-heading mb-2 text-break'>Competencies</h3>
                        </div>
                        <ul class="list-group list-group-flush">
                            <li class="list-group-item"><a href="/ui/cmds/portal/validations/competencies/start">Begin Self-Assessment</a></li>
                            <li runat="server" id="ViewMySelfAssessmentsLink" class="list-group-item"><a href="/ui/cmds/portal/validations/competencies/search">View My Self-Assessments</a></li>
                            <li class="list-group-item"><a href="/ui/cmds/portal/validations/competencies/submit">Request Competency Validation</a></li>
                            <li runat="server" id="ValidateCompetenciesLink2" class="list-group-item"><a href="/ui/cmds/design/validations/competencies/start">Validate Competencies</a></li>
                        </ul>
                    </div>
                </div>

                <div class="col">
                    <div class="card card-tile shadow">
                        <div class="card-body text-center">
                            <i class='far fa-award fa-3x mb-3 text-primary'></i>
                            <h3 class='h5 nav-heading mb-2 text-break'>Achievements</h3>
                        </div>
                        <ul class="list-group list-group-flush">
                            <li runat="server" id="TrainingPlanLink2" class="list-group-item"><a href="/ui/portal/learning/plan">My Training Plan</a></li>
                            <li class="list-group-item"><a href="/ui/cmds/admin/uploads/outline">Organization-Specific Achievements</a></li>
                            <li class="list-group-item"><i class="fas fa-caret-right me-1"></i><a href="/ui/cmds/admin/uploads/outline?achievementType=Additional+Compliance+Requirement">Additional Compliance Requirements</a></li>
                            <li class="list-group-item"><i class="fas fa-caret-right me-1"></i><a href="/ui/cmds/admin/uploads/outline?achievementType=Code+of+Practice" runat="server" id="CopAnchor">Codes of Practice</a></li>
                            <li class="list-group-item"><i class="fas fa-caret-right me-1"></i><a href="/ui/cmds/admin/uploads/outline?achievementType=Human+Resources+Document">Human Resources Documents</a></li>
                            <li class="list-group-item"><i class="fas fa-caret-right me-1"></i><a href="/ui/cmds/admin/uploads/outline?achievementType=Safe+Operating+Practice" runat="server" id="SopAnchor">Safe Operating Practices</a></li>
                            <li class="list-group-item"><i class="fas fa-caret-right me-1"></i><a href="/ui/cmds/admin/uploads/outline?achievementType=Site-Specific+Operating+Procedure" runat="server" id="OppAnchor">Operating Procedures</a></li>
                            <li class="list-group-item"><i class="fas fa-caret-right me-1"></i><a href="/ui/cmds/admin/uploads/outline?achievementType=Training+Guide" runat="server" id="TrdAnchor">Training Documents</a></li>

                            <li runat="server" id="SearchUploadsLink" class="list-group-item"><a href="/ui/cmds/admin/uploads/search">Search Files &amp; Links</a></li>
                            <li runat="server" id="ManageUploadsLink" class="list-group-item"><a href="/ui/cmds/admin/uploads/upload">Upload Files &amp; Links</a></li>
                        </ul>
                    </div>
                </div>

                <div class="col">
                </div>

            </div>

        </div>
    </div>

</section>

<section runat="server" id="ProgressSection" class="pb-5 mb-md-2">

    <h2 class="h4 mb-3">Progress</h2>

    <div class="card border-0 shadow-lg">
        <div class="card-body">

            <asp:Repeater runat="server" ID="ProgressRepeater">
                <HeaderTemplate>
                    <div class="row">
                </HeaderTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
                <ItemTemplate>
                    <div class="col-6 col-xxl-3 mb-3">
                        <div class="card h-100">
                            <div class="card-body">
                                <%# Eval("ChartHtml") %>
                                <div class="mt-4 text-center">
                                    <div class="fw-bold mb-1"><%# Eval("Title") %></div>
                                    <div>
                                        <asp:HyperLink runat="server" NavigateUrl='<%# Eval("ProgressUrl") %>' Text='<%# Eval("ProgressText") %>' Visible='<%# Eval("ProgressUrl") != null %>' />
                                        <asp:Literal runat="server" Text='<%# Eval("ProgressText") %>' Visible='<%# Eval("ProgressUrl") == null %>' />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </div>

</section>
