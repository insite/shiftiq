<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Launch.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.Launch" %>

<%@ Register TagPrefix="uc" TagName="SubmissionRepeater" Src="~/UI/Portal/Workflow/Forms/Controls/SubmissionRepeater.ascx" %>

<style type="text/css">

    .alert p {
        margin:0;
    }

</style>

<div class="row">
    <div class="col-lg-12">
        <h1 runat="server" id="SurveyFormTitle" class="py-1 my-2 pb-2 mb-4"></h1>
    </div>
</div>

<div class="alert">
    <insite:Alert runat="server" ID="ErrorAlert" />
</div>

<asp:Panel runat="server" ID="SessionPanel" Visible="false">

    <div runat="server" ID="SurveyFormInstructions" class="mb-4">
        
    </div>

    <div class="mb-5">
        <insite:Button runat="server" ID="StartButton" ButtonStyle="Default" Text="Start Form" Icon="fas fa-rocket-launch" />
    </div>
    
    <asp:Panel runat="server" ID="ResponsePanel" CssClass="mb-4">

        <hr />
        <h3 class="mb-0"><%= Translate("Form Submissions") %></h3>
        <p><small class="text-body-secondary"><asp:Literal runat="server" ID="ResponseCount" /></small></p>

        <uc:SubmissionRepeater runat="server" ID="SubmissionRepeater" />

    </asp:Panel>

</asp:Panel>

<asp:Panel runat="server" ID="DebugPanel" Visible="false">

    <hr class="mt-6" />
    
    <div class="float-end">
        <insite:Button runat="server" ID="DebugButton" ButtonStyle="Danger" Icon="far fa-rocket-launch" IconPosition="AfterText" Text="Continue" />
    </div>

    <h1 class="text-danger"><i class="far fa-debug me-1"></i> Debug: Launch</h1>

    <ul>
        <li>
            If the form has starting instructions then display them.
        </li>
        <li>
            If the form requires User Identification, and if the respondent is not known 
            <code>(Current.User == UserIdentifiers.Someone)</code>, 
            then display an error message.
        </li>
        <li>
            If the form requires User Authentication, and if the current user is not authenticated, 
            then display an error message.
        </li>
        <li>
            If the user has existing submissions to the form then display them in a table. For each submission, if 
            it is not complete then display a Resume button; otherwise display a Review button. 
            Users can delete (and then optionally restart) a submission if the sesponse is not yet Completed (and Locked).
        </li>
        <li>
            If the form allows multiple submissions from the same user, then show a Start Form button.
        </li>
        <li>
            If the form does not allow multiple submissions from the same user, and if the user has not yet created a
            first submission, then show a Start Form button.
        </li>
        <li>
            If the form has no starting instructions and no existing submissions, then then start the submission
            automatically.
        </li>
    </ul>

</asp:Panel>
