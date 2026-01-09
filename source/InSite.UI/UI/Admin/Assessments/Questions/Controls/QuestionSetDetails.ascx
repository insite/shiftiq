<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionSetDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionSetDetails" %>

<%@ Register TagPrefix="uc" TagName="QuestionRepeater" Src="~/UI/Admin/Assessments/Questions/Controls/QuestionRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="SetDetails" Src="~/UI/Admin/Assessments/Sets/Controls/SetDetails.ascx" %>

<insite:Nav runat="server" ID="TabsNav">
            
    <insite:NavItem runat="server" ID="QuestionNavItem" Title="Question">
        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="QuestionsUpdatePanel" />
        <insite:UpdatePanel runat="server" ID="QuestionsUpdatePanel">
            <ContentTemplate>
                <uc:QuestionRepeater runat="server" ID="QuestionRepeater" />

                <insite:Button runat="server" ID="LoadQuestionsButton" Visible="false" ButtonStyle="Success" CssClass="w-100 mt-3" Text="Load All Questions" Icon="fas fa-spinner" />
            </ContentTemplate>
        </insite:UpdatePanel>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="SetNavItem" Title="Set">
        <uc:SetDetails runat="server" ID="SetDetails" />
    </insite:NavItem>

</insite:Nav>
