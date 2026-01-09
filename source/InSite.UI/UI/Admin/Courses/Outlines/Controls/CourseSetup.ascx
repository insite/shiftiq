<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourseSetup.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.CourseSetup" %>

<%@ Register Src="./PrivacySettingsGroups.ascx" TagName="PrivacySettingsGroups" TagPrefix="uc" %>
<%@ Register Src="./CourseFileList.ascx" TagName="CourseFileList" TagPrefix="uc" %>
<%@ Register Src="./CourseCategoryList.ascx" TagName="CourseCategoryList" TagPrefix="uc" %>

<insite:Alert runat="server" ID="CourseSetupAlert" />

<insite:Nav runat="server">
            
    <insite:NavItem runat="server" ID="DetailsTab" Title="Course Details">

        <div class="row">
            <div class="col-lg-4">

                <div class="card h-100">
                    <div class="card-body">

                        <h3>Identification</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Course Name
                                <insite:RequiredValidator runat="server" ID="CourseNameValidator" ControlToValidate="CourseName" ValidationGroup="CourseSetup" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="CourseName" MaxLength="200" />
                            </div>
                            <div class="form-text">
        
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Course Code</label>
                            <div>
                                <insite:TextBox runat="server" ID="CourseCode" MaxLength="30" />
                            </div>
                            <div class="form-text">
                                An alphanumeric code used to identify the course in a catalog.
                            </div>
                        </div>
        
                        <div class="form-group mb-3">
                            <label class="form-label">Course Tag</label>
                            <div>
                                <insite:TextBox runat="server" ID="CourseLabel" MaxLength="20" />
                            </div>
                            <div class="form-text">
                                An <i>optional</i> term to categorize the course.
                            </div>
                        </div>

                        <div class="form-group mb-3">

                            <div class="float-end">
                                <span class="badge bg-custom-default">Asset # <asp:Literal runat="server" ID="CourseAsset" /></span>
                            </div>
                            <label class="form-label">
                                Course Identifier
                            </label>
                            <div>
                                <div class="float-end">
                                    <insite:IconLink runat="server" ID="DeleteLink" ToolTip="Delete this course" Name="trash-alt" />
                                </div>
                                <asp:Literal runat="server" ID="CourseThumbprint" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>
            <div class="col-lg-4">

                <div class="card">
                    <div class="card-body">

                        <h3>Completion</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Requirements
                            </label>
                            <div>
                                <asp:Label runat="server" ID="RequirementCount" />
                                <asp:Repeater runat="server" ID="RequirementRepeater">
                                    <HeaderTemplate>
                                        <ul>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <%# Eval("Count") %>
                                            <%# Eval("Type") %>
                                        </li>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Course completed on this Activity
                            </label>
                            <div>
                                <insite:ActivityComboBox runat="server" ID="CompletionActivityIdentifier" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>
            <div class="col-lg-4">

                <div class="card h-100">
                    <div class="card-body">

                        <h3>Settings</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Options</label>
                            <div>
                                <asp:CheckBox runat="server" ID="IsMultipleUnitsEnabled" Text="Enable Multiple Units" />
                            </div>
                            <div>
                                <asp:CheckBox runat="server" ID="IsProgressReportEnabled" Text="Enable Progress Report" />
                            </div>
                            <div>
                                <asp:CheckBox runat="server" ID="AllowDiscussion" Text="Allow Learner Discussion" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Outline Width</label>
                            <div>
                                <asp:RadioButton runat="server" ID="OutlineWidth3" GroupName="OutlineWidth" Text="Narrow" />
                                <asp:RadioButton runat="server" ID="OutlineWidth4" GroupName="OutlineWidth" Text="Default" Checked="true" />
                                <asp:RadioButton runat="server" ID="OutlineWidth5" GroupName="OutlineWidth" Text="Wide" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Course Hook
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="CourseHook" MaxLength="100" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>

        </div>
        
    </insite:NavItem>

    <insite:NavItem runat="server" Title="Content">

        <div class="row">
            <div class="col-lg-6 mb-md-3">

                <div class="card h-100">
                    <div class="card-body">

                        <h3>Content</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Language</label>
                            <div>
                                <insite:ComboBox runat="server" ID="Language" />
                            </div>
                        </div>

                        <div class="row row-translate">
                            <div class="col-md-12">
                                <asp:Repeater runat="server" ID="ContentRepeater">
                                    <ItemTemplate>
                                        <div class="form-group mb-3">
                                            <insite:DynamicControl runat="server" ID="Container" />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                    </div>
                </div>

            </div>
            <div class="col-lg-6">

                <div class="card h-100">
                    <div class="card-body">

                        

                    </div>
                </div>

            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Style">

        <div class="card">
            <div class="card-body">

                <div class="form-group mb-3">
                    <label class="form-label">Cascading Style Sheet (CSS) Rules</label>
                    <div>
                        <insite:TextBox runat="server" ID="CourseStyle" TextMode="MultiLine" Rows="20" AllowHtml="true" />
                    </div>
                </div>

            </div>
        </div>
        
    </insite:NavItem>

    <insite:NavItem runat="server" ID="PublicationsTab" Title="Publication">

        <div class="row">
            <div class="col-lg-4">

                <div class="card h-100">
                    <div class="card-body">

                        <h3>Course Settings</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Course Slug (URL Segment)
                                <insite:RequiredValidator runat="server" ControlToValidate="CourseSlug" ValidationGroup="CourseSetup" />
                            </label>
                            <div>
                                <insite:TextBox ID="CourseSlug" runat="server" MaxLength="100" Width="100%" />
                            </div>
                            <div class="form-text">
                                The part of the URL that specifically refers to this course.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <div runat="server" id="CourseIconPreview" class="float-end"></div>
                            <label class="form-label">
                                Course Icon
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="CourseIcon" MaxLength="30" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="CourseImageField">
                            <label class="form-label">
                                Course Image
                            </label>

                            <div style="position:relative;">
                                <asp:Image runat="server" ID="CourseImage" CssClass="img-responsive" />
                                <div style="position:absolute;top:0px;right:0px;">
                                    <insite:IconButton runat="server" ID="DeleteImage" Name="trash-alt" Type="Solid" ToolTip="Delete this image"/>
                                </div>
                            </div>
                        </div>

                        <div class="mb-3">
                            <insite:FileUploadV2 runat="server" ID="CourseImageUploadV2" LabelText="Upload New Course Image" FileUploadType="Image" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Course Image URL Override
                            </label>
                            <insite:TextBox runat="server" ID="CourseImageUrl" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">
                                Custom Flag Color
                            </label>
                            <div>
                                <insite:ColorComboBox runat="server" ID="CourseFlagColor" CssClass="w-25" />
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">
                                Custom Flag Text
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="CourseFlagText" MaxLength="50" />
                            </div>
                        </div>
                    </div>

                </div>

            </div>
            <div class="col-lg-4">

                <div class="card">
                    <div class="card-body">

                        <h3>Portal Page Settings</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Web Portal
                            </label>
                            <insite:WebSiteComboBox runat="server" ID="WebSiteIdentifier" AllowBlank="true" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Web Folder
                            </label>
                            <insite:WebFolderComboBox runat="server" ID="WebFolderIdentifier" AllowBlank="true" />
                        </div>

                        <div runat="server" id="WebPagePanel" visible="false">

                            <div class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconButton runat="server" ID="WebPageIdentifierAdd" Name="plus-circle" Type="Solid" CausesValidation="false" />
                                    <span runat="server" id="WebPageIdentifierLinks">
                                        <a runat="server" id="WebPageIdentifierEdit" href="#" target="_blank"><i class="icon fas fa-pencil"></i></a>
                                        <a runat="server" id="WebPageIdentifierView" href="#" target="_blank"><i class="icon fas fa-external-link-square"></i></a>
                                    </span>
                                </div>
                                <label class="form-label">
                                    Web Page
                                </label>
                                <div>
                                    <insite:WebPageComboBox runat="server" ID="WebPageIdentifier" AllowBlank="true" />
                                </div>
                                <div runat="server" id="WebPageHelp" class="form-text"></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Web Page Status
                                </label>
                                <div>
                                    <asp:CheckBox runat="server" ID="PublicationStatus" Text="Published" />
                                </div>
                            </div>

                            <div runat="server" id="PublishedInfoPanel" class="form-group mb-3">
                                <label class="form-label">
                                    Published By
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="PublishedBy" />
                                </div>
                                <div>
                                    <asp:Literal runat="server" ID="PublishedDate" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label fw-light">Close Date</label>
                                <insite:DateTimeOffsetSelector runat="server" ID="CourseClosed" />
                            </div>

                        </div>

                    </div>
                </div>

            </div>
            <div class="col-lg-4">

                <div class="card">
                    <div class="card-body">

                        <h3>Catalog Settings</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Catalog
                            </label>
                            <insite:CatalogComboBox runat="server" ID="CatalogIdentifier" AllowBlank="true" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Sequence in Catalog
                            </label>
                            <insite:NumericBox runat="server" ID="CatalogSequence" NumericMode="Integer" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Settings
                            </label>
                            <insite:CheckBox runat="server" ID="CourseIsHidden" Text="Hidden" />
                        </div>

                        <uc:CourseCategoryList runat="server" ID="CategoryList" />

                    </div>
                </div>
            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" ID="FilesTab" Title="Files">
        <div class="card">
            <div class="card-body">

                <uc:CourseFileList runat="server" ID="FileList" />

            </div>
        </div>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="RecordsTab" Title="Records">

        <div class="row">
            <div class="col-lg-4">

                <div class="card h-100">
                    <div class="card-body">

                        <div runat="server" ID="MissingGradebookPanel" class="alert alert-warning">
                            <p class="mb-2">
                                Records (including scores) are not currently tracked for learners in this course. 
                            </p>
                            <p class="mb-2">
                                Would you like to create a new gradebook to track progress, 
                                or select an existing gradebook already set up for your course?
                            </p>
                            <asp:RadioButtonList runat="server" ID="MissingGradebookRadio" AutoPostBack="true">
                                <asp:ListItem Value="Create" Text="Create Gradebook" />
                                <asp:ListItem Value="Select" Text="Select Gradebook" />
                            </asp:RadioButtonList>
                        </div>

                        <h3 runat="server" id="GradebookHeading">
                            Progress
                            <span class="text-muted fs-sm">(Gradebook Settings)</span>
                        </h3>

                        <div runat="server" id="GradebookIdentifierField" class="form-group mb-3">
                            <div class="float-end">
                                <insite:IconButton runat="server" id="GradebookCreateButton" ToolTip="Add a new gradebook" Name="plus-square" />
                                <insite:IconLink runat="server" id="GradebookOutlineLink" ToolTip="View details for this gradebook" Name="external-link-square" Target="_blank" />
                            </div>
                            <label class="form-label">
                                Gradebook
                            </label>
                            <div>
                                <insite:GradebookComboBox runat="server" ID="GradebookIdentifier" />
                            </div>
                        </div>

                        <div runat="server" id="GradebookNameField" class="form-group mb-3">
                            <label class="form-label">Gradebook Name</label>
                            <div>
                                <insite:TextBox runat="server" ID="GradebookName" />
                            </div>
                            <div class="form-text">
                                Tracks learner progress and scores in the course
                            </div>
                        </div>

                        <div runat="server" id="FinalGradeItemIdentifierField" class="form-group mb-3" visible="false">
                            <label class="form-label">Grade Item for Final Score</label>
                            <div>
                                <insite:GradebookItemComboBox runat="server" ID="FinalGradeItemIdentifier" Enabled="false" />
                            </div>
                        </div>

                        <div runat="server" id="FinalGradeItemPassPercentField" class="form-group mb-3" visible="false">
                            <label class="form-label">Passing Score (%)</label>
                            <div>
                                <insite:NumericBox runat="server" ID="FinalGradeItemPassPercent" NumericMode="Integer" MinValue="0" MaxValue="100" Width="80" style="text-align:right;" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>
            <div runat="server" id="AchievementPanel" class="col-lg-4">

                <div class="card h-100">
                    <div class="card-body">

                        <h3 runat="server" id="H1">
                            Recognition
                            <span class="text-muted fs-sm">(Achievement Settings)</span>
                        </h3>

                        <div runat="server" id="AchievementIdentifierField" class="form-group mb-3">
                            <div class="float-end">
                                <insite:IconButton runat="server" id="AchievementCreateButton" ToolTip="Add a new certificate" Name="plus-square" />
                                <insite:IconLink runat="server" id="AchievementOutlineLink" ToolTip="View details for this certificate" Name="external-link-square" Target="_blank" />
                            </div>
                            <label class="form-label">Achievement</label>
                            <div>
                                <insite:FindAchievement runat="server" ID="AchievementIdentifier" />
                            </div>
                            <div runat="server" id="AchievementWarning" class="alert alert-warning mt-2" role="alert">
                                You should not modify the achievement for a gradebook with learners who are now enrolled.
                            </div>
                        </div>

                        <div runat="server" id="AchievementFields">

                        <div class="form-group mb-3">
                            <label class="form-label">Achievement Name</label>
                            <div>
                                <insite:TextBox runat="server" ID="AchievementName" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Achievement Type/Tag</label>
                            <div>
                                <insite:TextBox runat="server" ID="AchievementLabel" />
                            </div>
                            <div class="form-text">Displayed below the achievement title for learners</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Achievement Expiration</label>
                            <div>
                                <asp:Literal runat="server" ID="AchievementExpiration" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Certificate Layout</label>
                            <div>
                                <insite:CertificateLayoutComboBox runat="server" ID="AchievementLayout" />
                            </div>
                        </div>

                        </div>

                    </div>
                </div>
            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" ID="EnrollmentsTab" Title="Enrollments">

        <div class="card">
            <div class="card-body">

                <div runat="server" ID="EnrollmentsMissingGradebookPanel" class="alert alert-warning">
                    <p>
                        Remember to create a gradebook to enable learner enrollment and progress tracking for this course.
                    </p>
                </div>

                <div runat="server" ID="EnrollmentsPanel">

                    <div class="d-flex justify-content-between mb-3 flex-column flex-xl-row">
                        <div class="flex-fill mb-2 mb-xl-0" style="max-width:660px;">
                            <div class="d-flex">
                                <div class="flex-grow-1">
                                    <insite:FindPerson runat="server" ID="EnrollmentPersonIdentifier" EmptyMessage="Learner" />
                                </div>
                                <div class="ms-2">
                                    <insite:Button runat="server" ID="EnrollmentPersonAdd" Size="Default" CssClass="align-baseline"
                                        Icon="fas fa-plus-circle" Text="Add Learner" ButtonStyle="OutlinePrimary" />
                                </div>
                            </div>
                        </div>
                        <div class="flex-fill ms-xl-3 mb-2 mb-xl-0" style="max-width:660px;">
                            <div class="d-flex">
                                <div class="flex-grow-1">
                                    <insite:FindEvent runat="server" ID="EnrollmentClassIdentifier" ShowPrefix="false" EmptyMessage="Class" />
                                </div>
                                <div class="ms-2">
                                    <insite:Button runat="server" ID="EnrollmentClassAdd" Size="Default" CssClass="align-baseline"
                                        Icon="fas fa-plus-circle" Text="Add Class" ButtonStyle="OutlinePrimary" />
                                </div>
                            </div>
                        </div>
                        <div class="flex-fill ms-xl-3 mb-2 mb-xl-0" style="max-width:660px;">
                            <div class="d-flex">
                                <div class="flex-grow-1">
                                    <insite:FindGroup runat="server" ID="EnrollmentGroupIdentifier" ShowPrefix="false" EmptyMessage="Group" CurrentOrganizationOnly="True" />
                                </div>
                                <div class="ms-2">
                                    <insite:Button runat="server" ID="EnrollmentGroupAdd" Size="Default" CssClass="align-baseline"
                                        Icon="fas fa-plus-circle" Text="Add Group" ButtonStyle="OutlinePrimary" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="ExistedEnrollmentsPanel" class="row">
                        <div class="col-lg-12">
                            <div id="CommandButtons" runat="server" style="padding-bottom: 10px;">
                                <insite:Button runat="server" ID="SelectAllButton" ButtonStyle="Default" ToolTip="Select All" style="padding:5px 8px;" Icon="far fa-square" />
                                <insite:Button runat="server" ID="UnselectAllButton" ButtonStyle="Default" ToolTip="Deselect All" style="display:none; padding:5px 8px;" Icon="far fa-check-square" />
                                <insite:Button runat="server" ID="PreDeleteButton" ButtonStyle="Default" ToolTip="Delete Selected Enrollments" style="padding:5px 8px;" Icon="fas fa-trash-alt" />
                            </div>

                            <asp:DataList runat="server" ID="EnrollmentList" RepeatColumns="6" RepeatDirection="Vertical" CssClass="table table-striped table-bordered">
                                <ItemTemplate>
                                    <asp:Literal runat="server" ID="LearnerIdentifier" Text='<%# Eval("LearnerIdentifier") %>' Visible="false" />
                                    <asp:CheckBox runat="server" ID="IsSelected" Text='<%# Eval("LearnerName") %>' />
                                </ItemTemplate>
                            </asp:DataList>
                        </div>
                    </div>

                </div>

            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Competencies">

        <div class="row">
            <div class="col-lg-6">

                <div class="card">
                    <div class="card-body">

                        <h3>Framework</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Competency Framework</label>
                            <div>
                                <insite:FindStandard runat="server" ID="FrameworkIdentifier" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Privacy">
        <div class="row">
            <div class="col-md-6">
                <div class="card h-100">
                    <div class="card-body">
                        <uc:PrivacySettingsGroups runat="server" ID="PrivacySettingsGroups" />
                    </div>
                </div>
            </div>
        </div>
    </insite:NavItem>

