<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionMatchingDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionMatchingDetails" %>

<%@ Register Src="MatchingPairList.ascx" TagName="MatchingPairList" TagPrefix="uc" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AnswersUpdatePanel" />
<insite:UpdatePanel runat="server" ID="AnswersUpdatePanel" Cssclass="form-group mb-3">
    <ContentTemplate>
        <uc:MatchingPairList runat="server" ID="Answers" />
    </ContentTemplate>
</insite:UpdatePanel>

<h3>Additional Match Possibilities (distractors)</h3>

<div class="form-group mb-3">
    <div>
        <insite:TextBox runat="server" TextMode="MultiLine" Rows="4" TranslationControl="Distractors" />
        <insite:EditorTranslation runat="server" ID="Distractors" />
    </div>
</div>

<div class="row">

    <div class="col-md-4">
        <h3>Feedback for Correct Answers</h3>

        <div class="form-group mb-3">
            <div>
                <insite:TextBox runat="server" TextMode="MultiLine" Rows="2" TranslationControl="FeedbackForCorrectAnswers" />
                <insite:EditorTranslation runat="server" ID="FeedbackForCorrectAnswers" />
            </div>
        </div>
    </div>

    <div class="col-md-4">
        <h3>Feedback for Wrong Answers</h3>

        <div class="form-group mb-3">
            <div>
                <insite:TextBox runat="server" TextMode="MultiLine" Rows="2" TranslationControl="FeedbackForWrongAnswers" />
                <insite:EditorTranslation runat="server" ID="FeedbackForWrongAnswers" />
            </div>
        </div>
    </div>

    <div class="col-md-4">
        <h3>Feedback for Everyone</h3>

        <div class="form-group mb-3">
            <div>
                <insite:TextBox runat="server" TextMode="MultiLine" Rows="2" TranslationControl="FeedbackForEveryone" />
                <insite:EditorTranslation runat="server" ID="FeedbackForEveryone" />
            </div>
        </div>
    </div>

</div>
