<%@ Control Language="C#" CodeBehind="Start.ascx.cs" Inherits="InSite.Portal.Surveys.Responses.Start" %>

<section class="container mb-2 mb-sm-0 pb-sm-5">

    <uc:Breadcrumbs runat="server" ID="Breadcrumbs" />

    <insite:Alert runat="server" ID="ErrorAlert" />

    <div class="row">
        <div class="col-lg-12">
            <h1 runat="server" id="SurveyFormTitle" class="py-1 my-2 pb-2 mb-4"></h1>
        </div>
    </div>

    <div class="mb-4">
        -
    </div>

    <asp:Panel runat="server" ID="DebugPanel" Visible="false">

        <hr class="mt-6" />

        <div class="float-end">
            <insite:Button runat="server" ID="DebugButton" ButtonStyle="Danger" Icon="far fa-rocket-launch" IconPosition="AfterText" Text="Continue" />
        </div>

        <h1 class="text-danger"><i class="far fa-debug me-1"></i>Debug: Start</h1>

        <ul>
            <li>This page has no user interface.
            </li>
            <li>It sends the command to start a survey response session, then automatically jumps to the first page of 
            questions in the survey.
            </li>
        </ul>

    </asp:Panel>

</section>
