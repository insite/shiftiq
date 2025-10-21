<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionExemplarEditor.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Questions.Controls.QuestionExemplarEditor" %>

<div class="form-group mb-3">
    <div>
        <insite:MarkdownEditor runat="server" ID="ExemplarText" TranslationControl="EditorTranslation" />
    </div>
    <div class="mt-1">
        <insite:EditorTranslation runat="server" ID="EditorTranslation" TableContainerID="TranslationContainer" EnableMarkdownConverter="true" />
    </div>

    <div class="form-text my-3">
        Provide a benchmark answer to guide assessors in evaluating responses. This is not visible to learners.
    </div>

    <div runat="server" id="TranslationContainer"></div>
</div>
