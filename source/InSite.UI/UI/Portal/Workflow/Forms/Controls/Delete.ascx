<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Delete.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.Delete" %>

<div class="row">
    <div class="col-lg-12">
        <h1 runat="server" id="SurveyFormTitle" class="py-1 my-2 pb-2 mb-4"></h1>
    </div>
</div>

<insite:Alert runat="server" ID="ErrorAlert" />

<div class="mb-4 alert alert-danger">
    <i class="fas fa-stop-circle"></i> <strong><%= Translate("Confirm") %>:</strong>
    <%= Translate("Are you sure you want to delete this submission session?") %>
</div>

<insite:DeleteButton runat="server" ID="DeleteButton" ValidationGroup="Void" />
<insite:CancelButton runat="server" ID="CancelButton" />

<asp:Panel runat="server" ID="DebugPanel" Visible="false">

    <hr class="mt-6" />

    <h1 class="text-danger"><i class="far fa-debug me-1"></i> Debug: Delete</h1>
        
    <ul>
        <li>
            Here we need to display information about the selected submission and prompt the user to confirm the delete.
        </li>
    </ul>

</asp:Panel>
