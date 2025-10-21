<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigurationDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Specifications.Controls.ConfigurationDetails" %>

<h3>Configuration</h3>

<div class="form-group mb-3">
    <label class="form-label">
        Consequence Type
    </label>
    <div>
        <asp:RadioButtonList runat="server" ID="ConsequenceType">
            <asp:ListItem Value="High" />
            <asp:ListItem Value="Medium" />
            <asp:ListItem Value="Low" />
        </asp:RadioButtonList>
    </div>
    <div class="form-text">
        Indicates the stakes for exam forms following this specification.
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Form Limit
        <insite:RequiredValidator runat="server" ControlToValidate="FormLimit" ValidationGroup="Assessment" />
    </label>
    <div>
        <insite:NumericBox runat="server" ID="FormLimit" NumericMode="Integer" MinValue="0" />
    </div>
    <div class="form-text">
        The maximum number of forms allowed in this specification.
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Question Item Limit
        <insite:RequiredValidator runat="server" ControlToValidate="QuestionLimit" ValidationGroup="Assessment" />
    </label>
    <div>
        <insite:NumericBox runat="server" ID="QuestionLimit" NumericMode="Integer" MinValue="0" />
    </div>
    <div class="form-text">
        The maximum number of questions allowed on each exam form.
    </div>
</div>

<insite:Container runat="server" ID="ScenarioFields">
    <div class="form-group mb-3">
        <label class="form-label">
            Sections as Tabs
        </label>
        <div>
            <insite:BooleanComboBox runat="server" ID="SectionsAsTabsEnabled" TrueText="Enabled" FalseText="Disabled" AllowBlank="false" />
        </div>
        <div class="form-text">
            If this feature is enabled then each form's section will appear in a tab at the top of the form.
        </div>
    </div>

    <div runat="server" id="TabNavigationField" class="form-group mb-3">
        <label class="form-label">
            Tab Navigation
        </label>
        <div>
            <insite:BooleanComboBox runat="server" ID="TabNavigationEnabled" TrueText="Enabled" FalseText="Disabled" AllowBlank="false" />
        </div>
        <div class="form-text">
            When enabled, Next and Previous buttons are displayed and learner can freely move around tabs.
            When disabled, only Next button is displayed and only forward progression through the assessment is allowed.
        </div>
    </div>

    <div runat="server" ID="SingleQuestionPerTabField" class="form-group mb-3">
        <label class="form-label">
            Single Question per Tab
        </label>
        <div>
            <insite:BooleanComboBox runat="server" ID="SingleQuestionPerTabEnabled" TrueText="Enabled" FalseText="Disabled" AllowBlank="false" />
        </div>
        <div class="form-text">
            If this feature is enabled then only one question is displayed on the current tab at a time.
        </div>
    </div>

    <div runat="server" ID="TabTimeLimitField" class="form-group mb-3">
        <label class="form-label">
            Tab Time Limit
        </label>
        <div>
            <insite:ComboBox runat="server" ID="TabTimeLimit" AllowBlank="false" />
        </div>
    </div>
</insite:Container>
