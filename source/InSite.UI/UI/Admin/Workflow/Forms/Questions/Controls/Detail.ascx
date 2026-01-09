<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.Controls.Detail" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Questions/Controls/DetailAnswerRepeater.ascx" TagName="DetailAnswerRepeater" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Questions/Controls/DetailCommentBox.ascx" TagName="DetailCommentBox" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Questions/Controls/DetailList.ascx" TagName="DetailList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Questions/Controls/DetailLikertTable.ascx" TagName="DetailLikertTable" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/ContentEditor/SectionMdHtml.ascx" TagName="SectionMdHtml" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assets/Glossaries/Terms/Controls/TermGrid.ascx" TagName="GlossaryTermGrid" TagPrefix="uc" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .upload-error {
            color: red;
        }

        .settings-row .form-horizontal .form-group > .control-label {
            width: 135px !important;
            max-width: none !important;
        }

        .settings-row .form-horizontal .form-group > .value {
            width: 175px !important;
            min-width: 0 !important;
        }
    </style>
</insite:PageHeadContent>

<insite:ValidationSummary runat="server" ValidationGroup="SurveyQuestion" />

<insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

    <insite:NavItem runat="server" ID="SurveyInformationSection" Title="Question" Icon="far fa-question" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">Question</h2>

            <div class="row">
                <div class="col-lg-4">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Details</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Page #
                                    <insite:RequiredValidator runat="server" ID="PageIDRequired" ControlToValidate="PageID" FieldName="Page #" ValidationGroup="SurveyQuestion" />
                                </label>
                                <div>
                                    <insite:ComboBox runat="server" ID="PageID" EmptyMessage="Page" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Question Type
                                    <insite:RequiredValidator runat="server" ID="QuestionTypeSelectorRequired" ControlToValidate="QuestionTypeSelector" FieldName="Question Type" ValidationGroup="SurveyQuestion" />
                                </label>
                                <div>
                                    <insite:FormQuestionTypeComboBox runat="server" ID="QuestionTypeSelector" />
                                    <asp:Literal runat="server" ID="QuestionType" />
                                </div>
                            </div>


                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                            <insite:UpdatePanel runat="server" ID="UpdatePanel">
                                <ContentTemplate>
                                    <div class="form-group mb-3">
                                        <label class="form-label">Question #</label>
                                        <div class="row">
                                            <div class="col-lg-6">
                                                <insite:NumericBox runat="server" ID="Sequence" NumericMode="Integer" MinValue="1" CssClass="w-75 d-inline" />
                                                <insite:RequiredValidator runat="server" ControlToValidate="Sequence" FieldName="Question #" ValidationGroup="SurveyQuestion" />
                                            </div>
                                            <div class="col-lg-6">
                                                <insite:ColorComboBox runat="server" ID="IndicatorValue" />
                                            </div>
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Question Code</label>
                                        <div>
                                            <insite:TextBox runat="server" ID="QuestionCode" MaxLength="4" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <div>
                                            <asp:Literal runat="server" ID="IndicatorPreview"></asp:Literal>
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Respondent Attribute</label>
                                        <div>
                                            <insite:TextBox runat="server" ID="QuestionAttribute" MaxLength="100" />
                                        </div>
                                        <div class="form-text">
                                            Answers to this question will update the specified field in a respondent's contact record.
                                            Use with caution to avoid overwriting existing information.
                                        </div>
                                    </div>

                                    <insite:Container runat="server" ID="SettingsContainer">

                                        <h3>Settings</h3>

                                        <div runat="server" id="IsRequiredField">
                                            <insite:CheckBox runat="server" ID="IsRequired" Text="Answer Required" />
                                        </div>

                                        <div runat="server" id="IsNestedField">
                                            <insite:CheckBox runat="server" ID="IsNested" Text="Nested with Previous Question" />
                                        </div>

                                        <div runat="server" id="NumberEnableStatisticsField">
                                            <insite:CheckBox runat="server" ID="NumberEnableStatistics" Text="Calculate Statistics" />
                                        </div>

                                        <div runat="server" id="NumberAutoCalcField">
                                            <insite:CheckBox runat="server" ID="NumberEnableAutoCalc" Text="Sum from other" />
                                            <div style="padding-left: 25px;">
                                                <insite:FindEntity runat="server" ID="NumberAutoCalcFields"
                                                    EntityName="Numeric Question" EmptyMessage="Questions" MaxSelectionCount="0" PageSize="50" />
                                            </div>
                                        </div>

                                        <div runat="server" id="NumberNaPermitted">
                                            <insite:CheckBox runat="server" ID="NumberEnableNa" Text="Not Applicable Permitted" />
                                        </div>

                                        <div runat="server" id="EnableCreateCaseField">
                                            <insite:CheckBox runat="server" ID="EnableCreateCase" Text="Create Case" />
                                        </div>
                                    </insite:Container>
                                </ContentTemplate>
                            </insite:UpdatePanel>

                        </div>
                    </div>
                </div>

                <div class="col-lg-8 col-question-text">
                    <div class="card border-0 shadow-lg mb-3">
                        <div class="card-body">
                            <h3>Question Text</h3>
                            <uc:SectionMdHtml runat="server" ID="QuestionTitle" EnableAJAX="false" />
                        </div>
                    </div>

                    <uc:DetailCommentBox runat="server" ID="DetailCommentBox" />
                </div>
            </div>

        </section>
    </insite:NavItem>
    <insite:NavItem runat="server" ID="ListSection" Title="Options" Icon="far fa-list" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">Options</h2>

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">
                    <uc:DetailList runat="server" ID="DetailList" />
                </div>
            </div>
        </section>
    </insite:NavItem>
    <insite:NavItem runat="server" ID="LikertTableSection" Title="Options" Icon="far fa-list" IconPosition="BeforeText">
        <section>
             <h2 class="h4 mt-4 mb-3">Options</h2>

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">
                    <uc:DetailLikertTable runat="server" ID="DetailLikertTable" />
                </div>
            </div>
        </section>
    </insite:NavItem>
    <insite:NavItem runat="server" ID="GlossaryTermSection" Title="Glossary Terms" Icon="far fa-scroll" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">Glossary Terms</h2>

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">
                    <uc:GlossaryTermGrid runat="server" ID="GlossaryTermGrid" />
                </div>
            </div>
        </section>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="AnswerNav" Title="Answers" Icon="far fa-poll-people" IconPosition="BeforeText">
        <uc:DetailAnswerRepeater runat="server" ID="DetailAnswerRepeater" />
    </insite:NavItem>

