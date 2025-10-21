<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TranslationControl.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.TranslationControl" %>

<div class="row">

    <div class="col-md-6">
        <h2 style="margin:0">Default Text</h2>
        <div class="row" style="margin-top: 10px; margin-bottom: 10px;">
            <div class="col-xs-12">
                <insite:LanguageComboBox runat="server" ID="DefaultLanguageSelector" Width="120px" Enabled="false" ZIndex="8501" />
                <insite:RequiredValidator runat="server" ID="RequiredValidator" ControlToValidate="DefaultText" ValidationGroup="TranslationControl" Enabled="false" />
            </div>
        </div>
        <div class="row">
            <div class="col-xs-12">
                <insite:TextBox runat="server" ID="DefaultText" Width="100%" />
                <div runat="server" id="DefaultLiteral" style="padding: 20px; border: solid 1px #ddd;"></div>
            </div>
        </div>
    </div>

    <div runat="server" id="TranslationColumn" class="col-md-6">
        <h2 style="margin:0">Translation(s)</h2>
        <div class="row" style="margin-top: 10px; margin-bottom: 10px;">
            <div class="col-xs-12">
                <insite:LanguageComboBox runat="server" ID="TranslatedLanguageSelector" Width="120" AllowBlank="False" ZIndex="8501" />
                <insite:IconButton runat="server" ID="RequestGoogleTranslation"
                    Name="globe"
                    ToolTip="Request Google Translation"
                    ConfirmText="Are you sure you want a Google translation of this label?" />
            </div>
        </div>
        <div runat="server" id="TranslatedTextRow" class="row">
            <div class="col-xs-12">
                <insite:TextBox runat="server" ID="TranslatedText" Width="100%" />
                <div runat="server" id="OrganizationSpecificRow" style="margin-top: 10px; margin-bottom: 10px;">
                    <insite:CheckBox runat="server" ID="IsOrganizationSpecific" Enabled="false" />
                </div>
                <div style="margin-top: 10px;">
                    <asp:Repeater runat="server" ID="OtherTranslations">
                        <HeaderTemplate>
                            <table>
                        </HeaderTemplate>
                        <FooterTemplate></table></FooterTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style='font-weight:bold; width:50px; padding:5px; vertical-align:top;'>
                                    <asp:LinkButton runat="server" ID="SelectLinkButton" Text='<%# Eval("LanguageCode") %>' />
                                    <strong><asp:Literal runat="server" ID="SelectLiteral" Text='<%# Eval("LanguageCode") %>' /></strong>
                                </td>
                                <td style='padding:5px; word-break:break-word;'><%# Eval("Html") %></td>
                                <td style="padding: 5px; vertical-align: top;">
                                    <insite:IconButton runat="server" ID="DeleteTranslationCommand" Name="trash-alt" ToolTip="Delete Translation"
                                        CommandName="DeleteTranslation" CommandArgument='<%# Eval("LanguageCode") %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField runat="server" ID="SelectedLanguageCode" />
    <asp:Button runat="server" ID="SelectButton" Style="display: none;" />
</div>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">

        (function () {
            var instance = window.translationControl = window.translationControl || {};

            instance.initEditor = function initEditor(id) {
                var $input = $('#' + String(id));
                if ($input.length !== 1 || $input.data('simple-mde'))
                    return;

                var editor = new SimpleMDE({
                    element: $input[0],
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
                    toolbar: [
                        'bold',
                        'italic',
                        'heading',
                        '|',
                        'quote',
                        'unordered-list',
                        'ordered-list',
                        '|',
                        'link',
                        'image',
                        '|',
                        'preview',
                        '|',
                        'guide'
                    ],
                });

                $input.data('simple-mde', editor);
            };

            instance.refreshEditor = function (id) {
                var editorSelector = '';
                if (typeof id === 'string' && id.length > 0)
                    editorSelector = '#' + String(id);

                var editor = $(editorSelector).data('simple-mde');
                if (editor instanceof SimpleMDE && $(editor.codemirror.display.wrapper).is(':visible'))
                    editor.codemirror.refresh();
            };
        })();

    </script>
</insite:PageFooterContent>
