<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConditionDetail.ascx.cs" Inherits="InSite.Admin.Surveys.Forms.Controls.SurveyConditionDetail" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<div class="row">

    <div class="col-md-6">
        <uc:FormDetails runat="server" ID="SurveyDetail" />
    </div>
    <div class="col-md-6">
        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
        <insite:UpdatePanel runat="server" ID="UpdatePanel">
            <ContentTemplate>
                <div class="form-group mb-3">
                    <label class="form-label">Question</label>
                    <div>
                        <insite:FormQuestionComboBox runat="server" ID="QuestionID" Width="95%" MaxHeight="420px" />
                        <insite:RequiredValidator runat="server" ControlToValidate="QuestionID" FieldName="Question" ValidationGroup="Survey" />
                    </div>
                    <div class="form-text">
                        Select the question for which you want to define conditional logic so that another question on the form is suppressed, depending upon the respondent's answer to it.
                    </div>
                </div>
                <div runat="server" id="OptionListField" class="form-group mb-3">
                    <label class="form-label">Option List</label>
                    <div>
                        <insite:FormOptionListComboBox runat="server" ID="OptionListID" Width="95%" MaxHeight="380px" />
                        <insite:RequiredValidator runat="server" ControlToValidate="OptionListID" FieldName="Option List" ValidationGroup="Survey" />
                    </div>
                    <div class="form-text">
                        Select the option list for which you want to define your conditional logic.
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">Option</label>
                    <div>
                        <insite:FormOptionItemComboBox runat="server" ID="OptionID" Width="95%" MaxHeight="340px" />
                        <insite:RequiredValidator runat="server" ControlToValidate="OptionID" FieldName="Option" ValidationGroup="Survey" />
                    </div>
                    <div class="form-text">
                        Select the answer field value that will cause one or more subsequent questions to be supressed. You can suppress only questions that appear on subsequent pages. You cannot supress questions on the same page.
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Hide
                        <insite:CustomValidator runat="server" ID="HideValidator" ErrorMessage="Required field: Hide" ValidationGroup="Survey" />
                    </label>
                    <div>
                        <asp:CheckBoxList runat="server" ID="HideQuestionID" Width="95%"></asp:CheckBoxList>
                    </div>
                    <div class="form-text">
                        Select the question that should be suppressed when a respondent selects the above answer field value.
                    </div>
                </div>
            </ContentTemplate>
        </insite:UpdatePanel>
    </div>
</div>
