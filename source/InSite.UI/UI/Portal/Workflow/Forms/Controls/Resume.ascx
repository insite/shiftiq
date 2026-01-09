<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Resume.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.Resume" %>

<section class="container mb-2 mb-sm-0 pb-sm-5">

    <uc:Breadcrumbs runat="server" ID="Breadcrumbs" />

    <div class="mt-5">
        <insite:Alert runat="server" ID="ErrorAlert" />
    </div>

    <div class="row">
        <div class="col-lg-12">
            <h1 runat="server" id="SurveyFormTitle" class="py-1 my-2 pb-2 mb-4"></h1>
        </div>
    </div>

    <asp:Panel runat="server" ID="DebugPanel" Visible="false">

        <hr class="mt-6" />

        <div class="float-end">
            <insite:Button runat="server" ID="DebugButton" ButtonStyle="Danger" Icon="far fa-rocket-launch" IconPosition="AfterText" Text="Continue" />
        </div>

        <h1 class="text-danger"><i class="far fa-debug me-1"></i>Debug: Resume</h1>

        <ul>
            <li>This page has no user interface.
            </li>
            <li>It automatically jumps to the form page that contains the last question 
            answered by the user.
            </li>
            <li>If the user has not answered any questions then this page jumps to the first page of the form.
            </li>
        </ul>

    </asp:Panel>

</section>
