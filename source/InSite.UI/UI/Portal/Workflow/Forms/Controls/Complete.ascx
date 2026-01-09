<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Complete.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.Complete" %>

<div class="row">
    <div class="col-lg-12">
        <h1 runat="server" id="SurveyFormTitle" class="py-1 my-2 pb-2 mb-4"></h1>
    </div>
</div>

<insite:Alert runat="server" ID="ErrorAlert" />

<div runat="server" ID="SurveyFormInstructionsAlert" class="mb-4 alert alert-light" style="color:rgb(69,119,150);" >
    <asp:Literal runat="server" ID="SurveyFormInstructions" />
</div>

<div class="mb-4">
    <insite:Button runat="server" ID="BackToCourseButton" Text="Return to Course" Icon="fas fa-arrow-alt-left" ButtonStyle="Default" />
    <insite:Button runat="server" ID="ReviewButton" Text="Review my Submission" Icon="fas fa-arrow-alt-right" IconPosition="AfterText" ButtonStyle="Primary" />
</div>

<asp:Panel runat="server" ID="DebugPanel" Visible="false">

    <hr class="mt-6" />

    <h1 class="text-danger"><i class="far fa-debug me-1"></i> Debug: Complete</h1>

    <ul>
        <li>
            If the form has completed instructions then these are displayed here. 
        </li>
        <li>
            If user feedback is enabled, then the View Feedback button is visible, which navigates to the Review page.
        </li>
    </ul>

</asp:Panel>