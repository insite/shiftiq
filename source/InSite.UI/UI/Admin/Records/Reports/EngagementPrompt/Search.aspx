<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Admin.Records.Reports.EngagementPrompt.Search" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="./SearchCriteria.ascx" TagName="SearchCriteria" TagPrefix="uc" %>
<%@ Register Src="./SearchResults.ascx" TagName="SearchResults" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    
    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="MessageTab" Title="Message" Icon="far fa-bell" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Message
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
            
                        <strong>Gentle Reminder: $ProgramName</strong>

                        <p>Hello $LearnerNameFirst,</p>

                        <p>This is a friendly reminder that you enrolled in the $ProgramName on $EnrollmentStarted.</p>
    
                        <p>
                            According the gradebook(s) for this program, you have not yet been granted a Certificate of completion.
                            Our records also indicate that you most recently logged in $SessionStartedLast.
                        </p>

                        <p>Let us know if you are having any difficulties completing the work. We are here to help!</p>

                        <p>You can reply to this message by replying directly to this email.</p>

                        <p>(Administrator/Instructor adds whatever additional information and/or signature they want here.)</p>

                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchCriteriaTab" Title="Search" Icon="far fa-search" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Search
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:SearchCriteria runat="server" ID="SearchCriteria" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchResultsTab" Title="Recipients" Icon="far fa-users" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Recipients
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:SearchResults runat="server" ID="SearchResults" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchScheduleSection" Title="Schedule" Icon="far fa-calendar" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Schedule
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
            
                    <ul>
                        <li>
                            Send this message now.
                        </li>
                        <li>
                            Send this message every Day|Week|Month.
                        </li>
                    </ul>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
