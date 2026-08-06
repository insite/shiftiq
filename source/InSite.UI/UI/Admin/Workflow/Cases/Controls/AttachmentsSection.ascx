<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentsSection.ascx.cs" Inherits="InSite.Admin.Issues.Outlines.Controls.AttachmentsSection" %>

<%@ Register TagPrefix="uc" TagName="CaseDocumentList" Src="CaseDocumentList.ascx" %>
<%@ Register TagPrefix="uc" TagName="CaseFileRequirementList" Src="CaseFileRequirementList.ascx" %>

<div class="mt-4 mb-4">
    <insite:Button runat="server" ID="AddAttachmentButton" ButtonStyle="Default" Text="Attach Document" Icon="fas fa-upload" />
    <insite:Button runat="server" ID="AddRequestButton" ButtonStyle="Default" Text="Request Document" Icon="fas fa-upload" />
    <insite:Button runat="server" ID="CopyDocumentsButton" ButtonStyle="Default" Text="Copy Documents" Icon="fas fa-copy" PostBackEnabled="false" />
</div>

<div runat="server" id="RequestsSection" class="card border-0 shadow-lg mb-3">
    <div class="card-body">
        <h3>Requests</h3>
        <uc:CaseFileRequirementList runat="server" ID="CaseFileRequirementList" />
    </div>
</div>

<div class="card border-0 shadow-lg">
    <div class="card-body">
        <h3>Documents</h3>
        <uc:CaseDocumentList runat="server" ID="CaseDocumentList" ClientSelectCallback="attachmentsSection.onDocSelect" />
    </div>
</div>

<insite:FindCase runat="server" ID="CopyDocumentsCaseSelector" Output="None" AllowClear="false" CloseOnSelect="false" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            if (window.attachmentsSection)
                return;

            const instance = window.attachmentsSection = {}
            const btnAddAttachment = document.getElementById('<%= AddAttachmentButton.ClientID %>');
            const btnAddRequest = document.getElementById('<%= AddRequestButton.ClientID %>');
            const btnCopyDocs = document.getElementById('<%= CopyDocumentsButton.ClientID %>');

            btnCopyDocs.addEventListener('click', onCopyDocumentsClick);

            instance.onDocSelect = function (data) {
                if (data.checkedCount > 0) {
                    btnAddAttachment.classList.add('disabled');
                    btnAddRequest.classList.add('disabled');
                    btnCopyDocs.classList.remove('disabled');
                } else {
                    btnAddAttachment.classList.remove('disabled');
                    btnAddRequest.classList.remove('disabled');
                    btnCopyDocs.classList.add('disabled');
                }
            };

            function onCopyDocumentsClick() {
                document.getElementById('<%= CopyDocumentsCaseSelector.ClientID %>').show();
            }
        })();
    </script>
</insite:PageFooterContent>
