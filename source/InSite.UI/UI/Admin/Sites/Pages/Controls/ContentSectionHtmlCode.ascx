<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentSectionHtmlCode.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.ContentSectionHtmlCode" %>

<div class="form-group mb-3" id="<%= ClientID %>">
    <label class="form-label">
        <%= string.IsNullOrEmpty(Title) ? "&nbsp;" : HttpUtility.HtmlEncode(Title) %>
        <insite:RequiredValidator runat="server" ID="RequiredValidator" ControlToValidate="Value" Visible="false" Display="Dynamic" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="Value" Width="100%" TextMode="MultiLine" />
    </div>
    <div class="form-text"><%= Description %></div>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        div.html-code-editor {
            height: 500px;
        }

            div.html-code-editor > div {
                padding: 6px 12px;
                border: 1px solid #ccc;
                border-radius: 4px;
                width: 100%;
                height: 100%;
            }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <insite:ResourceLink runat="server" Type="JavaScript" Url="/UI/Layout/common/parts/plugins/ace.cloud9/ace.js" />

    <script type="text/javascript">
        (function () {
            var instance = window.sectionHtmlCode = window.sectionHtmlCode || {};

            instance.init = function (selector) {
                $(selector).each(function () {
                    var $input = $(this);
                    if ($input.data('inited') === true)
                        return;

                    var $input = $input.hide();
                    var $wrapper = $('<div class="html-code-editor">');
                    var $editor = $('<div>');

                    $wrapper.insertAfter($input);
                    $wrapper.append($editor);

                    var editor = ace.edit($editor[0], {
                        minLines: 15
                    });

                    editor.$blockScrolling = Infinity;
                    editor.setFontSize(15);
                    editor.setShowPrintMargin(false);
                    editor.session.setMode('ace/mode/html');

                    editor.session.setValue($input.val());
                    editor.session.on('change', function () {
                        $input.val(editor.session.getValue());
                    });

                    $input.data('inited', true);
                });
            };
        })();
    </script>
</insite:PageFooterContent>
