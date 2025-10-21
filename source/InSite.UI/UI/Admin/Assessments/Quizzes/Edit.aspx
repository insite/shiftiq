<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Quizzes.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="QuizDetails" Src="~/UI/Admin/Assessments/Quizzes/Controls/Details.ascx" %>
<%@ Register TagPrefix="uc" TagName="TypingSpeed" Src="~/UI/Admin/Assessments/Quizzes/Controls/EditorTypingSpeed.ascx" %>
<%@ Register TagPrefix="uc" TagName="TypingAccuracy" Src="~/UI/Admin/Assessments/Quizzes/Controls/EditorTypingAccuracy.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Quiz" />

    <asp:CustomValidator runat="server" ID="MaxSizeValidator" ErrorMessage="Quiz text exceeded the max length" Display="None" ValidationGroup="Quiz" />

    <section class="mb-3">
        <div class="row">
            <div class="col-md-6">

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <h3>Quiz</h3>
                        <uc:QuizDetails runat="server" ID="QuizDetails" ValidationGroup="Quiz" />
                    </div>
                </div>

            </div>
        </div>
    </section>

    <section runat="server" id="TypingSpeedSection" class="mb-3" visible="false">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h3>Quiz Text</h3>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="TypingSpeedUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="TypingSpeedUpdatePanel">
                    <ContentTemplate>
                        <uc:TypingSpeed runat="server" ID="TypingSpeed" ValidationGroup="Quiz" />
                    </ContentTemplate>
                </insite:UpdatePanel>
                
            </div>
        </div>
    </section>

    <section runat="server" id="TypingAccuracySection" class="mb-3" visible="false">
        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="TypingAccuracyUpdatePanel" />
        <insite:UpdatePanel runat="server" ID="TypingAccuracyUpdatePanel">
            <ContentTemplate>
                <uc:TypingAccuracy runat="server" ID="TypingAccuracy" ValidationGroup="Quiz" />
            </ContentTemplate>
        </insite:UpdatePanel>
    </section>

    <section>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Quiz" />
        <insite:DeleteButton runat="server" ID="DeleteButton" CausesValidation="false" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>
</asp:Content>
