<%@ Page Language="C#" CodeBehind="Manage.aspx.cs" Inherits="InSite.UI.Admin.Courses.Manage" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Outlines/Controls/CourseSetup.ascx" TagName="CourseSetup" TagPrefix="uc" %>
<%@ Register Src="./Outlines/Controls/UnitSetup.ascx" TagName="UnitSetup" TagPrefix="uc" %>
<%@ Register Src="./Outlines/Controls/ModuleSetup.ascx" TagName="ModuleSetup" TagPrefix="uc" %>
<%@ Register Src="./Outlines/Controls/ModuleTreeView.ascx" TagName="ModuleTreeView" TagPrefix="uc" %>
<%@ Register Src="./Outlines/Controls/ActivitySetup.ascx" TagName="ActivitySetup" TagPrefix="uc" %>
<%@ Register Src="./Outlines/Controls/NotificationSetup.ascx" TagName="NotificationSetup" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="CourseSetup" />
    <insite:ValidationSummary runat="server" ValidationGroup="NotificationSetup" />
    <insite:ValidationSummary runat="server" ValidationGroup="ActivitySetup" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="OutlinePanel" Title="Course Outline" Icon="far fa-sitemap" IconPosition="BeforeText">
            <section>

                <h2 class="h4 mt-4 mb-3">Course Outline</h2>

                <div class="row">
                    <div class="col-lg-5">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <uc:ModuleTreeView runat="server" ID="ModuleTreeView" />
                                <div runat="server" id="NoGradebookReminder" class="alert alert-warning">
                                    Remember to create a gradebook to enable learner enrollment and progress tracking for this course.
                                    <a runat="server" id="CreateGradebookLink" href="#">Click here to create a gradebook.</a>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-7">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <asp:PlaceHolder runat="server" ID="ActivityPlaceHolder" />
                            </div>
                        </div>
                    </div>
                </div>

            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="ActivityPanel" Title="Activity Setup" Icon="far fa-cube" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Activity Setup
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:ActivitySetup runat="server" ID="ActivitySetup" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="ModulePanel" Title="Module Setup" Icon="far fa-cubes" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Module Setup
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <uc:ModuleSetup runat="server" ID="ModuleSetup" />

                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="UnitPanel" Title="Unit Setup" Icon="far fa-cubes" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Unit Setup
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:UnitSetup runat="server" ID="UnitSetup" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="NotificationPanel" Title="Notifications" Icon="far fa-bell" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Notifications
                </h2>

                <uc:NotificationSetup runat="server" ID="NotificationSetup" />
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CoursePanel" Title="Course Setup" Icon="far fa-chalkboard-teacher" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Course Setup
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:CourseSetup runat="server" ID="CourseSetup" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
