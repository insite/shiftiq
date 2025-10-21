<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewTypingSpeed.ascx.cs" Inherits="InSite.UI.Portal.Assessments.QuizAttempts.Controls.ViewTypingSpeed" %>

<div class="card">
    <div class="card-body">
        <div runat="server" id="QuizText" class="fs-lg quiz-text">
        </div>
        <insite:LoadingPanel runat="server" ID="LoadingPanel" />
        <insite:UpdatePanel runat="server" ID="UpdatePanel" />
    </div>
</div>

<insite:PageHeadContent runat="server">
    <insite:ResourceBundle runat="server" Type="Css">
        <Items>
            <insite:ResourceBundleFile Url="/UI/Portal/Assessments/QuizAttempts/Content/timer.css" />
            <insite:ResourceBundleFile Url="/UI/Portal/Assessments/QuizAttempts/Content/typing-speed.css" />
        </Items>
    </insite:ResourceBundle>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <insite:ResourceBundle runat="server" Type="JavaScript">
        <Items>
            <insite:ResourceBundleFile Url="/UI/Portal/Assessments/QuizAttempts/Content/timer.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/Assessments/QuizAttempts/Content/typing-speed.js" />
        </Items>
    </insite:ResourceBundle>
</insite:PageFooterContent>
