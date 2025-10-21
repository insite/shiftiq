<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionTextEditor.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionTextEditor" %>

<%@ Register TagPrefix="uc" TagName="ImageRepeater" Src="../../Attachments/Controls/ImageRepeater.ascx" %>

<div class="form-group mb-3">
    <div>
        <insite:MarkdownEditor runat="server" ID="QuestionText"
            UploadControl="EditorUpload"
            TranslationControl="EditorTranslation"
            ClientEvents-OnSetup="questionTextEditor.onSetup"
        />
    </div>
    <div class="mt-1">
        <insite:EditorTranslation runat="server" ID="EditorTranslation" TableContainerID="TranslationContainer" EnableMarkdownConverter="true" />
    </div>
    <insite:EditorUpload runat="server" ID="EditorUpload" Mode="Custom" FileExtensions="doc,docx,gif,jpg,jpeg,pdf,png,xls,xlsx,zip" />
    <div runat="server" id="TranslationContainer"></div>
</div>

<insite:Modal runat="server" ID="ImageSelectorWindow" Title="Select Image" Width="980px">
    <ContentTemplate>
        <div class="p-2">

            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ImageSelectorUpdatePanel" />

            <insite:UpdatePanel runat="server" ID="ImageSelectorUpdatePanel">
                <ContentTemplate>
                    <div runat="server" id="ImageSelectorGallery">
                        <h3>Images</h3>
                        <uc:ImageRepeater runat="server" ID="ImageSelectorRepeater" />
                    </div>

                    <div runat="server" id="ImageSelectorEmptyMessage" class="text-center fs-2 my-4" visible="false">
                        No Images
                    </div>

                    <asp:Button runat="server" ID="ImageSelectorRefresh" CssClass="d-none" />
                </ContentTemplate>
            </insite:UpdatePanel>

        </div>
    </ContentTemplate>
</insite:Modal>

