<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Records.Programs.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ProgramCategoryList" Src="./Controls/ProgramCategoryList.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProgramUserGrid" Src="./Controls/ProgramUserGrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProgramContent" Src="./Controls/ProgramContent.ascx" %>
<%@ Register TagPrefix="uc" TagName="NotificationSetup" Src="./Controls/NotificationSetup.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProgramTaskViewer" Src="./Controls/ProgramTaskViewer.ascx" %>
<%@ Register TagPrefix="uc" TagName="PublicationSetup" Src="./Controls/PublicationSetup.ascx" %>
<%@ Register TagPrefix="uc" TagName="PrivacySettingRepeater" Src="~/UI/Admin/Courses/Outlines/Controls/PrivacySettingsGroups.ascx" %>
<%@ Register TagPrefix="uc" TagName="CredentialGrid" Src="./Controls/CredentialGrid.ascx" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementListEditor.ascx" TagName="AchievementListEditor" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="ExpirationField" Src="~/UI/Admin/Records/Achievements/Controls/AchievementExpirationField.ascx" %>
<%@ Register Src="./Controls/TaskGrid.ascx" TagName="TaskGrid" TagPrefix="uc" %>

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
                    <insite:Button runat="server" id="DuplicateLink" ToolTip="Duplicate" ButtonStyle="Default" Text="Duplicate" Icon="far fa-fw fa-copy" />
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">

                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <h3>Identification</h3>

                            <div class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="RenameLink" Style="padding: 8px" ToolTip="Rename program" Name="pencil" />
                                </div>
                                <asp:Label ID="ProgramNameLabel" runat="server" Text="Program Name" CssClass="form-label" AssociatedControlID="ProgramName" />
                                <div>
                                    <asp:Literal runat="server" ID="ProgramName" />
                                </div>
                            </div>
                            <div runat="server" id="ProgramCodeField" class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="RecodeLink" Style="padding: 8px" ToolTip="Recode program" Name="pencil" />
                                </div>
                                <asp:Label ID="ProgramCodeLabel" runat="server" Text="Program Code" CssClass="form-label" AssociatedControlID="ProgramCode" />
                                <div>
                                    <asp:Literal runat="server" ID="ProgramCode" />
                                </div>
                            </div>
                            <div runat="server" id="GroupField" class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="ModifyGroupLink" Style="padding: 8px" ToolTip="Modify group" Name="pencil" />
                                </div>
                                <asp:Label ID="GroupLabel" runat="server" Text="Group" CssClass="form-label" AssociatedControlID="GroupName" />
                                <div>
                                    <asp:Literal runat="server" ID="GroupName" />
                                </div>
                            </div>
                            <div runat="server" id="ProgramTagField" class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="RetagLink" Style="padding: 8px" ToolTip="Retag program" Name="pencil" />
                                </div>
                                <asp:Label ID="ProgramTagLabel" runat="server" Text="Program Tag" CssClass="form-label" AssociatedControlID="ProgramTag" />
                                <div>
                                    <asp:Literal runat="server" ID="ProgramTag" />
                                </div>
                            </div>
                            <div class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="DescribeLink" Style="padding: 8px" ToolTip="Describe program" Name="pencil" />
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

                            <h3>Recognition</h3>

                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AchievementUpdatePanel" />
                            <insite:UpdatePanel runat="server" ID="AchievementUpdatePanel">
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="AchievementSaveButton" />
                                </Triggers>
                                <ContentTemplate>

                                    <div runat="server" id="AchievementIdentifierField" class="form-group mb-3">
                                        <div class="float-end">
                                            <insite:IconButton runat="server" id="AchievementCreateButton" ToolTip="Add a new certificate" Name="plus-square" />
                                            <insite:IconLink runat="server" id="AchievementOutlineLink" ToolTip="View details for this certificate" Name="external-link-square" Target="_blank" />
                                        </div>
                                        <label class="form-label">Achievement</label>
                                        <div>
                                            <insite:FindAchievement runat="server" ID="AchievementIdentifier" />
                                        </div>
                                    </div>

                                    <insite:Container runat="server" id="AchievementFields">

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Achievement Name
                                                <insite:RequiredValidator runat="server" ControlToValidate="AchievementName" FieldName="Achievement Name" ValidationGroup="Achievement" />
                                            </label>
                                            <div>
                                                <insite:TextBox runat="server" ID="AchievementName" />
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Achievement Type
                                                <insite:RequiredValidator runat="server" ControlToValidate="AchievementLabel" FieldName="Achievement Type" ValidationGroup="Achievement" />
                                            </label>
                                            <div>
                                                <insite:TextBox runat="server" ID="AchievementLabel" />
                                            </div>
                                        </div>

                                        <uc:ExpirationField runat="server" ID="AchievementExpiration" 
                                            LabelText="Achievement Expiration" HelpText="" 
                                            ValidationGroup="Achievement" />

                                        <div class="form-group mb-3">
                                            <label class="form-label">Certificate Layout</label>
                                            <div>
                                                <insite:CertificateLayoutComboBox CssClass="dropup" runat="server" ID="AchievementLayout" />
                                            </div>
                                        </div>

                                    </insite:Container>

                                    <div class="mt-3">
                                        <insite:SaveButton runat="server" ID="AchievementSaveButton" ValidationGroup="Achievement" />
                                        <insite:CancelButton runat="server" ID="AchievementCancelButton" />
                                    </div>

                                </ContentTemplate>
                            </insite:UpdatePanel>

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
                            <uc:ProgramTaskViewer runat="server" ID="SurveyTaskRepeater" ObjectType="Survey" Visible="false" />
                            
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
                                    <insite:IconLink runat="server" ID="ModifyCatalogLink" Style="padding: 8px" ToolTip="Modify catalog" Name="pencil" />
                                </div>
                                <asp:Label runat="server" Text="Catalog" CssClass="form-label" />
                                <div>
                                    <asp:Literal runat="server" ID="CatalogName" />
                                </div>
                            </div>

                            <uc:ProgramCategoryList runat="server" ID="ProgramCategoryList" />

                        </div>
                    </div>
                </div>
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="ContentTab" Title="Content" Icon="far fa-scroll" IconPosition="BeforeText">

            <uc:ProgramContent runat="server" ID="ProgramContent" />

        </insite:NavItem>

        <insite:NavItem runat="server" ID="LearnerTab" Title="Enrollments" Icon="far fa-users" IconPosition="BeforeText">

            <uc:ProgramUserGrid runat="server" ID="ProgramUserGrid" />

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
                        <uc:AchievementListEditor ID="AchievementEditor" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AchievementTaskSection" Visible="false" Title="Settings" Icon="far fa-cogs" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Settings</h2>
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
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

    <div class="mt-4">
        <insite:SaveButton runat="server" ID="SaveAchievementsButton" Visible="false" />
    </div>

</asp:Content>