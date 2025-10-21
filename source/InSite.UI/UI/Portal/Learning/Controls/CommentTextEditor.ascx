<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentTextEditor.ascx.cs" Inherits="InSite.UI.Portal.Learning.Controls.CommentTextEditor" %>

<insite:Alert runat="server" ID="CommentTextAlert" />

<div runat="server" id="TextDropZone">
    <div>
        <insite:TextBox ID="CommentText" runat="server" TextMode="MultiLine" />
    </div>
    <div class="fs-xs text-body-secondary mt-2">
        Please remember posted comments cannot be modified or deleted by learners. 
        Administrators and instructors reserve the right to remove any comments.
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            var instance = <%= TextEditorObject %> = {};

            var editorToolbar = ['bold', 'italic', 'heading', '|', 'quote', 'unordered-list', 'ordered-list', '|', 'link', 'image', '|', 'preview', '|', 'guide'];

            var editor = new SimpleMDE({
                element: document.getElementById('<%= CommentText.ClientID %>'),
                forceSync: true,
                spellChecker: false,
                autoDownloadFontAwesome: false,
                parsingConfig: {
                    strikethrough: false
                },
                renderingConfig: {
                    singleLineBreaks: false
                },
                status: false,
                toolbar: editorToolbar
            });

            $(editor.codemirror.display.wrapper).parents('.panel-group').on('shown.bs.collapse', function () {
                refreshEditor();
            });

            $(editor.codemirror.display.wrapper).parents('.tab-pane').each(function () {
                $('[data-bs-target="#' + this.id + '"][data-bs-toggle]').on('shown.bs.tab', function () {
                    refreshEditor();
                });
            });
            
            function refreshEditor() {
                if ($(editor.codemirror.display.wrapper).is(':visible'))
                    editor.codemirror.refresh();
            }
        })();

    </script>
</insite:PageFooterContent>