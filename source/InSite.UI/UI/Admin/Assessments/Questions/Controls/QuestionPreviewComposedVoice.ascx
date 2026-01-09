<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPreviewComposedVoice.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionPreviewComposedVoice" %>

<div class="form-group composed-voice-input">
    <insite:InputAudio runat="server" ID="InputAudio" AutoUpload="false" />
    <insite:OutputAudio runat="server" ID="OutputAudio" AllowDelete="true" />
</div>
