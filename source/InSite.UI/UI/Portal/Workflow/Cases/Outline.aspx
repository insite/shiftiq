<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Portal.Issues.Outline" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagName="CaseFileRequirementList" TagPrefix="uc" Src="Controls/CaseFileRequirementList.ascx" %>
<%@ Register TagName="CaseDocumentList" TagPrefix="uc" Src="Controls/CaseDocumentList.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <div class="row">

    <div class="col-lg-12">

        <h2>
            <asp:Literal runat="server" ID="IssueType" />
            <span class="ms-1 me-1">&raquo;</span>
            <asp:Literal runat="server" ID="IssueStatusName" />
        </h2>

        <p>
            <asp:Literal runat="server" ID="IssueTitle" />
        </p>

        <asp:Literal runat="server" ID="IssueDescriptionHtml" />

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AttachmentPanel" />
        <insite:UpdatePanel runat="server" ID="AttachmentPanel">
            <ContentTemplate>

                <div runat="server" id="RequestPanel" class="card shadow mt-3">

                    <div class="card-body">

                        <h3 class="card-title">Requests</h3>

                        <uc:CaseFileRequirementList runat="server" ID="CaseFileRequirementList" />

                    </div>

                </div>

                <div runat="server" id="NewAttachmentPanel" class="card shadow mt-3">

                    <div class="card-body">

                        <h3 class="card-title">Attachments</h3>

                        <uc:CaseDocumentList runat="server" ID="CaseDocumentList" />

                    </div>

                </div>

            </ContentTemplate>
        </insite:UpdatePanel>

    </div>

</div>


</asp:Content>
