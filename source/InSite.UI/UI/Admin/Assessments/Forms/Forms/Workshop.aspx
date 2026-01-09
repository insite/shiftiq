<%@ Page Language="C#" CodeBehind="Workshop.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Workshop" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="FormDetails" Src="../Controls/FormDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionStatisticsPanel" Src="~/UI/Admin/Assessments/Questions/Controls/QuestionStatisticsPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="WorkshopCommentRepeater" Src="~/UI/Admin/Assessments/Comments/Controls/WorkshopCommentRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="WorkshopQuestionScript" Src="~/UI/Admin/Assessments/Questions/Controls/WorkshopQuestionScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="WorkshopQuestionRepeater" Src="~/UI/Admin/Assessments/Questions/Controls/WorkshopQuestionRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="AttachmentsTabsNav" Src="~/UI/Admin/Assessments/Attachments/Controls/AttachmentsTabsNav.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProblemRepeater" Src="~/UI/Admin/Assessments/Outlines/Controls/ProblemRepeater.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="FormTab" Title="Form" Icon="far fa-window" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <insite:Nav runat="server">
                        <insite:NavItem runat="server" Title="Form">
                            <uc:FormDetails runat="server" ID="FormDetails" />
                        </insite:NavItem>
                        <insite:NavItem runat="server" Title="Summaries">
                            <uc:QuestionStatisticsPanel runat="server" ID="StatisticsPanel" />
                        </insite:NavItem>
                    </insite:Nav>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="QuestionsTab" Title="Questions" Icon="far fa-question" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="QuestionsUpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="QuestionsUpdatePanel">
                        <ContentTemplate>
                            <div runat="server" id="QuestionCompetencyRow" class="row">
                                <div class="col-md-6">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Section
                                        </label>
                                        <insite:ComboBox runat="server" ID="SectionSelector" />
                                    </div>
                                </div>

                                <div class="col-md-6">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Competency
                                        </label>
                                        <insite:StandardComboBox runat="server" ID="CompetencySelector" TextType="CodeTitle" />
                                    </div>
                                </div>
                            </div>

                            <insite:Container runat="server" ID="QuestionFilter">
                                <div class="row">
                                    <div class="col-sm-6">
                                        <div class="row">

                                            <div class="col-md-6">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Question Flag
                                                    </label>
                                                    <div>
                                                        <insite:FlagMultiComboBox runat="server" ID="QuestionFlag" AllowBlank="false" Multiple-ActionsBox="true" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-md-6">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Condition
                                                    </label>
                                                    <div>
                                                        <insite:QuestionConditionMultiComboBox runat="server" ID="QuestionCondition" Multiple-ActionsBox="true" />
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="row">

                                            <div class="col-lg-4">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Taxonomy
                                                    </label>
                                                    <insite:TaxonomyComboBox runat="server" ID="QuestionTaxonomy" />
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        LIG
                                                    </label>
                                                    <insite:BooleanComboBox runat="server" ID="IsQuestionHasLig" TrueText="LIG" FalseText="No LIG" AllowBlank="true" />
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Reference
                                                    </label>
                                                    <insite:BooleanComboBox runat="server" ID="IsQuestionHasReference" TrueText="Reference" FalseText="No Reference" AllowBlank="true" />
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                                <div class="row">

                                    <div class="col-lg-3">
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Question Changed On
                                            </label>
                                            <div>
                                                <insite:ComboBox runat="server" ID="QuestionDateRangeSelector" />
                                            </div>
                                        </div>
                                    </div>

                                    <div runat="server" ID="QuestionDateRangeInputs" class="col-lg-5 d-none">
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                &nbsp;
                                            </label>
                                            <div>
                                                <div class="d-inline-block" style="width:calc(50% - 20px);">
                                                    <insite:DateSelector runat="server" ID="QuestionDateRangeSince" EmptyMessage="&ge;" />
                                                </div>
                                                <div class="d-inline-block text-center" style="width:30px;">
                                                    to
                                                </div>
                                                <div class="d-inline-block" style="width:calc(50% - 20px);">
                                                    <insite:DateSelector runat="server" ID="QuestionDateRangeBefore" EmptyMessage="&lt;" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </insite:Container>

                            <div>
                                <insite:Button runat="server" ID="ApplyFilterButton" ButtonStyle="Default" 
                                    Text="Apply" Icon="fas fa-filter" />
                                <insite:Button runat="server" ID="ClearFilterButton" ButtonStyle="Default" 
                                    Text="Clear" Icon="fas fa-times" />
                            </div>

                            <hr class="mt-4" />

                            <div runat="server" id="NoQuestionsMessage" visible="false" class="alert alert-warning" role="alert">
    		                    There are no questions for this form.
                            </div>

                            <div class="mt-3">
                                <h3 runat="server" id="QuestionsHeader">
                                    Questions
                                    <span class="form-text">(<asp:Literal runat="server" ID="QuestionCount" />)</span>
                                </h3>

                                <uc:WorkshopQuestionRepeater runat="server" ID="QuestionRepeater" />
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>


                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CommentsTab" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <div class="mb-3">
                        <insite:AddButton runat="server" id="AddCommentLink" Text="New Comment" />
                    </div>

                    <uc:WorkshopCommentRepeater runat="server" ID="CommentRepeater" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AttachmentsTab" Title="Attachments" Icon="far fa-paperclip" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <div class="row mb-3">
                        <div class="col-lg-6">
                            <insite:TextBox runat="server" ID="FilterAttachmentsTextBox" Width="300" EmptyMessage="Filter Attachments" />
                        </div>
                    </div>

                    <uc:AttachmentsTabsNav runat="server" ID="AttachmentsNav" KeywordInput="FilterAttachmentsTextBox" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ProblemTab" Title="Problems" Icon="far fa-exclamation-triangle" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <uc:ProblemRepeater runat="server" ID="ProblemRepeater" />

                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:CloseButton runat="server" ID="CloseButton" />
    </div>

    <insite:Modal runat="server" ID="CommentWindow" Title="New Comment" Width="700px">
        <ContentTemplate>

            <div class="p-2">

                <div class="form-group mb-3">
                    <label class="form-label">Author Type</label>
                    <insite:CommentAuthorTypeComboBox runat="server" ID="CommentAuthorType" AllowBlank="false" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Flag</label>
                    <insite:FlagComboBox runat="server" ID="CommentFlag" AllowBlank="false" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Text</label>
                    <insite:TextBox runat="server" ID="CommentText" TextMode="MultiLine" Rows="4" />
                </div>

                <div class="mt-3">
                    <insite:SaveButton runat="server" ID="PostCommentButton" Text="Post" OnClientClick="saveCommentWindow(); return false;" />
                    <insite:CloseButton runat="server" ButtonStyle="Default" OnClientClick="closeCommentWindow(); return false;" />
                </div>

            </div>

            <insite:LoadingPanel runat="server" />
        </ContentTemplate>
    </insite:Modal>

    <uc:WorkshopQuestionScript runat="server" ID="WorkshopScript" />

    <insite:PageFooterContent runat="server">
        <script>
            var _fieldId = null, _questionId = null;

            function showCommentWindow(questionId, fieldId) {
                if (!questionId || !fieldId)
                    return;

                _fieldId = fieldId;
                _questionId = questionId;

                modalManager.show($('#<%= CommentWindow.ClientID %>'));
            }

            function closeCommentWindow() {
                modalManager.close($('#<%= CommentWindow.ClientID %>'));
            }

            function saveCommentWindow() {
                var $loadingPanel = $('#<%= CommentWindow.ClientID %> .loading-panel').show();

                $.ajax({
                    type: 'POST',
                    data:
                    {
                        isPageAjax: true,
                        bankId: '<%= BankID %>',
                        fieldId: _fieldId,
                        authorType: $('#<%= CommentAuthorType.ClientID %>').selectpicker('val'),
                        flag: $('#<%= CommentFlag.ClientID %>').selectpicker('val'),
                        text: $('#<%= CommentText.ClientID %>').val()
                    },
                    error: function () {
                        alert('An error occurred during operation.');
                    },
                    success: function (data) {
                        var $section = $('tr[data-question="' + _questionId + '"] div.posted-comments-section');

                        $section.show();
                        $section.html(data);

                        {
                            var $combo = $('#<%= CommentAuthorType.ClientID %>');
                            $combo.selectpicker('val', $combo.find('option:first').prop('value'));
                        }

                        {
                            var $combo = $('#<%= CommentFlag.ClientID %>');
                            $combo.selectpicker('val', $combo.find('option:first').prop('value'));
                        }

                        $('#<%= CommentText.ClientID %>').val('');

                        bindScrollParameter();

                        modalManager.close($('#<%= CommentWindow.ClientID %>'));
                    },
                    complete: function () {
                        $loadingPanel.hide();
                    }
                });
            }

            (function () {
                Sys.Application.add_load(function () {
                    $('#<%= QuestionDateRangeSelector.ClientID %>')
                        .off('change', onDateRangeSelected)
                        .on('change', onDateRangeSelected);

                    onDateRangeSelected();
                });

                function onDateRangeSelected() {
                    var $input = $('#<%= QuestionDateRangeSelector.ClientID %>');
                    var $panel = $('#<%= QuestionDateRangeInputs.ClientID %>');

                    if ($input.selectpicker('val') == 'Custom')
                        $panel.removeClass('d-none');
                    else
                        $panel.addClass('d-none');
                }

            })();

            (function () {
                if (typeof URLSearchParams !== 'undefined') {
                    var location = window.location;
                    var queryParams = new URLSearchParams(location.search);
                    var scroll = queryParams.get('scroll');

                    if (scroll) {
                        try {
                            var scrollTop = inSite.common.base64.toInt(scroll);

                            $(document).ready(function () {
                                $(window).scrollTop(scrollTop);
                            });

                            queryParams.delete('scroll');

                            var path = location.pathname + '?' + queryParams.toString();
                            var url = location.origin + path;

                            window.history.replaceState({}, '', url);

                            $('form#aspnetForm').attr('action', path);
                        } catch (e) {

                        }
                    }
                }

                bindScrollParameter();
            })();

            function bindScrollParameter() {
                $('a.scroll-send').on('click', function () {
                    var scroll = $(window).scrollTop();

                    if (scroll > 0)
                        scroll = inSite.common.base64.fromInt(Math.floor(scroll));
                    else
                        scroll = null;

                    var url = $(this).attr('href');
                    $(this).attr('href', inSite.common.updateQueryString('scroll', scroll, url));
                });
            }
        </script>
    </insite:PageFooterContent>

</asp:Content>