</insite:Nav>

<div class="mt-3">
    <insite:SaveButton runat="server" ID="CourseSaveButton" ValidationGroup="CourseSetup" />
    <insite:CancelButton runat="server" ID="CourseCancelButton" />
</div>

<asp:Button runat="server" ID="DeleteEnrollmentsButton" style="display:none;" />

<insite:PageFooterContent runat="server">
<script type="text/javascript">

    (function () {
        Sys.Application.add_load(onLoad);

        function onLoad() {
            $("#<%= PreDeleteButton.ClientID %>")
                .off('click', onDeleteCompetencies)
                .on('click', onDeleteCompetencies);

            $("#<%= SelectAllButton.ClientID %>")
                .off('click', onSelectAll)
                .on('click', onSelectAll);

            $("#<%= UnselectAllButton.ClientID %>")
                .off('click', onUnselectAll)
                .on('click', onUnselectAll);
        }

        function onDeleteCompetencies() {
            if ($("#<%= ExistedEnrollmentsPanel.ClientID %> input[id$='IsSelected']:checked").length > 0) {
                if (confirm('Are you sure you want to delete selected enrollments?'))
                    __doPostBack('<%= DeleteEnrollmentsButton.UniqueID %>', '');
            }
            else {
                alert("Please select the enrollments you want to delete.");
            }

            return false;
        }

        function onSelectAll() {
            $('#<%= UnselectAllButton.ClientID %>').css('display', '');
            $('#<%= SelectAllButton.ClientID %>').css('display', 'none');
            return setCheckboxes('<%= ExistedEnrollmentsPanel.ClientID %>', true);
        }

        function onUnselectAll() {
            $('#<%= UnselectAllButton.ClientID %>').css('display', 'none');
            $('#<%= SelectAllButton.ClientID %>').css('display', '');
            return setCheckboxes('<%= ExistedEnrollmentsPanel.ClientID %>', false);
        }
    })();

</script>
</insite:PageFooterContent>