<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GoTo.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.GoTo" %>

<section class="container mb-2 mb-sm-0 pb-sm-5">

    <uc:Breadcrumbs runat="server" ID="Breadcrumbs" />

    <insite:Alert runat="server" ID="ErrorAlert" />

    <div class="row">
        <div class="col-lg-12">
            <h1 runat="server" id="SurveyFormTitle" class="py-1 my-2 pb-2 mb-4"></h1>
        </div>
    </div>

    <div runat="server" id="InfoPanel" class="mb-4" visible="false">
        Go To <strong><%= GoToType %> Page</strong> ... which is ... <strong>Page <%= GoToInfo.Page %></strong>
    </div>

    <insite:Pagination runat="server" ID="Pagination" Description="" />

    <asp:Panel runat="server" ID="DebugPanel" Visible="false">

        <hr class="mt-6" />

        <div class="float-end">
            <insite:Button runat="server" ID="DebugButton" ButtonStyle="danger" Icon="far fa-rocket-launch" IconPosition="AfterText" Text="Continue" />
        </div>

        <h1 class="text-danger"><i class="far fa-debug me-1"></i>Debug: Go To</h1>

        <ul>
            <li>If Debug mode is enabled, then this page is visible for testing and debugging purposes.
            </li>
            <li>This page displays a pagination control with the current page indicated.
            </li>
            <li>It calculates the next page, based on the current page and the current submission.
            </li>
            <li>If Debug mode is disabled, then this page automatically jumps to the Next or Previous page, based on
            the branches defined for the form, and based on the answers submitted by the user on preceding questions.
            </li>
        </ul>

    </asp:Panel>

</section>