</insite:Nav>
<insite:PageHeadContent runat="server">
    <style type="text/css">
    
        .col-question-text .content-field {
            position: relative;
        }
    
        .col-question-text .content-field > .commands {
            position: absolute;
            right: 51px;
            top: -2px;
        }
    
        .col-question-text .content-field > .commands > span.lang-out {
            background-color: #aaa;
        }
    
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            $('.col-question-text .content-section-mdhtml').on('updated.content-section', function (e, lang) {
                if (typeof optionGrid != 'undefined')
                    optionGrid.setLanguage(lang);

                if (typeof detailLikertTable != 'undefined')
                    detailLikertTable.setLanguage(lang);
            });

            var $numAutoCalc = $(document.getElementById('<%= NumberEnableAutoCalc.ClientID %>')).on('change', onNumAutoCalcChanged);
            var $numNa = $(document.getElementById('<%= NumberEnableNa.ClientID %>')).on('change', onNumNaChanged);

            onNumAutoCalcChanged();
            onNumNaChanged();

            function onNumAutoCalcChanged() {
                var $container = $(document.getElementById('<%= NumberAutoCalcFields.ClientID %>')).closest('div');
                if ($numAutoCalc.prop('checked') == true) {
                    $container.show();
                    $numNa.prop('disabled', true).prop('checked', false);
                    onNumNaChanged();
                } else {
                    $container.hide();
                    $numNa.prop('disabled', false);
                }
            }

            function onNumNaChanged() {
                if ($numNa.prop('checked') == true) {
                    $numAutoCalc.prop('disabled', true).prop('checked', false);
                    onNumAutoCalcChanged();
                } else {
                    $numAutoCalc.prop('disabled', false);
                }
            }
        })();
    </script>
</insite:PageFooterContent>


