<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Assessments.Questions.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="BankDetails" Src="~/UI/Admin/Assessments/Banks/Controls/BankInfo.ascx" %>
<%@ Register TagPrefix="uc" TagName="SetDetails" Src="~/UI/Admin/Assessments/Sets/Controls/SetInfo.ascx" %>
<%@ Register TagPrefix="uc" TagName="TextEditor" Src="../Controls/QuestionTextEditor.ascx" %>
<%@ Register TagPrefix="uc" TagName="MatchingDetails" Src="../Controls/QuestionMatchingDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="RubricDetail" Src="../Controls/RubricDetail.ascx" %>
<%@ Register TagPrefix="uc" TagName="LikertDetails" Src="../Controls/QuestionLikertDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="HotspotDetails" Src="../Controls/QuestionHotspotDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="OrderingDetails" Src="../Controls/QuestionOrderingDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="ExemplarEditor" Src="../Controls/QuestionExemplarEditor.ascx" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-question"></i>
            Question
        </h2>

        <div runat="server" id="CreationTypePanel" class="row mb-3">
            <div class="col-lg-6">

                <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Question"/>

                <insite:Container runat="server" ID="DuplicateQuestionContainer">
                    <div class="form-group mt-3 mb-3">
                        <label class="form-label">
                            Assessment Bank
                            <insite:RequiredValidator runat="server" ControlToValidate="DuplicateBankId" FieldName="Assessment Bank" ValidationGroup="Assessment" />
                        </label>
                        <div>
                            <insite:FindBank runat="server" ID="DuplicateBankId" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="form-label">
                            Assessment Question
                            <insite:RequiredValidator runat="server" ControlToValidate="DuplicateQuestionId" FieldName="Assessment Question" ValidationGroup="Assessment" />
                        </label>
                        <div>
                            <insite:FindBankQuestion runat="server" ID="DuplicateQuestionId" />
                        </div>
                    </div>
                </insite:Container>

            </div>
        </div>

        <div class="row mb-3">

            <div class="col-lg-6">
                <div id="SetOutputField" runat="server" class="card border-0 shadow-lg mb-3">
                    <div class="card-body">
                        <h3>Question Set</h3>
                        <uc:SetDetails runat="server" ID="SetDetails" />
                    </div>
                </div>

                <div runat="server" id="OneQuestionPanel1" class="card border-0 shadow-lg">
                    <div class="card-body">
                        <h3>Question</h3>

                        <div runat="server" id="SetInputField" class="form-group mb-3">
                            <label class="form-label">
                                Question Set
                                <insite:RequiredValidator runat="server" ControlToValidate="QuestionSetSelector" FieldName="Question Set" ValidationGroup="Assessment" />
                            </label>
                            <div>
                                <insite:ComboBox runat="server" ID="QuestionSetSelector" Width="350px" />
                            </div>
                        </div>

                        <div runat="server" id="QuestionOutputStandardField" class="form-group mb-3">
                            <label class="form-label">Competency Evaluation</label>
                            <div>
                                <assessments:AssetTitleDisplay runat="server" ID="QuestionOutputStandard" />
                            </div>
                        </div>

                        <div runat="server" id="QuestionInputStandardField" class="form-group mb-3">
                            <label class="form-label">Competency Evaluation</label>
                            <div>
                                <insite:StandardComboBox runat="server" ID="QuestionInputStandard" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Question Type
                                <insite:RequiredValidator runat="server" ControlToValidate="QuestionType" FieldName="Question Type" ValidationGroup="Assessment" />
                            </label>
                            <div>
                                <insite:ExamQuestionTypeComboBox runat="server" ID="QuestionType" AllowBlank="false" />
                            </div>
                        </div>

                        <div runat="server" id="QuestionSubtypeField" class="form-group mb-3" visible="false">
                            <label class="form-label">
                                Question Subtype
                                <insite:RequiredValidator runat="server" ControlToValidate="QuestionSubtype" FieldName="Question Subtype" ValidationGroup="Assessment" />
                            </label>
                            <div>
                                <insite:ExamQuestionTypeComboBox runat="server" ID="QuestionSubtype" AllowBlank="false" />
                            </div>
                        </div>

                        <div runat="server" id="CalculationMethodField" class="form-group mb-3" visible="false">
                            <label class="form-label">
                                Calculation Method
                                <insite:RequiredValidator runat="server" ControlToValidate="CalculationMethod" FieldName="Calculation Method" ValidationGroup="Assessment" />
                            </label>
                            <div>
                                <insite:ExamQuestionCalculationMethodComboBox runat="server" ID="CalculationMethod" />
                            </div>
                        </div>

                        <div runat="server" id="MaximumPointsField" class="form-group mb-3" visible="false">
                            <label class="form-label">
                                Maximum Possible Points
                                <insite:RequiredValidator runat="server" ControlToValidate="MaximumPoints" FieldName="Maximum Possible Points" ValidationGroup="Assessment" />
                            </label>
                            <div>
                                <insite:NumericBox runat="server" ID="MaximumPoints" DecimalPlaces="2" MinValue="0.00" MaxValue="999.99" />
                            </div>
                            <div class="form-text">The maximum number of possible points awarded for an answer to this question.</div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Bank</h3>
                        <uc:BankDetails runat="server" ID="BankDetails" />
                    </div>
                </div>
            </div>
        </div>

        <insite:Container runat="server" ID="OneQuestionPanel2">
            <div class="card border-0 shadow-lg h-100 mb-3">
                <div class="card-body">
                    <h3>Question Text</h3>
                    <uc:TextEditor runat="server" ID="QuestionText" />
                </div>
            </div>

            <div runat="server" id="ExemplarCard" visible="false" class="card border-0 shadow-lg h-100 mb-3">
                <div class="card-body">
                    <h3>Exemplar</h3>
                    <uc:ExemplarEditor runat="server" ID="ExemplarText" />
                </div>
            </div>

            <div runat="server" id="OptionsCard" visible="false" class="card border-0 shadow-lg h-100 mb-3">
                <div class="card-body">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OptionListUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="OptionListUpdatePanel" Cssclass="form-group mb-3">
                        <ContentTemplate>
                            <insite:DynamicControl runat="server" ID="Options" />
                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>
            </div>

            <div runat="server" id="MatchingCard" visible="false" class="card border-0 shadow-lg h-100 mb-3">
                <div class="card-body">

                    <uc:MatchingDetails runat="server" ID="MatchingDetails" ValidationGroup="Assessment" />

                </div>
            </div>

            <div runat="server" id="RubricCard" visible="false" class="card border-0 shadow-lg h-100 mb-3">
                <div class="card-body">

                    <uc:RubricDetail runat="server" ID="RubricDetail" />

                </div>
            </div>

            <insite:Container runat="server" ID="LikertContainer" Visible="false">
                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="LikertUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="LikertUpdatePanel" Cssclass="form-group mb-3">
                    <ContentTemplate>
                        <uc:LikertDetails runat="server" ID="LikertDetails" ValidationGroup="Assessment" />
                    </ContentTemplate>
                </insite:UpdatePanel>
            </insite:Container>

            <insite:Container runat="server" ID="HotspotContainer" Visible="false">
                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="HotspotUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="HotspotUpdatePanel" Cssclass="form-group mb-3">
                    <ContentTemplate>
                        <uc:HotspotDetails runat="server" ID="HotspotDetails" ValidationGroup="Assessment" />
                    </ContentTemplate>
                </insite:UpdatePanel>
            </insite:Container>

            <insite:Container runat="server" ID="OrderingContainer" Visible="false">
                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OrderingUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="OrderingUpdatePanel" Cssclass="form-group mb-3">
                    <ContentTemplate>
                        <uc:OrderingDetails runat="server" ID="OrderingDetails" ValidationGroup="Assessment" />
                    </ContentTemplate>
                </insite:UpdatePanel>
            </insite:Container>
        </insite:Container>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" CausesValidation="true" GroupName="QuestionCommands" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>

</asp:Content>
