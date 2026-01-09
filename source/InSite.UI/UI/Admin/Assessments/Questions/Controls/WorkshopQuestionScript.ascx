<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkshopQuestionScript.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.WorkshopQuestionScript" %>

<%@ Register TagPrefix="uc" TagName="TextEditor" Src="../Controls/QuestionTextEditor.ascx" %>

<asp:Panel runat="server" ID="Container" CssClass="d-none">
    <uc:TextEditor runat="server" ID="QuestionText" AllowTranslation="false" />
</asp:Panel>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var instance; {
                var workshopQuestion = window.workshopQuestion = window.workshopQuestion || {};
                instance = workshopQuestion.markdown = workshopQuestion.markdown || {};
            }

            var editorId = '<%= QuestionText.ClientID %>';
            var containerId = '<%= Container.ClientID %>';
            var $editor = null;

            instance.move = function ($input) {
                moveBack();

                var $container = $(document.getElementById(containerId)).removeClass('d-none');

                $editor = $container.find('> .form-group').detach().insertAfter($input);

                setTimeout(questionTextEditor.focus, 10, editorId);
            };

            instance.set = function ($input) {
                var data = $input.val();
                if (data && data.length > 0 && data[0] == '+')
                    data = JSON.parse(data.substring(1));
                else
                    data = {};

                questionTextEditor.setTranslation(editorId, {
                    lang: 'en',
                    data: data
                });
            };

            instance.getValue = function () {
                if ($editor == null)
                    return null;

                var value = questionTextEditor.getTranslation(editorId);
                return '+' + JSON.stringify(value.data);
            };

            instance.remove = function ($link) {
                moveBack();
            };

            function moveBack() {
                if ($editor != null) {
                    $(document.getElementById(containerId)).prepend($editor.detach()).addClass('d-none');

                    $editor = null;
                }
            }
        })();
    </script>
</insite:PageFooterContent>