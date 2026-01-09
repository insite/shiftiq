<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionOrderingDetails.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Questions.Controls.QuestionOrderingDetails" %>

<%@ Register TagPrefix="uc" TagName="ImageRepeater" Src="../../Attachments/Controls/ImageRepeater.ascx" %>

<div class="card border-0 shadow-lg h-100 mb-3">
    <div class="card-body d-flex flex-column">

        <h3>Options</h3>

        <div runat="server" id="TopLabelField" class="form-group mb-3" visible="false">
            <label class="form-label">Top Label</label>
            <div>
                <insite:TextBox runat="server" TranslationControl="TopLabel" AllowHtml="true" />
                <div class="mt-1">
                    <insite:EditorTranslation runat="server" ID="TopLabel" ClientEvents-OnSetText="optionWriteRepeater.onSetTitleTranslation" ClientEvents-OnGetText="optionWriteRepeater.onGetTitleTranslation" />
                </div>
            </div>
        </div>

        <asp:Repeater runat="server" ID="OptionRepeater">
            <HeaderTemplate>
                <table id="<%# OptionRepeater.ClientID %>" class="table table-striped table-write-options"><tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Literal runat="server" ID="Key" Text='<%# Eval("Key") %>' Visible="false" />

                <tr data-id='<%# Eval("Key") %>'>
                    <td class="text-end" style="width:65px;">
                        <strong><%# (int)Eval("Index") + 1 %></strong>
                        <insite:RequiredValidator runat="server" ID="TextRequiredValidator" FieldName="Option Text" ControlToValidate="OptionText" />
                    </td>
                    <td>
                        <div class="position-relative">
                            <div>
                                <insite:MarkdownEditor runat="server" ID="OptionEditor"
                                    TranslationControl="OptionText"
                                    UploadControl="OptionUpload"
                                    ClientEvents-OnSetup="orderingDetails.onOptionTextSetup" />
                            </div>
                            <div class="mt-1">
                                <insite:EditorTranslation runat="server" ID="OptionText"
                                    TableContainerID="TranslationContainer"
                                    EnableMarkdownConverter="true"
                                    Text='<%# Eval("Text") %>' />
                            </div>
                            <insite:EditorUpload runat="server" ID="OptionUpload" Mode="Custom" FileExtensions="gif,jpg,jpeg,png" />
                            <div runat="server" id="TranslationContainer"></div>
                        </div>
                    </td>
                    <td class="text-end" style="width:76px;">
                        <span class="start-sort">
                            <i class="fas fa-sort"></i>
                        </span>
                    </td>
                    <td class="text-nowrap" style="width:30px;">
                        <span style="line-height: 28px;">
                            <insite:IconButton runat="server" CommandName="Delete" Name="trash-alt" ToolTip="Delete" Visible='<%# !(bool)Eval("IsReadOnly") %>' />
                        </span>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
        </asp:Repeater>

        <asp:HiddenField runat="server" ID="OptionsOrder" ViewStateMode="Disabled" />

        <div runat="server" id="BottomLabelField" class="form-group mb-3" visible="false">
            <label class="form-label">Bottom Label</label>
            <div>
                <insite:TextBox runat="server" TranslationControl="BottomLabel" AllowHtml="true" />
                <div class="mt-1">
                    <insite:EditorTranslation runat="server" ID="BottomLabel" ClientEvents-OnSetText="optionWriteRepeater.onSetTitleTranslation" ClientEvents-OnGetText="optionWriteRepeater.onGetTitleTranslation" />
                </div>
            </div>
        </div>

        <div class="mt-3">
            <insite:Button runat="server" ID="AddOptionButton" Icon="fas fa-plus-circle" Text="Add New Option" ButtonStyle="Default" />
            <insite:Button runat="server" ID="ToggleLabelsButton" ButtonStyle="Default" />
        </div>

    </div>
</div>

