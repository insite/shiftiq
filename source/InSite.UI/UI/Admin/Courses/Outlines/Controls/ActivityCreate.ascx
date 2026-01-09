<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityCreate.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.ActivityCreate" %>

<div style="padding: 15px 0;">

    <insite:ValidationSummary runat="server" ValidationGroup="ActivityCreate" />
    <div runat="server" id="ActivityCreateError" class="alert alert-danger" visible="false"></div>

    <h3><asp:Literal runat="server" ID="ActivityCreateHeading" /></h3>

    <div runat="server" ID="AssessmentInfo" visible="false" class="alert d-flex alert-info">
        You can also configure a new assessment in the&nbsp;
        <a target="_blank" href="/ui/admin/assessments/banks/create">Assessments toolkit</a>
    </div>

    <div class="row">
        <div class="col-md-12">

            <div runat="server" id="UnitNameField" class="form-group mb-3">
                <label class="form-label">
                    Unit Name
                    <insite:RequiredValidator runat="server" FieldName="Unit Name" ControlToValidate="UnitName" ValidationGroup="ActivityCreate" />
                </label>
                <div>
                    <insite:TextBox runat="server" ID="UnitName" MaxLength="200" />
                </div>
            </div>

            <div runat="server" id="ModuleNameField" class="form-group mb-3">
                <label class="form-label">
                    Module Name
                    <insite:RequiredValidator runat="server" FieldName="Module Name" ControlToValidate="ModuleName" ValidationGroup="ActivityCreate" />
                </label>
                <div>
                    <insite:TextBox runat="server" ID="ModuleName" MaxLength="200" />
                </div>
            </div>

            <div runat="server" id="ActivityNameField" class="form-group mb-3">
                <label class="form-label">
                    <asp:Literal runat="server" ID="ActivityNameLabel" />
                    <insite:RequiredValidator runat="server" FieldName="Activity Name" ControlToValidate="ActivityName" ValidationGroup="ActivityCreate" />
                </label>
                <div>
                    <insite:TextBox runat="server" ID="ActivityName" MaxLength="200" />
                </div>
            </div>

            <div runat="server" id="AssessmentField" class="form-group mb-3">
                <label class="form-label">
                    Assessment Settings
                </label>
                <div>

                    <table class="table">
                        <tr>
                            <td colspan="2">
                                <asp:RadioButtonList runat="server" ID="AssessmentType" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                    <asp:ListItem Value="New" Text="Add New Form &nbsp; " Selected="true" />
                                    <asp:ListItem Value="Existing" Text="Select Existing Form" />
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr runat="server" id="AssessmentFormField">
                            <td style="width: 250px">Assessment Form
                                <insite:RequiredValidator runat="server" ID="AssessmentFormValidator" FieldName="Assessment Form" ControlToValidate="AssessmentFormIdentifier" ValidationGroup="ActivityCreate" />
                            </td>
                            <td>
                                <insite:FindBankForm runat="server" ID="AssessmentFormIdentifier" />
                                <div runat="server" id="AssessmentFormError" class="alert alert-danger mt-3" visible="false"></div>
                            </td>
                        </tr>
                        <tr runat="server" id="QuestionCountField">
                            <td class="text-nowrap" style="width:0;">
                                Number of Multiple-Choice Questions
                                <insite:RequiredValidator runat="server" ID="QuestionCountValidator" FieldName="Number of Questions" ControlToValidate="QuestionCount" ValidationGroup="ActivityCreate" />
                            </td>
                            <td>
                                <insite:NumericBox runat="server" ID="QuestionCount" MinValue="1" ValueAsInt="10" NumericMode="Integer" Width="100px" /></td>
                        </tr>
                        <tr runat="server" id="PassingScoreField">
                            <td>
                                Passing Score
                                <insite:RequiredValidator runat="server" ID="PassingScoreValidator" FieldName="Passing Score" ControlToValidate="PassingScore" ValidationGroup="ActivityCreate" />
                            </td>
                            <td>
                                <insite:NumericBox runat="server" ID="PassingScore" MinValue="1" MaxValue="100" NumericMode="Integer" Width="100px" CssClass="d-inline" /> %
                            </td>
                        </tr>
                    </table>

                </div>
            </div>

            <div runat="server" id="SurveyField" class="form-group mb-3">
                <label class="form-label">
                    Form
                    <insite:RequiredValidator runat="server" FieldName="Form" ControlToValidate="SurveyFormIdentifier" ValidationGroup="ActivityCreate" />
                </label>
                <div>
                    <insite:FindWorkflowForm runat="server" ID="SurveyFormIdentifier" />
                    <div runat="server" id="SurveyFormError" class="alert alert-danger mt-3" visible="false"></div>
                </div>
            </div>

            <div runat="server" id="LinkTypeField" class="form-group mb-3">
                <label class="form-label">
                    Link Type
                    <insite:RequiredValidator runat="server" FieldName="URL Type" ControlToValidate="LinkType" ValidationGroup="ActivityCreate" />
                </label>
                <div class="row">
                    <div class="col-lg-6">
                        <insite:ComboBox runat="server" ID="LinkType">
                            <Items>
                                <insite:ComboBoxOption Value="External" Text="External Web Page" Selected="true" />
                                <insite:ComboBoxOption Value="Internal" Text="Internal Web Page" />
                                <insite:ComboBoxOption Value="SCORM" Text="SCORM Package" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                    <div class="col-lg-6">
                        <insite:ComboBox runat="server" ID="ActivityPlatform" Visible="false">
                            <Items>
                                <insite:ComboBoxOption Value="SCORM Cloud" Text="SCORM Cloud" />
                                <insite:ComboBoxOption Value="Moodle" Text="Moodle" />
                                <insite:ComboBoxOption Value="Scoop" Text="Scoop" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>
            </div>

            <div runat="server" id="LinkField" class="form-group mb-3">
                <label class="form-label">
                    Link URL
                    <insite:RequiredValidator runat="server" FieldName="URL" ControlToValidate="NavigateUrl" ValidationGroup="ActivityCreate" />
                </label>
                <div>
                    <insite:TextBox runat="server" ID="NavigateUrl" MaxLength="500" />
                </div>
                <div style="padding-top: 5px">
                    <asp:CheckBox runat="server" ID="NavigateUrlTarget" Text="Open in a new window or tab" />
                </div>
            </div>

            <div runat="server" id="VideoUrlField" class="form-group mb-3">
                <div class="float-end">
                    <asp:HyperLink runat="server" ID="VideoUrlLink" Target="_blank" Visible="false">
                        <i class="fas fa-external-link"></i>
                    </asp:HyperLink>
                </div>
                <label class="form-label">
                    Video URL
                    <insite:RequiredValidator runat="server" FieldName="Video URL" ControlToValidate="VideoUrl" ValidationGroup="ActivityCreate" />
                </label>
                <div>
                    <insite:TextBox runat="server" ID="VideoUrl" MaxLength="500" />
                </div>
            </div>

            <div runat="server" id="VideoTargetField" class="form-group mb-3">
                <label class="form-label">
                    Video Target
                </label>
                <div>
                    <insite:ComboBox runat="server" ID="VideoTarget" Width="100%">
                        <Items>
                            <insite:ComboBoxOption Value="_blank" Text="Opens in a new window or tab" Selected="true" />
                            <insite:ComboBoxOption Value="_self" Text="Opens in the same window as it was clicked" />
                            <insite:ComboBoxOption Value="_top" Text="Opens in the full body of the window" />
                            <insite:ComboBoxOption Value="_embed" Text="Opens in an embedded frame" />
                        </Items>
                    </insite:ComboBox>
                </div>
            </div>

            <div runat="server" id="QuizField" class="form-group mb-3">
                <label class="form-label">
                    Quiz
                    <insite:RequiredValidator runat="server" FieldName="Quiz Form" ControlToValidate="QuizIdentifier" ValidationGroup="ActivityCreate" />
                </label>
                <div>
                    <insite:QuizComboBox runat="server" ID="QuizIdentifier" />
                </div>
            </div>

            <div class="mt-3">
                <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="ActivityCreate" />
                <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
            </div>

        </div>
    </div>
</div>
