<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScenarioQuestions.ascx.cs" Inherits="InSite.UI.Admin.Standards.Standards.Controls.ScenarioQuestions" %>

<%@ Register Src="~/UI/Admin/Assessments/Questions/Controls/QuestionRepeater.ascx" TagName="BankQuestionRepeater" TagPrefix="uc" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="QuestionsUpdatePanel" />
<insite:UpdatePanel runat="server" ID="QuestionsUpdatePanel">
    <Triggers>
        <asp:PostBackTrigger ControlID="AddQuestionButton" />
    </Triggers>
    <ContentTemplate>
        <div class="row mb-3">
            <asp:Panel runat="server" DefaultButton="FilterQuestionsButton" CssClass="col-lg-4 mb-3 mb-lg-0">
                <insite:Container runat="server" ID="FilterQuestionsField">
                    <insite:TextBox runat="server" ID="FilterQuestionsTextBox" EmptyMessage="Filter Questions" CssClass="d-inline-block" style="width: calc(100% - 25px);" />
                    <insite:IconButton runat="server" ID="FilterQuestionsButton" Name="filter" ToolTip="Apply Filter" />
                </insite:Container>
            </asp:Panel>

            <div class="col-lg-8 text-end">
                <insite:Button runat="server" ID="AddQuestionButton" ButtonStyle="Default" ToolTip="Add a new question to this scenario" Text="Add Question" Icon="fas fa-plus-circle" />
            </div>
        </div>

        <uc:BankQuestionRepeater runat="server" ID="QuestionRepeater" />

        <insite:Button runat="server" ID="LoadQuestionsButton" Visible="false" ButtonStyle="Success" CssClass="w-100 mt-3" Text="Load All Questions" Icon="fas fa-spinner" />
    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            const instance = window.scenarioQuestions = window.scenarioQuestions || {};
            instance.scrollToQuestion = function (id) {
                const $questionRow = $('table.question-grid:visible > tbody > tr[data-question="' + String(id) + '"]');
                if ($questionRow.length !== 1)
                    return;

                const headerHeight = $('header.navbar:first').outerHeight();
                let scrollTo = $questionRow.offset().top - headerHeight;

                if (scrollTo < 0)
                    scrollTo = 0;

                $('html, body').animate({ scrollTop: scrollTo }, 250);
            };
        })();
    </script>
</insite:PageFooterContent>