<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionAnalysisRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Attempts.Controls.QuestionAnalysisRepeater" %>

<%@ Register TagPrefix="uc" TagName="AnalysisTable" Src="QuestionAnalysisTable.ascx" %>

<insite:PageHeadContent runat="server" ContentKey="ResultQuestionAnalysisRepeater">
    <style type="text/css">
        table.question-analysis img {
            max-width: 100%;
        }

        table.table.table-answers {
            background-color: transparent;
            margin-bottom: 0;
        }

            table.table.table-answers > tbody > tr:last-child > td {
                border-bottom-width: 0;
            }

        table.table.table-answers td,
        table.table.table-answers th {
            white-space: nowrap;
        }

        .question-sequence {
            font-size: 27px;
            color: #265F9F;
            white-space: nowrap;
            text-align: center;
            padding-top: 0.25rem !important;
        }
    </style>
</insite:PageHeadContent>

<div class="mb-3">
    <insite:DropDownButton runat="server" ID="DownloadButton" ButtonStyle="OutlinePrimary" DefaultAction="None" IconName="download" Text="Download" CssClass="d-inline-block">
        <Items>
            <insite:DropDownButtonItem Name="Csv" IconName="file-csv" Text="Text (*.csv)" />
            <insite:DropDownButtonItem Name="Xlsx" IconName="file-excel" Text="Excel (*.xlsx)" />
        </Items>
    </insite:DropDownButton>
    <insite:Button runat="server" ID="ToggleQuestionAnalysisImagesVisibility" ButtonStyle="Default" Text="<span>Hide Images</span>" Icon="fas fa-image" />
    <asp:HiddenField runat="server" ID="IsQuestionAnalysisImagesVisible" Value="true" />
</div>

<table class="table table-striped question-analysis">

    <thead>
        <tr>
            <th style="width:40px;" class="text-center">#</th>
            <th style="width:10px;"></th>
            <th>Question</th>
            <th>Answers</th>
        </tr>
    </thead>

    <tbody>
        <asp:Repeater runat="server" ID="Repeater">
            <ItemTemplate>
                <tr data-question='<%# Eval("Question.Identifier") %>'>
                    <td class="question-sequence"><%# GetQuestionSequence(Container.DataItem) %></td>
                    <td><%# Eval("Question.Classification.Code") %></td>
                    <td><%# GetContentTitle(Eval("Question.Content")) %></td>
                    <td class="py-0 pe-0">
                        <uc:AnalysisTable runat="server" ID="AnalysisTable" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>

</table>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">

        (function () {
            var $btn = $('#<%= ToggleQuestionAnalysisImagesVisibility.ClientID %>');
            var $imgs = $('table.question-analysis > tbody > tr > td:nth-child(3) img');
            
            if ($imgs.length == 0) {
                $btn.hide();
                return;
            }

            $btn.show().on('click', onToggle);

            var $input = $('#<%= IsQuestionAnalysisImagesVisible.ClientID %>');

            function onToggle(e) {
                e.preventDefault();

                $input.val($input.val() == 'false')

                refresh();
            }

            function refresh() {
                if ($input.val() == 'true') {
                    $imgs.show();
                    $btn.find("span").html('Hide Images');
                } else {
                    $imgs.hide();
                    $btn.find("span").html('Show Images');
                }
            }

            function scrollToQuestion(questionId) {
                var $row = $(".question-analysis tr[data-question='" + questionId + "']");

                if ($row.length > 0) {
                    setTimeout(function () {
                        $("html, body").animate({
                            scrollTop: $row.offset().top - 130
                        }, 50);
                    }, 0);
                }
            }

            <%= SelectedQuestionID.HasValue ? string.Format("scrollToQuestion('{0}');", SelectedQuestionID) : "" %>

            refresh();
        })();

    </script>

</insite:PageFooterContent>