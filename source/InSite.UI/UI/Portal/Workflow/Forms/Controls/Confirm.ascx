<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Confirm.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.Confirm" %>

<div class="row">
    <div class="col-lg-12">
        <h1 runat="server" id="SurveyFormTitle" class="py-1 my-2 pb-2 mb-4"></h1>
    </div>
</div>

<insite:Alert runat="server" ID="ErrorAlert" />

<div runat="server" id="InstructionPanel" class="mb-4 alert alert-light">
    <asp:Literal runat="server" ID="SurveyFormInstructions" />
</div>

<div runat="server" id="ButtonPanel" class="mb-4">
    <insite:Button runat="server" ID="PreviousButton" ButtonStyle="Default" Text="Previous" Icon="fas fa-arrow-alt-left" />
    <insite:Button runat="server" ID="SubmitButton" ButtonStyle="Success" Icon="fas fa-check" Text="Submit My Submission" DisableAfterClick="true" />
</div>

<asp:Panel runat="server" ID="DebugPanel" Visible="false">

    <hr class="mt-6" />

    <h1 class="text-danger"><i class="far fa-debug me-1"></i> Debug: Confirm</h1>

    <ul>
        <li>
            This page prompts the user to confirm final submission of their completed submission. After a response is 
            completed it is automatically locked, and the user cannot modify any answers to their questions in the
            submission.
        </li>
    </ul>

</asp:Panel>