<div class="card border-0 shadow-lg h-100 mt-3">
    <div class="card-body">

        <h3 class="table-write-header">
            <span style="width:335px;">
                <span style="width:110px;">Points</span>
                <span>Cut Score (%)</span>
            </span>
            Correct Solutions
        </h3>

        <asp:Repeater runat="server" ID="SolutionRepeater">
            <HeaderTemplate>
                <table id="<%# SolutionRepeater.ClientID %>" class="table table-striped table-write-options"><tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Literal runat="server" ID="Key" Text='<%# Eval("Key") %>' Visible="false" />

                <tr data-id='<%# Eval("Key") %>'>
                    <td class="text-end" style="width:65px;">
                        <strong><%# Eval("Letter")  %></strong>
                    </td>
                    <td>
                        <asp:Repeater runat="server" ID="OptionRepeater">
                            <HeaderTemplate><div></HeaderTemplate>
                            <FooterTemplate></div></FooterTemplate>
                            <ItemTemplate>
                                <asp:Literal runat="server" ID="Key" Text='<%# Eval("Key") %>' Visible="false" />
                                <div class="bg-white border rounded py-2 px-3 mb-3 solution-option" data-id='<%# Eval("Key") %>'>
                                    <div class="mb-1 fw-bold">Option <%# (int)Eval("Index") + 1 %></div>
                                    <div style="white-space:pre-wrap;"><%# HttpUtility.HtmlEncode((string)Eval("Text.Default")) %></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:HiddenField runat="server" ID="OptionsOrder" ViewStateMode="Disabled" />
                    </td>
                    <td style="width:110px;">
                        <insite:NumericBox runat="server" ID="Points" MinValue="0" MaxValue="999.99" ValueAsDecimal='<%# Eval("Points") %>' />
                    </td>
                    <td style="width:110px;">
                        <insite:NumericBox runat="server" ID="CutScore" MinValue="0" MaxValue="100" ValueAsDecimal='<%# Eval("CutScore") %>' />
                    </td>
                    <td class="text-end" style="width:76px;">
                        <span class="start-sort ui-sortable-handle">
                            <i class="fas fa-sort"></i>
                        </span>
                    </td>
                    <td class="text-nowrap" style="width:30px;">
                        <span style="line-height: 28px;">
                            <insite:IconButton runat="server" CommandName="Delete" Name="trash-alt" ToolTip="Delete" Visible='<%# !(bool)Eval("IsReadOnly") %>' />
                        </span>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
        </asp:Repeater>

        <asp:HiddenField runat="server" ID="SolutionsOrder" ViewStateMode="Disabled" />

        <div class="mt-3">
            <insite:Button runat="server" ID="AddSolutionButton" Icon="fas fa-plus-circle" Text="Add New Solution" ButtonStyle="Default" />
        </div>
    </div>
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

<insite:PageHeadContent runat="server">
    <link rel="stylesheet" href="/ui/admin/assessments/options/controls/write/style.css">

    <style type="text/css">
        .solution-option {
            cursor: grab;
        }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script src="/ui/admin/assessments/options/controls/write/script.js"></script>

    <script type="text/javascript">
        (function () {
            (function () {
                const instance = window.orderingDetails = window.orderingDetails || {};

                let imgSelectorWindow = null;
                let imgSelectorText = null;

                instance.initReorder = function (id) {
                    const $container = $(document.getElementById(id)).prev('div');
                    if ($container.data('ui-sortable'))
                        return;

                    $container.data('input-id', id).sortable({
                        items: '> div',
                        cursor: 'grabbing',
                        opacity: 0.65,
                        tolerance: 'pointer',
                        axis: 'y',
                        forceHelperSize: true,
                        start: function (s, e) {
                            e.placeholder.height(e.item.height());
                        },
                        stop: function (e, a) {
                            const $container = a.item.parent('div');
                            const inputId = $container.data('input-id');
                            const $input = $(document.getElementById(inputId));

                            if ($input.length == 0)
                                return;

                            let result = '';

                            $container.find('> div').each(function () {
                                result += ';' + String($(this).data('id'));
                            });

                            $input.val(result.length > 0 ? result.substring(1) : '');
                        },
                    });
                };

                instance.onOptionTextSetup = function (mde, options) {
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

                    options.onSetText = onEditorSetText;
                    options.onGetText = onEditorGetText;
                    options.onPreview = onEditorGetText;
                };

                instance.initImgSelector = function () {
                    $('#<%= ImageSelectorWindow.ClientID %> .file-preview a').on('click', onImageSelectorClick);
                };

                function onEditorSetText(o) {
                    o.text = questionTextEditor.toInSiteMarkdown(o.text);
                }

                function onEditorGetText(o) {
                    o.text = questionTextEditor.fromInSiteMarkdown(o.text);
                }

                function onSelectImageOpen(editor) {
                    imgSelectorText = editor.codemirror.getTextArea();
                    imgSelectorWindow = modalManager.show('<%= ImageSelectorWindow.ClientID %>');
                    $(imgSelectorWindow).find('#<%= ImageSelectorRefresh.ClientID %>').click();
                }

                function onImageSelectorClick(e) {
                    e.preventDefault();
                    e.stopPropagation();

                    var $this = $(this);
                    var url = encodeURI($this.attr('href'));
                    var $thumbnail = $this.closest('.file-thumbnail');

                    var title = '';

                    var $title = $thumbnail.find('> span.file-title');
                    if ($title.length == 1)
                        title = $title.text().trim();

                    if (!title) {
                        var $name = $thumbnail.find('> span.file-name');
                        if ($name.length == 1)
                            title = $name.text().trim();
                    }

                    inSite.common.markdownEditor.onFileUploaded({
                        textId: imgSelectorText.id,
                        title: title,
                        url: url
                    });

                    modalManager.close(imgSelectorWindow);

                    imgSelectorText = null;
                    imgSelectorWindow = null;
                }
            })();
        })();
    </script>
</insite:PageFooterContent>
