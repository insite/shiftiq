<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Review.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.Review" %>

<%@ Register TagPrefix="uc" TagName="ReviewDetails" Src="./ReviewDetails.ascx" %>

<div class="row">
    <div class="col-lg-12">
        <h1 runat="server" id="SurveyFormTitle" class="py-1 my-2 pb-2 mb-4"></h1>
    </div>
</div>

<insite:Alert runat="server" ID="ErrorAlert" />

<div runat="server" id="ReturnPanel" class="btn-group me-1" role="group" visible="false">
    <insite:Button runat="server" ID="BackToCourseButton" ButtonStyle="Default" Text="Return to Course" Icon="fas fa-arrow-alt-left" Visible="false" />
    <insite:Button runat="server" ID="BackToRegistrationButton" ButtonStyle="Default" Text="Return to Registration" Icon="fas fa-arrow-alt-left" Visible="false" />
</div>

<insite:Button runat="server" ID="DownloadSummaryButton" ButtonStyle="Default" Text="Download Summary" CssClass="ms-1" Icon="fas fa-download" Visible="false" />
<insite:Button runat="server" ID="DownloadDetailsButton" ButtonStyle="Default" Text="Download Details" CssClass="ms-1" Icon="fas fa-download"  Visible="false" />
<insite:Button runat="server" ID="DownloadAnsweredButton" ButtonStyle="Default" Text="Download Details, answered questions only" CssClass="ms-1" Icon="fas fa-download"  Visible="false" />
<insite:Button runat="server" ID="HomeButton" ButtonStyle="Success" Text="Home" CssClass="ms-1" Icon="fas fa-home" NavigateUrl="/ui/portal/home" />

<div style="clear:both;"></div>
    
<uc:ReviewDetails runat="server" ID="Details" />

<asp:Panel runat="server" ID="DebugPanel" Visible="false">

    <hr class="mt-6" />

    <h1 class="text-danger"><i class="far fa-debug me-1"></i> Debug: Review</h1>

    <ul>
        <li>
            This page displays all the user's answers to the questions in this survey.
        </li>
        <li>
            If user feedback is enabled, then administrator feedback to the respondent is also included.
            (Note this is called the "Feedback Report" in Surveys I.)
        </li>
    </ul>

</asp:Panel>
