<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecificationDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Specifications.Controls.SpecificationDetails" %>

<div class="row">
    <div class="col-lg-6">

        <h3>Identification</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Specification Name
                <insite:IconLink Name="trash-alt" runat="server" id="DeleteSpecificationLink" ToolTip="Delete Specification" CssClass="ms-1" />
                <insite:IconLink Name="pencil" runat="server" id="RenameSpecificationLink" ToolTip="Rename Specification" CssClass="ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="SpecificationName" />
            </div>
            <div class="form-text">
                A short, descriptive name that identifies this specification within the bank.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Specification Type
            </label>
            <div>
                <asp:Literal runat="server" ID="SpecificationType" />
            </div>
            <div class="form-text" runat="server" id="SpecificationTypeHelp"></div>
        </div>

        <h3>Configuration</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Form Limit
                <insite:IconLink Name="pencil" runat="server" id="ReconfigureLink" ToolTip="Reconfigure Specification" CssClass="ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="SpecificationFormLimit" />
            </div>
            <div class="form-text">
                Maximum number of forms allowed in this specification.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Question Limit per Form
            </label>
            <div>
                <asp:Literal runat="server" ID="SpecificationQuestionLimit" />
            </div>
            <div class="form-text">
                Maximum number of question items allowed on each form in this specification.
            </div>
        </div>

        <insite:Container runat="server" ID="ScenarioFields">
            <div class="form-group mb-3">
                <label class="form-label">
                    Sections as Tabs
                    <insite:IconButton runat="server" ID="DisableSectionsAsTabsButton" Name="toggle-on" CssClass="ms-1" />
                    <insite:IconButton runat="server" ID="EnableSectionsAsTabsButton" Name="toggle-off" CssClass="ms-1" />
                </label>
                <div>
                    <asp:Literal runat="server" ID="SectionsAsTabsOutput" />
                </div>
                <div class="form-text">
                    If this feature is enabled then each form's section will appear in a tab at the top of the form.
                </div>
            </div>

            <div runat="server" id="TabNavigationField" class="form-group mb-3" visible="false">
                <label class="form-label">
                    Tab Navigation
                    <insite:IconButton runat="server" ID="DisableTabNavigationButton" Name="toggle-on" CssClass="ms-1" />
                    <insite:IconButton runat="server" ID="EnableTabNavigationButton" Name="toggle-off" CssClass="ms-1" />
                </label>
                <div>
                    <asp:Literal runat="server" ID="TabNavigationOutput" />
                </div>
                <div class="form-text">
                    When enabled, Next and Previous buttons are displayed and learner can freely move around tabs.
                    When disabled, only Next button is displayed and only forward progression through the assessment is allowed.
                </div>
            </div>

            <div runat="server" id="SingleQuestionPerTabField" class="form-group mb-3" visible="false">
                <label class="form-label">
                    Single Question per Tab
                    <insite:IconButton runat="server" ID="DisableSingleQuestionPerTabButton" Name="toggle-on" CssClass="ms-1" />
                    <insite:IconButton runat="server" ID="EnableSingleQuestionPerTabButton" Name="toggle-off" CssClass="ms-1" />
                </label>
                <div>
                    <asp:Literal runat="server" ID="SingleQuestionPerTabOutput" />
                </div>
                <div class="form-text">
                    If this feature is enabled then only one question is displayed on the current tab at a time.
                </div>
            </div>

            <div runat="server" id="TabTimeLimitField" class="form-group mb-3" visible="false">
                <label class="form-label">
                    Tab Time Limit
                </label>
                <div>
                    <asp:Literal runat="server" ID="TabTimeLimitOutput" />
                </div>
            </div>

        </insite:Container>

    </div>
    <div class="col-lg-6">

        <h3>Scoring Calculation</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Disclosure Type
                <insite:IconLink Name="pencil" runat="server" id="ChangeCalculationLink" ToolTip="Change Scoring Calculation" CssClass="ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="SpecificationCalculationDisclosure" />
            </div>
            <div class="form-text">
                What information is disclosed to a student/candidate after completing an exam submission?
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Passing Score
            </label>
            <div>
                <asp:Literal runat="server" ID="SpecificationCalculationPassingScore" />
            </div>
            <div class="form-text">
                What is the minimum score required to pass the exam?
            </div>
        </div>

    </div>
</div>