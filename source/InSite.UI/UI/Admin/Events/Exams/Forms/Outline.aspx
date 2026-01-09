<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Events.Exams.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../../Comments/Controls/CommentPanel.ascx" TagName="CommentPanel" TagPrefix="uc" %>
<%@ Register Src="../../Attendees/Controls/AttendeePanel.ascx" TagName="AttendeePanel" TagPrefix="uc" %>
<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>
<%@ Register Src="../../Candidates/Controls/CandidatePanel.ascx" TagName="CandidatePanel" TagPrefix="uc" %>
<%@ Register Src="../../Timers/Controls/TimerPanel.ascx" TagName="TimerPanel" TagPrefix="uc" %>
<%@ Register Src="../Controls/SeatGrid.ascx" TagName="SeatGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/MultilingualStringInfo.ascx" TagName="MultilingualStringInfo" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Exam" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="EventSection" Title="Exam Event" Icon="far fa-calendar-alt" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Exam Event</h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:Detail ID="Detail" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SeatPanel" Title="Seats" Icon="far fa-money-check-alt" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <h2 class="h4 mb-1"></h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div class="text-end" style="padding-bottom: 15px;">
                                    <insite:Button runat="server" ID="AddSeat" Text="Add Seat" Icon="fas fa-plus-circle" ButtonStyle="OutlinePrimary" />
                                </div>

                                <uc:SeatGrid runat="server" ID="SeatsGrid" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="AttendeeSection" Title="Contacts" Icon="far fa-users" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Contacts
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:AttendeePanel runat="server" ID="AttendeePanel" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CandidateSection" Title="Candidates" Icon="far fa-id-card" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Candidates
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:CandidatePanel runat="server" ID="CandidatePanel" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ContentSection" Title="Content" Icon="far fa-edit" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <h2 class="h4 mb-1"></h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div style="position: absolute; right: 15px; z-index: 1;">
                                    <div>
                                        <insite:Button runat="server" ID="PreviewLink" Text="Preview" Icon="fas fa-external-link" ButtonStyle="OutlinePrimary" NavigateTarget="_blank" />
                                    </div>
                                </div>

                                <div class="row content-details">
                                    <div class="col-md-12">

                                        <insite:Nav runat="server" ID="ContentNavigation">

                                            <insite:NavItem runat="server" ID="TitleTab" Title="Title">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" ID="EditContentTitleLink" ToolTip="Revise Title" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentTitle" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="SummaryTab" Title="Summary">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" ID="EditContentSummaryLink" ToolTip="Revise Summary" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentSummary" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="DescriptionTab" Title="Description">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" ID="EditContentDescriptionLink" ToolTip="Revise Description" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentDescription" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="MaterialsTab" Title="Materials">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" ID="EditContentMaterialsLink" ToolTip="Revise Materials for Participation" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentMaterials" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="InstructionsTab" Title="Instructions">
                                                <div class="card">
                                                    <div class="card-body">
                                                        <insite:Nav runat="server">
                                                            <insite:NavItem runat="server" ID="ContactSubTab" Title="Contact and Support">
                                                                <div class="content-cmds">
                                                                    <insite:Button runat="server" ID="EditContentContactLink" ToolTip="Revise Contact and Support Instructions" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                                </div>
                                                                <div class="content-string">
                                                                    <uc:MultilingualStringInfo runat="server" ID="ContentContact" />
                                                                </div>
                                                            </insite:NavItem>
                                                            <insite:NavItem runat="server" ID="AccommodationSubTab" Title="Accommodation">
                                                                <div class="content-cmds">
                                                                    <insite:Button runat="server" ID="EditContentAccommodationLink" ToolTip="Revise Accommodation Instructions" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                                </div>
                                                                <div class="content-string">
                                                                    <uc:MultilingualStringInfo runat="server" ID="ContentAccommodation" />
                                                                </div>
                                                            </insite:NavItem>
                                                            <insite:NavItem runat="server" ID="AdditionalSubTab" Title="Additional">
                                                                <div class="content-cmds">
                                                                    <insite:Button runat="server" ID="EditContentAdditionalLink" ToolTip="Revise Additional Instructions" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                                </div>
                                                                <div class="content-string">
                                                                    <uc:MultilingualStringInfo runat="server" ID="ContentAdditional" />
                                                                </div>
                                                            </insite:NavItem>
                                                            <insite:NavItem runat="server" ID="CancellationSubTab" Title="Cancellation and Refund">
                                                                <div class="content-cmds">
                                                                    <insite:Button runat="server" ID="EditContentCancellationLink" ToolTip="Revise Cancellation and Refund Instructions" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                                </div>
                                                                <div class="content-string">
                                                                    <uc:MultilingualStringInfo runat="server" ID="ContentCancellation" />
                                                                </div>
                                                            </insite:NavItem>
                                                            <insite:NavItem runat="server" ID="CompletionSubTab" Title="Registration Completion">
                                                                <div class="content-cmds">
                                                                    <insite:Button runat="server" ID="EditContentCompletionLink" ToolTip="Revise Registration Completion Instructions" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                                                </div>
                                                                <div class="content-string">
                                                                    <uc:MultilingualStringInfo runat="server" ID="ContentCompletion" />
                                                                </div>
                                                            </insite:NavItem>
                                                        </insite:Nav>
                                                    </div>
                                                </div>
                                            </insite:NavItem>
                                        </insite:Nav>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CommentSection" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Comments
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:CommentPanel ID="CommentPanel" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="TimerSection" Title="Timers" Icon="far fa-alarm-clock" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Timers
                </h2>

                <uc:TimerPanel runat="server" ID="TimerPanel" />
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
