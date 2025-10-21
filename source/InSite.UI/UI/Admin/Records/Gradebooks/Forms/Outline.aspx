<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Records.Gradebooks.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/CredentialGrid.ascx" TagName="CredentialGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/GradeItemsGrid.ascx" TagName="GradeItemsGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/LearningMasteryGrid.ascx" TagName="LearningMasteryGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/OutlineProgressList.ascx" TagName="OutlineProgressList" TagPrefix="uc" %>
<%@ Register Src="../Controls/ScormEventGrid.ascx" TagName="ScormEventGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/ScormRegistrationGrid.ascx" TagName="ScormRegistrationGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UpdatePanel runat="server" ID="StatusUpdatePanel">
        <ContentTemplate>
            <insite:Alert runat="server" ID="StatusAlert" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="ProgressPanel" Title="Learner Progress" Icon="far fa-ballot-check" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Learner Progress
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:OutlineProgressList runat="server" ID="ProgressList" UserMode="Admin" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="LearningMasteryPanel" Title="Learning Mastery" Icon="far fa-ballot-check" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Learning Mastery
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:LearningMasteryGrid runat="server" ID="LearningMasteryGrid" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CredentialPanel" Title="Achievements" Icon="far fa-award" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Achievements
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:CredentialGrid runat="server" ID="CredentialGrid" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="ScormPanel" Title="SCORM" Icon="far fa-id-card" IconPosition="BeforeText">
            <section>
                
                <h2 class="h4 mt-4 mb-3">SCORM Registrations</h2>
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:ScormRegistrationGrid runat="server" ID="ScormRegistrationGrid" />
                    </div>
                </div>

                <h2 class="h4 mt-4 mb-3">SCORM Events</h2>
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:ScormEventGrid runat="server" ID="ScormEventGrid" />
                    </div>
                </div>

            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="ConfigurationSection" Title="Grade Items" Icon="far fa-list-ul" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Grade Items
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
            
                        <div runat="server" id="GradeItemModifyPanel" style="padding-bottom:15px;">
                            <insite:Button runat="server" ID="AddCategoryButton" Text="Add Category" Icon="fas fa-plus-circle" ButtonStyle="Default" />
                            <insite:Button runat="server" ID="AddScoreButton" Text="Add Score" Icon="fas fa-plus-circle" ButtonStyle="Default" />
                            <insite:Button runat="server" ID="AddCalculationButton" Text="Add Calculation" Icon="fas fa-plus-circle" ButtonStyle="Default" />
                        </div>

                        <uc:GradeItemsGrid runat="server" ID="GradeItemsGrid" />

                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="GradebookPanel" Title="Gradebook Setup" Icon="far fa-spell-check" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Gradebook Setup
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="row button-group mb-3">
                            <div class="col-lg-12">
                                <insite:Button runat="server" ID="NewGradebookLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/records/gradebooks/open" />

                                <insite:ButtonSpacer runat="server" />

                                <insite:Button runat="server" ID="CopyLink" Text="Duplicate" ButtonStyle="Default" Icon="fas fa-copy" />
                                <insite:Button runat="server" ID="LockButton" Text="Lock" Icon="fas fa-lock" ConfirmText="Locking this gradebook will prevent additional edits. Learners in any attached courses will no longer be able to progress. Manually granted achievements can no longer be granted. Are you sure you want to proceed?" ButtonStyle="Default" />
                                <insite:Button runat="server" ID="UnlockButton" Text="Unlock" icon="fas fa-lock-open" ConfirmText="Unlocking this gradebook will allow changes to be made to existing scores, learners, and other settings. Are you sure you want to proceed?" ButtonStyle="Default" />


                                <insite:ButtonSpacer runat="server" />

                                <insite:Button runat="server" ID="DownloadLink" Text="Download JSON" icon="fas fa-download" ButtonStyle="Primary" />
                                <insite:Button runat="server" ID="ViewHistoryLink" Text="History" Icon="fas fa-history" ButtonStyle="Default" />
                                <insite:DeleteButton runat="server" ID="DeleteLink" />

                            </div>
                        </div>

                        <div class="row">
                        
                            <div class="col-md-6">
                                    
                                <div class="card h-100">
                                    <div class="card-body">

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="Rename" style="padding:8px" ToolTip="Rename Gradebook" />
                                            </div>
                                            <label class="form-label">
                                                Gradebook Title
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="GradebookTitle" />
                                            </div>
                                            <div class="form-text">A descriptive title for this gradebook.</div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeAchievement" style="padding:8px" ToolTip="Change Achievement" />
                                            </div>
                                            <label class="form-label">
                                                Achievement
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="AchievementTitle" />
                                            </div>
                                            <div class="form-text">The achievement granted for successful completion of the items in this gradebook.</div>
                                        </div>
            
                                        <div runat="server" id="OneClassPanel" class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeClass" style="padding:8px" ToolTip="Change Class" />
                                            </div>
                                            <label class="form-label">
                                                Class
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="ClassTitle" />
                                            </div>
                                            <div><asp:Literal runat="server" ID="ClassScheduled" /></div>
                                            <div class="form-text">This class contains the registrations for learners tracked in this gradebook.</div>
                                        </div>

                                        <div runat="server" id="MultiClassPanel" class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeClass2" style="padding:8px" ToolTip="Change Class" />
                                            </div>
                                            <label class="form-label">
                                                Classes
                                            </label>
                                            <div>
                                                <ul>
                                                <asp:Repeater runat="server" ID="ClassRepeater">
                                                    <ItemTemplate>
                                                        <li>
                                                            <a href='<%# Eval("EventIdentifier", "/ui/admin/events/classes/outline?event={0}") %>'>
                                                                <%# Eval("EventTitle") %>
                                                            </a>
                                                        </li>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                </ul>
                                            </div>
                                            <div class="form-text">These classes contain the registrations for learners tracked in this gradebook.</div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangePeriod" style="padding:8px" ToolTip="Change Period" />
                                            </div>
                                            <label class="form-label">
                                                Period
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="PeriodName" />
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Reference
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="Reference" />
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
            
                            <div class="col-md-6">

                                <div class="card h-100">
                                    <div class="card-body">
                            
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Current Status
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="GradebookStatus" />
                                            </div>
                                            <div class="form-text">Changes to a locked gradebook are not permitted.</div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeCheckboxes" style="padding:8px" ToolTip="Change Gradebook's Include Type" />
                                            </div>
                                            <label class="form-label">
                                                Include
                                            </label>
                                            <div>
                                                <asp:CheckBox runat="server" ID="Scores" Enabled="false" Text="Scores" />
                                                <asp:CheckBox runat="server" ID="Standards" Enabled="false" Text="Standards" />
                                            </div>
                                            <div class="form-text">Track scores, standards, or both.</div>
                                        </div>

                                        <div runat="server" id="StandardField" class="form-group mb-3" visible="false">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeFramework" style="padding:8px" ToolTip="Change Gradebook's Framework" />
                                            </div>
                                            <label class="form-label">
                                                Competency Framework
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="FrameworkTitle" />
                                            </div>
                                            <div class="form-text">The framework that contains the standards measured by this gradebook.</div>
                                        </div>

                                    </div>
                                </div>
                            </div>

                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
