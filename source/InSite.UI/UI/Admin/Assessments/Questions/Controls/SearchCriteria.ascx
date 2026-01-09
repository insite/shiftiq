<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="QuestionText" EmptyMessage="Question Text" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="QuestionCode" EmptyMessage="Question Code" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="QuestionReference" EmptyMessage="Question Reference" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="QuestionTag" EmptyMessage="Question Tag" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                         <insite:ExamQuestionTypeComboBox runat="server" ID="QuestionType" EmptyMessage="Question Type" AllowBlank="true" />
                    </div>

                    <div class="mb-2">
                         <insite:ColorComboBox runat="server" ID="QuestionFlag" EmptyMessage="Question Flag" AllowBlank="true" AllowNone="true" />
                    </div>

                    <div class="mb-2">
                         <insite:ComboBox runat="server" ID="QuestionPublicationStatus" EmptyMessage="Publication Status" AllowBlank="true" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="QuestionAsset" EmptyMessage="Question Asset" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="QuestionDateRangeSelector" EmptyMessage="Question Changed On" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="QuestionBank" EmptyMessage="Question Bank" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="QuestionCompetencyTitle" EmptyMessage="Question Competency title" />
                    </div>

                    <div class="mb-2">
                        <insite:DifficultyComboBox runat="server" ID="QuestionDifficulty" EmptyMessage="Question Difficulty" />
                    </div>

                    <div class="mb-2 d-none" runat="server" id="QuestionDateRangeSinceWrapper">
                        <insite:DateSelector runat="server" ID="QuestionDateRangeSince" EmptyMessage="Question Changed &ge;" />
                    </div>

                    <div class="mb-2 d-none" runat="server" id="QuestionDateRangeBeforeWrapper">
                        <insite:DateSelector runat="server" ID="QuestionDateRangeBefore" EmptyMessage="Question Changed &lt;" />
                    </div>

                    <div class="mb-2">
                         <insite:RubricComboBox runat="server" ID="RubricComboBox" EmptyMessage="Rubric" AllowBlank="true" />
                    </div>

                    <div class="mb-2">
                         <insite:ComboBox runat="server" ID="QuestionClassificationTags" EmptyMessage="Reporting Tags" AllowBlank="true" />
                    </div>
                    
                </div>
            </div>
        </div>
    </div>
    <div class="col-3">
        <div>
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
    </div>
    <div class="col-3">
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            Sys.Application.add_load(function () {
                $('#<%= QuestionDateRangeSelector.ClientID %>')
                    .off('change', onQuestionDateRangeSelected)
                    .on('change', onQuestionDateRangeSelected);

                onQuestionDateRangeSelected();
            });

            function onQuestionDateRangeSelected() {
                var $combo = $('#<%= QuestionDateRangeSelector.ClientID %>');
                var $sinceWrapper = $('#<%= QuestionDateRangeSinceWrapper.ClientID %>');
                var $beforeWrapper = $('#<%= QuestionDateRangeBeforeWrapper.ClientID %>');

                if ($combo.selectpicker('val') == 'Custom') {
                    $sinceWrapper.removeClass('d-none');
                    $beforeWrapper.removeClass('d-none');
                } else {
                    $sinceWrapper.addClass('d-none');
                    $beforeWrapper.addClass('d-none');
                }
            }

        })();
    </script>
</insite:PageFooterContent>
