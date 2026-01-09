<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="Controls/FieldGrid.ascx" TagName="FieldGrid" TagPrefix="uc" %>
<%@ Register Src="Controls/CompetencyGrid.ascx" TagName="CompetencyGrid" TagPrefix="uc" %>
<%@ Register Src="Controls/UserGrid.ascx" TagName="UserGrid" TagPrefix="uc" %>
<%@ Register Src="Controls/GroupGrid.ascx" TagName="GroupGrid" TagPrefix="uc" %>
<%@ Register Src="Controls/AchievementGrid.ascx" TagName="AchievementGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="UsersPanel" Title="Learners" Icon="far fa-users" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Learners
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-lg-7">
                                <insite:Button runat="server" ID="AddUsers" Text="Add Learners" Icon="fas fa-plus-circle" ButtonStyle="Default" CssClass="me-1" />
                                <insite:Button runat="server" ID="GrantButton" Text="Issue Achievements" Icon="fas fa-award" ButtonStyle="Default" />
                            </div>
                            <div class="col-lg-5">
                                <insite:InputSearch runat="server" ID="SearchInput" SubmitOnEnter="true" />
                            </div>
                        </div>

                        <div runat="server" id="PeoplePanel">
                            <h4>People</h4>
                            <uc:UserGrid runat="server" ID="Users" />
                        </div>

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="GroupUpdatePanel" />
                        <insite:UpdatePanel runat="server" ID="GroupUpdatePanel">
                            <ContentTemplate>
                                <div runat="server" id="GroupPanel">
                                    <h4>Groups</h4>
                                    <uc:GroupGrid runat="server" ID="Groups" />
                                </div>
                            </ContentTemplate>
                        </insite:UpdatePanel>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="FieldsPanel" Title="Fields" Icon="far fa-list" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Fields
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:FieldGrid runat="server" ID="Fields" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CompetenciesPanel" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Competencies
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:CompetencyGrid runat="server" ID="Competencies" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="AchievementsPanel" Title="Achievements" Icon="far fa-award" IconPosition="BeforeText">
            <insite:Alert runat="server" ID="NoAchievements" />

            <section runat="server" id="AchievementsInnerPanel">
                <h2 class="h4 mt-4 mb-3">
                    Achievements
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:AchievementGrid runat="server" ID="Achievements" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="LogbookPanel" Title="Logbook Template" Icon="far fa-pencil-ruler" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Logbook Template
                </h2>

                <div class="row button-group mb-3">
                    <div class="col-lg-12">
                        <insite:Button runat="server" ID="NewLogbookLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/records/logbooks/open" />

                        <insite:ButtonSpacer runat="server" />

                        <insite:Button runat="server" ID="LockUnlockLogbookButton" Text="Lock" Icon="fas fa-lock" ButtonStyle="Default" />
                        <insite:Button runat="server" ID="ViewHistoryLink" Text="History" Icon="fas fa-history" ButtonStyle="Default" />
                        <insite:DeleteButton runat="server" ID="DeleteLink" />
                    </div>
                </div>


                <div class="row">
                        
                    <div class="col-md-6">
                                    
                        <div class="card border-0 shadow-lg mb-3">
                            <div class="card-body">

                                <h3>Details</h3>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" id="Rename" style="padding:8px" ToolTip="Rename Logbook" />
                                    </div>
                                    <label class="form-label">
                                        Logbook Name
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="JournalSetupName" />
                                    </div>
                                    <div class="form-text">The name that uniquely identifies the logbook for internal filing purposes.</div>
                                </div>
            
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" id="ChangeClass" style="padding:8px" ToolTip="Change Logbook's Class" />
                                    </div>
                                    <label class="form-label">
                                        Class
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="ClassTitle" />
                                    </div>
                                    <div><asp:Literal runat="server" ID="ClassScheduled" /></div>
                                    <div class="form-text">The class that includes the list of registered students.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" id="ChangeAchievement" style="padding:8px" ToolTip="Change Logbook's Achievement" />
                                    </div>
                                    <label class="form-label">
                                        Achievement
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="AchievementTitle" />
                                    </div>
                                    <div class="form-text">The achievement issued for successful completion of required items in this Logbook.</div>
                                </div>

                               <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" id="ChangeIsRecordDownloadable" style="padding:8px" ToolTip="Change is Logbook Downloadable" />
                                    </div>
                                    <label class="form-label">
                                        Can Learner download this record.
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="IsDownloadable" />
                                    </div>
                                    <div class="form-text"></div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" id="ChangeFramework" style="padding:8px" ToolTip="Change Logbooks's Framework" />
                                    </div>
                                    <label class="form-label">
                                        Framework
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="FrameworkTitle" />
                                    </div>
                                    <div class="form-text">The framework that contains the standards measured by this logbook.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" id="ChangeIsValidationRequired" style="padding:8px" ToolTip="Change Validation's Requirement" />
                                    </div>
                                    <label class="form-label">
                                        Is Validation Required for this Logbook?
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="IsValidationRequired" />
                                    </div>
                                    <div class="form-text"></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Logbook Identifier
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="JournalSetupIdentifierLiteral" />
                                    </div>
                                    <div class="form-text">A globally unique identifier for this logbook.</div>
                                </div>

                            </div>
                        </div>

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <h3>Notifications</h3>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" id="ChangeValidatorMessage" style="padding:8px" ToolTip="Change Notification Message (Validator)" />
                                    </div>
                                    <label class="form-label">
                                        Notification Message (Validator)
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="ValidatorMessageName" />
                                    </div>
                                    <div class="form-text">The message that sent to validator(s) when the learner updates or comments the logbook.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" id="ChangeLearnerMessage" style="padding:8px" ToolTip="Change Notification Message (Learner)" />
                                    </div>
                                    <label class="form-label">
                                        Notification Message (Learner)
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="LearnerMessageName" />
                                    </div>
                                    <div class="form-text">The message that sent to the learner when the validator updates or comments the logbook.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" id="ChangeLearnerAddedMessage" style="padding:8px" ToolTip="Change Notification Message (Learner Added)" />
                                    </div>
                                    <label class="form-label">
                                        Notification Message (Learner Added)
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="LearnerAddedMessageName" />
                                    </div>
                                    <div class="form-text">The message that sent to the learner when added to the logbook.</div>
                                </div>

                            </div>
                        </div>
                    </div>
            
                    <div class="col-md-6">
                            
                        <div class="card border-0 shadow-lg mb-3">
                            <div class="card-body">

                                <h3>Content</h3>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" id="ChangeTitle" style="padding:8px" ToolTip="Change Logbook Title" />
                                    </div>
                                    <label class="form-label">
                                        Logbook Title
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="TitleOutput" />
                                    </div>
                                    <div class="form-text">A descriptive user-friendly title for the logbook.</div>
                                </div>

                            </div>
                        </div>

                        <div class="card border-0 shadow-lg mb-3">
                            <div class="card-body">

                                <h3>
                                    Instructions
                                    <insite:IconLink Name="pencil" runat="server" id="ChangeInstructions" style="padding:2px 8px;font-size:16px;float:right;" ToolTip="Change Instructions" />
                                </h3>

                                <div class="form-group mb-3">
                                    <div class="form-text">This information guides the learner in how to complete the logbook.</div>
                                    <div>
                                        <asp:Literal runat="server" ID="Instructions" />
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <h3>Contacts</h3>

                                <div class="row">
                                    <div class="col-lg-6">
                                        <b>Validators</b>
                                    </div>
                                    <div class="col-lg-6" style="text-align:right;">
                                        <insite:Button runat="server" id="AssignValidators" ButtonStyle="Success" Text="Assign Validators" Icon="far fa-plus-circle" />
                                    </div>
                                </div>
                                <div class="row" style="padding-top:15px;">
                                    <div class="col-lg-12">
                                        <asp:Repeater runat="server" ID="ValidatorRepeater">
                                            <HeaderTemplate>
                                                <table class="table table-striped">
                                                    <tbody>
                                            </HeaderTemplate>
                                            <FooterTemplate>
                                                </tbody></table>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <a href="/ui/admin/contacts/people/edit?contact=<%# Eval("UserIdentifier") %>">
                                                            <%# Eval("UserFullName") %>
                                                        </a>
                                                    </td>
                                                    <td style="width:40px;text-align:right;">
                                                        <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete"
                                                            NavigateUrl='<%# string.Format("/ui/admin/records/logbooks/validators/delete?journalsetup={0}&user={1}", JournalSetupIdentifier, Eval("UserIdentifier")) %>' />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
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
