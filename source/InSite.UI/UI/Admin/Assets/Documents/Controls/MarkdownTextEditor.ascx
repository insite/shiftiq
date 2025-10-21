<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MarkdownTextEditor.ascx.cs" Inherits="InSite.UI.Admin.Assets.Documents.Controls.MarkdownTextEditor" %>

<div runat="server" id="TextDropZone">
    <div>
        <insite:TextBox ID="MDText" runat="server" TextMode="MultiLine" />
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            var instance = <%= TextEditorObject %> = {};

            var editorToolbar = ['bold', 'italic', 'heading', '|', 'quote', 'unordered-list', 'ordered-list', '|', 'link', 'image', '|', 'preview', '|', 'guide'];

            var editor = new SimpleMDE({
                element: document.getElementById('<%= MDText.ClientID %>'),
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