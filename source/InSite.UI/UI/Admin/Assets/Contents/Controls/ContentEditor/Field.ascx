<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Field.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.ContentEditor.Field" %>

<%@ Import Namespace="Newtonsoft.Json" %>

<div class="form-group content-field" id="<%= ClientID %>">
    <label runat="server" id="TitleLabel">
        <asp:Literal runat="server" id="TitleOutput" />
        <insite:CustomValidator runat="server" ID="ClientStateRequiredValidator" Enabled="false" Display="Dynamic" />
    </label>
    <div>
        <insite:DynamicControl runat="server" ID="Input" />
        <div style="margin-top:3px;">
            <insite:EditorTranslation runat="server" ID="EditorTranslation" TableContainerID="TranslationContainer" />
            <insite:Button runat="server" ID="CleanTextButton" ButtonStyle="Default" Size="ExtraSmall" ToolTip="Clean Text" Icon="fas fa-sparkles" Text="Clean" PostBackEnabled="false" Visible="false" />
        </div>
        <insite:EditorUpload runat="server" ID="EditorUpload" Visible="false" />
        <div runat="server" id="TranslationContainer"></div>
    </div>
    
</div>

<insite:PageFooterContent runat="server" ID="CommonScript" RenderRequired="true">
    <script type="text/javascript">
        (function () {
            var instance = window.contentEditorField = window.contentEditorField || {};

            instance.init = function (options) {
                var btn = document.getElementById(options.cleanTextId);
                if (!btn)
                    return;

                var $btn = $(btn);
                if (typeof inSite.common.getObjByName('$.summernote.external.cleanText') !== 'function') {
                    $btn.remove();
                    return;
                }
                
                $btn.data('textId', options.textId)
                    .off('click', onCleanTextClick)
                    .on('click', onCleanTextClick);
            };

            function onCleanTextClick(e) {
                e.preventDefault();

                var $btn = $(this);

                var textId = $btn.data('textId');
                if (!textId)
                    return;

                var text = document.getElementById(textId);
                if (!text)
                    return;

                if (!confirm('Are you sure you want to clean the text?'))
                    return;

                var $text = $(text);

                $text.summernote('code', $.summernote.external.cleanText($text.summernote('code'), {
                    newLine: '\n'
                }))
            }
        })();
    </script>
</insite:PageFooterContent>
