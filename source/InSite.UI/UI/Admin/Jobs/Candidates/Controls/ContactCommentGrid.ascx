<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactCommentGrid.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.Controls.ContactCommentGrid" %>

<%@ Register Src="AuthorCommentGrid.ascx" TagName="AuthorCommentGrid" TagPrefix="uc" %>
<%@ Register Src="SubjectCommentGrid.ascx" TagName="SubjectCommentGrid" TagPrefix="uc" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div runat="server" id="AuthorGridPanel">
            <h3 runat="server" id="AuthorGridTitle">Author</h3>
            <uc:AuthorCommentGrid runat="server" ID="AuthorGrid" />
        </div>

        <div runat="server" id="SubjectGridPanel">
            <h3 runat="server" id="SubjectGridTitle">Subject</h3>
            <uc:SubjectCommentGrid runat="server" ID="SubjectGrid" />
        </div>

        <div runat="server" id="ButtonPanel" style="padding-top: 20px;">
            <insite:Button runat="server" ID="AddButton" ButtonStyle="Default" Text="Add Comment" Icon="fas fa-plus-circle" />
            <asp:Button ID="RefreshButton" runat="server" style="display: none;" />
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:Modal runat="server" ID="CreatorWindow" Title="Add New Comment" Width="550px" MinHeight="200px" />
<insite:Modal runat="server" ID="EditorWindow" Title="Edit Comment" Width="550px" MinHeight="200px" />

<insite:PageFooterContent runat="server" ID="ScriptLiteral">
    <script type="text/javascript">

        var contactCommentGrid = {
            showCreator: function (candidateId) {
                var wnd = modalManager.load('<%= CreatorWindow.ClientID %>', '/ui/admin/jobs/candidates/add-comment?candidate=' + String(candidateId));
                $(wnd).one('closed.modal.insite', contactCommentGrid.onWindowClose);
            },
            showEditor: function (commentId) {
                var wnd = modalManager.load('<%= EditorWindow.ClientID %>', '/ui/admin/jobs/candidates/edit-comment?comment=' + String(commentId));
                $(wnd).one('closed.modal.insite', contactCommentGrid.onWindowClose);
            },
            onWindowClose: function (e, s, a) {
                if (a == 'refresh')
                    __doPostBack('<%= RefreshButton.UniqueID %>', '');
            }
        };

    </script>
</insite:PageFooterContent>