<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Records.Programs.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ProgramContactTab" Src="./Controls/ProgramContactTab.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProgramContent" Src="./Controls/ProgramContent.ascx" %>
<%@ Register TagPrefix="uc" TagName="NotificationSetup" Src="./Controls/NotificationSetup.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProgramTaskViewer" Src="./Controls/ProgramTaskViewer.ascx" %>
<%@ Register TagPrefix="uc" TagName="PublicationSetup" Src="./Controls/PublicationSetup.ascx" %>
<%@ Register TagPrefix="uc" TagName="PrivacySettingRepeater" Src="~/UI/Admin/Courses/Outlines/Controls/PrivacySettingsGroups.ascx" %>
<%@ Register TagPrefix="uc" TagName="CredentialGrid" Src="./Controls/CredentialGrid.ascx" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementListEditor.ascx" TagName="AchievementListEditor" TagPrefix="uc" %>
<%@ Register Src="./Controls/TaskGridView.ascx" TagName="TaskGrid" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Nav runat="server">

        <insite:NavItem runat="server" ID="ProgramTab" Title="Program" Icon="far fa-graduation-cap" IconPosition="BeforeText">

            <div class="row mt-4 mb-4">
                <div class="col-lg-12">
                    <insite:Button runat="server" ID="NewButton" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/learning/programs/create" />
                    <insite:ButtonSpacer runat="server" />
                    <insite:Button runat="server" ID="ViewHistoryLink" Visible="false" Text="History" Icon="fas fa-history" ButtonStyle="Default" />
                    <insite:DeleteButton runat="server" ID="DeleteLink" />
                    <insite:ButtonSpacer runat="server" />
                    <insite:Button runat="server" ID="DuplicateLink" ToolTip="Duplicate" ButtonStyle="Default" Text="Duplicate" Icon="far fa-copy" />
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">

                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <h3>Identification</h3>

                            <div class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="RenameLink" CssClass="p-2" ToolTip="Rename program" Name="pencil" />
                                </div>
                                <asp:Label ID="ProgramNameLabel" runat="server" Text="Program Name" CssClass="form-label" AssociatedControlID="ProgramName" />
                                <div>
                                    <asp:Literal runat="server" ID="ProgramName" />
                                </div>
                            </div>
                            <div runat="server" id="ProgramCodeField" class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="RecodeLink" CssClass="p-2" ToolTip="Recode program" Name="pencil" />
                                </div>
                                <asp:Label ID="ProgramCodeLabel" runat="server" Text="Program Code" CssClass="form-label" AssociatedControlID="ProgramCode" />
                                <div>
                                    <asp:Literal runat="server" ID="ProgramCode" />
                                </div>
                            </div>
                            <div runat="server" id="GroupField" class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="ModifyGroupLink" CssClass="p-2" ToolTip="Modify group" Name="pencil" />
                                </div>
                                <asp:Label ID="GroupLabel" runat="server" Text="Group" CssClass="form-label" AssociatedControlID="GroupName" />
                                <div>
                                    <asp:Literal runat="server" ID="GroupName" />
                                </div>
                            </div>
                            <div runat="server" id="ProgramTagField" class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="RetagLink" CssClass="p-2" ToolTip="Retag program" Name="pencil" />
                                </div>
                                <asp:Label ID="ProgramTagLabel" runat="server" Text="Program Tag" CssClass="form-label" AssociatedControlID="ProgramTag" />
                                <div>
                                    <asp:Literal runat="server" ID="ProgramTag" />
                                </div>
                            </div>
                            <div class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="DescribeLink" CssClass="p-2" ToolTip="Describe program" Name="pencil" />
                                </div>
                                <asp:Label ID="ProgramDescriptionLabel" runat="server" Text="Description" CssClass="form-label" AssociatedControlID="ProgramDescription" />
                                <div>
                                    <span style="white-space:pre-wrap;"><asp:Literal runat="server" ID="ProgramDescription" /></span>
                                </div>
                            </div>
                            <div class="form-group mb-3">
                                <asp:Label ID="ProgramIdentifierLabel" runat="server" Text="Program Identifier" CssClass="form-label" AssociatedControlID="ProgramIdentifier" />
                                <div>
                                    <asp:Literal runat="server" ID="ProgramIdentifier" />
                                    <div>
                                        <asp:Literal runat="server" ID="ProgramType" />
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>

                    <div class="card border-0 shadow-lg mt-3">
                        <div class="card-body">

                            <div class="float-end">
                                <insite:IconLink runat="server" ID="EditAchievementLink" CssClass="p-2" ToolTip="Modify achievement" Name="pencil" />
                            </div>

                            <h3>
                                Recognition
                            </h3>

                            <asp:Panel runat="server" ID="NoAchievementMessage" CssClass="alert alert-info" Visible="false">
                                No achievement have been assigned to this program.
                            </asp:Panel>

                            <insite:Container runat="server" id="AchievementFields">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Achievement Name
                                        <insite:IconLink runat="server" id="AchievementOutlineLink" ToolTip="View details for this certificate" Name="external-link-square" Target="_blank" />
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="AchievementTitle" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Achievement Type
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="AchievementLabel" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Achievement Expiration
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="AchievementExpiration" />
                                    </div>
                                </div>

                                <div runat="server" id="AchievementLayoutField" class="form-group mb-3">
                                    <label class="form-label">
                                        Certificate Layout
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="AchievementLayout" />
                                    </div>
                                </div>

                            </insite:Container>

                        </div>
                    </div>

                </div>
                <div class="col-md-6">

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="row">

                                <div class="col-lg-8">
                                    <h3>Tasks</h3>
                                </div>
                                <div class="col-lg-4 text-end">
                                    <insite:Button runat="server" ID="EditButton" Text="Assign Tasks" Icon="fas fa-list-check" ButtonStyle="Success" />
                                </div>
                            </div>
                            
                            <uc:ProgramTaskViewer runat="server" ID="AchievementTaskRepeater" ObjectType="Achievement" Visible="false" />
                            
                            <uc:ProgramTaskViewer runat="server" ID="AssessmentTaskRepeater" ObjectType="Assessment" Visible="false" />
                            <uc:ProgramTaskViewer runat="server" ID="CourseTaskRepeater" ObjectType="Course" Visible="false" />
                            <uc:ProgramTaskViewer runat="server" ID="LogbookTaskRepeater" ObjectType="Logbook" Visible="false" />
                            <uc:ProgramTaskViewer runat="server" ID="SurveyTaskRepeater" ObjectType="Form" Visible="false" />
                            
                        </div>
                    </div>

                </div>
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="CatalogTab" Title="Catalog" Icon="far fa-books" IconPosition="BeforeText">

            <div class="row">
                <div class="col-6">
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="ModifyCatalogLink" CssClass="p-2" ToolTip="Modify catalog" Name="pencil" />
                                </div>
                                <asp:Label runat="server" Text="Catalog" CssClass="form-label" />
                                <div>
                                    <asp:Literal runat="server" ID="CatalogName" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Categories
                                </label>
                                <div class="ms-2">
                                    <asp:Repeater runat="server" ID="CategoriesFolderRepeater">
                                        <ItemTemplate>
                                            <div class="mt-1 mb-2 fs-sm">
                                                <%# Eval("FolderName") %>
                                            </div>

                                            <asp:Repeater runat="server" ID="ItemRepeater">
                                                <HeaderTemplate>
                                                    <div class="ms-2"><ul>
                                                </HeaderTemplate>
                                                <FooterTemplate>
                                                    </ul></div>
                                                </FooterTemplate>
                                                <ItemTemplate>
                                                    <li><%# Eval("ItemName") %></li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:Panel runat="server" ID="NoCatalogCategories" CssClass="alert alert-info" Visible="false">
                                        There are no selected catalog categories.
                                    </asp:Panel>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="ContentTab" Title="Content" Icon="far fa-scroll" IconPosition="BeforeText">

            <uc:ProgramContent runat="server" ID="ProgramContent" />

        </insite:NavItem>

        <insite:NavItem runat="server" ID="LearnerTab" Title="Enrollments" Icon="far fa-users" IconPosition="BeforeText">

            <uc:ProgramContactTab runat="server" ID="ContactTabControl" />

        </insite:NavItem>

        <insite:NavItem runat="server" ID="CredentialPanel" Visible="false" Title="Achievements" Icon="far fa-award" IconPosition="BeforeText">
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

        <insite:NavItem runat="server" ID="AchievementSection" Visible="false" Title="Achievements" Icon="far fa-award" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Achievements
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:achievementlisteditor id="AchievementListEditor" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SettingsSection" Visible="false" Title="Settings" Icon="far fa-cogs" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Settings</h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <div class="float-end">
                            <insite:IconLink runat="server" ID="ModifySettingsLink" CssClass="p-2" ToolTip="Modify settings" Name="pencil" />
                        </div>
                        
                        <uc:TaskGrid runat="server" ID="TaskGrid" />
                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PublicationTab" Title="Publication" Icon="far fa-upload" IconPosition="BeforeText">

            <uc:PublicationSetup runat="server" ID="PublicationSetup" />

        </insite:NavItem>

        <insite:NavItem runat="server" ID="NotificationTab" Title="Notifications" Icon="far fa-bell" IconPosition="BeforeText">

            <uc:NotificationSetup runat="server" ID="NotificationSetup" />

        </insite:NavItem>

        <insite:NavItem runat="server" ID="PrivacyTab" Title="Privacy" Icon="far fa-key" IconPosition="BeforeText">

            <div class="row">
                <div class="col-12">
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">
                            <uc:PrivacySettingRepeater runat="server" ID="PrivacySettingRepeater" />
                        </div>
                    </div>
                </div>
            </div>

        </insite:NavItem>

    </insite:Nav>

</asp:Content>