<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewComposedVoice.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.ViewComposedVoice" %>

<%@ Register TagPrefix="uc" TagName="ComposedRepeater" Src="~/UI/Admin/Assessments/Attempts/Controls/ViewComposed.ascx" %>

<uc:ComposedRepeater runat="server" ID="ComposedRepeater">
    <AnswerTemplate>
        <insite:OutputAudio runat="server" ID="AudioPlayer" />
    </AnswerTemplate>
</uc:ComposedRepeater>