<insite:PageFooterContent runat="server" ID="CommonScript">
<script type="text/javascript">

    (function () {
        const instance = window.questionTextEditor = window.questionTextEditor || {};

        const mapping = {};

        instance.init = function (id, textId, translationId, imgsWindowId, imgsUpdateUid) {
            const textEl = document.getElementById(textId);
            if (textEl == null)
                return;

            const $text = $(textEl);
            if ($text.data('qText'))
                return;

            $text.data('qText', {
                tId: translationId,
                wndId: imgsWindowId,
                btnUid: imgsUpdateUid
            });

            mapping[id] = textId;
        };

        instance.focus = function (id) {
            if (!mapping.hasOwnProperty(id))
                return;

            const textId = mapping[id];
            const editor = inSite.common.markdownEditor.getEditor(textId);
            if (!editor)
                return;

            editor.codemirror.focus();

            const lineCount = editor.codemirror.lineCount();
            if (lineCount <= 0)
                return;

            const lineLength = editor.codemirror.getLine(0).length;

            editor.codemirror.setCursor({ line: lineCount - 1, ch: lineLength })
        };

        instance.setTranslation = function (id, value) {
            if (typeof value != 'object' || !mapping.hasOwnProperty(id))
                return;

            const textId = mapping[id];
            const $text = $(document.getElementById(textId))

            const data = $text.data('qText');
            if (!data)
                return;

            inSite.common.editorTranslation.setState(data.tId, value);
        };

        instance.getTranslation = function (id, value) {
            if (!mapping.hasOwnProperty(id))
                return;

            const textId = mapping[id];
            const $text = $(document.getElementById(textId))

            const data = $text.data('qText');
            if (!data)
                return;

            return inSite.common.editorTranslation.getState(data.tId);
        };

        instance.onSetup = function (mde, options) {
            mde.toolbar = [
                'bold', 'italic', 'heading',
                '|',
                'quote', 'unordered-list', 'ordered-list',
                '|',
                'link', 'image',
                {
                    name: 'select-image',
                    action: onSelectImageOpen,
                    className: 'far fa-images',
                    title: 'Select Uploaded Image',
                },
                '|',
                'preview',
                '|',
                'guide'
            ];

            <% if (CurrentData.OrganizationIdentifier == OrganizationIdentifiers.SkilledTradesBC) { %>
            mde.toolbar.push('|');
            mde.toolbar.push({
                name: 'commentary-edit',
                action: function (editor) {
                    alert('NOT IMPLEMENTED');
                },
                className: 'far fa-comment-edit',
                title: 'Edit with Comments',
            });
            <% } %>

            options.onSetText = onEditorSetText;
            options.onGetText = onEditorGetText;
            options.onPreview = onEditorGetText;
        };

        instance.initImgSelector = function (textId, windowId) {
            $('#' + windowId + ' .file-preview a').on('click', function (e) {
                onImageSelectorClick.call(this, e, textId, windowId);
            });
        };

        instance.toInSiteMarkdown = toInSiteMarkdown;

        instance.fromInSiteMarkdown = fromInSiteMarkdown;

        function toInSiteMarkdown(text) {
            if (!text)
                return '';

            return text
                .replaceAll(/(&(o|c)cb;|{|})/g, function (m) {
                    if (m.length > 1)
                        return '&' + m;

                    if (m == '{')
                        return '&ocb;';

                    if (m == '}')
                        return '&ccb;';

                    return m;
                })
                .replaceAll(/[^\s]+(&nbsp;[^\s]*)+/g, function (m) {
                    return '{' + m.replaceAll(/&nbsp;/g, ' ') + '}';
                })
                .replaceAll(/&?&(o|c)cb;/g, function (m) {
                    if (m.length > 5)
                        return m.substring(1);

                    if (m == '&ocb;')
                        return '{{';

                    if (m == '&ccb;')
                        return '}}';

                    return m;
                })
                ;
        }

        function fromInSiteMarkdown(text) {
            if (!text)
                return '';

            return text
                .replaceAll(/(&(o|c)cb;|{{|}})/g, function (m) {
                    if (m.length > 2)
                        return '&' + m;

                    if (m == '{{')
                        return '&ocb;';

                    if (m == '}}')
                        return '&ccb;';

                    return m;
                })
                .replaceAll(/\{[^\{\}]*\}/g, function (m) {
                    return m.substring(1, m.length - 1).replaceAll(/\s/g, '&nbsp;')
                })
                .replaceAll(/&?&(o|c)cb;/g, function (m) {
                    if (m.length > 5)
                        return m.substring(1);

                    if (m == '&ocb;')
                        return '{';

                    if (m == '&ccb;')
                        return '}';

                    return m;
                })
                ;
        }

        function onEditorSetText(o) {
            o.text = toInSiteMarkdown(o.text);
        }

        function onEditorGetText(o) {
            o.text = fromInSiteMarkdown(o.text);
        };

        function onSelectImageOpen(editor) {
            const $element = $(editor.codemirror.getTextArea());
            const data = $element.data('qText');

            modalManager.show(data.wndId);

            __doPostBack(data.btnUid, '');
        }

        function onImageSelectorClick(e, textId, windowId) {
            e.preventDefault();
            e.stopPropagation();

            const $this = $(this);
            const url = encodeURI($this.attr('href'));
            const $thumbnail = $this.closest('.file-thumbnail');

            let title = '';

            const $title = $thumbnail.find('> span.file-title');
            if ($title.length == 1)
                title = $title.text().trim();

            if (!title) {
                const $name = $thumbnail.find('> span.file-name');
                if ($name.length == 1)
                    title = $name.text().trim();
            }

            inSite.common.markdownEditor.onFileUploaded({
                textId: textId,
                title: title,
                url: url
            });

            modalManager.close(windowId);
        }
    })();

</script>
</insite:PageFooterContent>