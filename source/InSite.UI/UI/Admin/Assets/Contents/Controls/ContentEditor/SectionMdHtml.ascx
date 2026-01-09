<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SectionMdHtml.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.ContentEditor.SectionMdHtml" %>

<%@ Register Src="Field.ascx" TagName="ContentEditorField" TagPrefix="uc" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="content-section-mdhtml">
            <div runat="server" id="BodyToggleWrapper" class="mdh-toggle-wrapper">
                <insite:Toggle runat="server" ID="IsBodyMarkdown"
                    Size="Mini" Width="70px" Height="21px"
                    TextOn="<i class='fab fa-markdown me-1'></i> MD" TextOff="<i class='far fa-code me-1'></i> HTML" 
                    StyleOn="Primary" StyleOff="Primary" />
            </div>
            <uc:ContentEditorField runat="server" ID="EditorFieldText" />
            <uc:ContentEditorField runat="server" ID="EditorFieldHtml" />
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageHeadContent runat="server" ID="CommonScript" RenderRequired="true">
    <script type="text/javascript">
        (function () {
            var instance = window.sectionMdHtml = window.sectionMdHtml || {};

            instance.init = function (options) {
                var data = {
                    $fieldToggle: $(String(options.toggleSelector)),
                };
                if (data.$fieldToggle.length !== 1)
                    return;

                var $section = data.$fieldToggle.closest('.content-section-mdhtml');
                if ($section.data('mdhtml'))
                    return;

                data.$fieldText = $(String(options.fieldTextSelector));
                data.$fieldHtml = $(String(options.fieldHtmlSelector));

                if (data.$fieldText.length !== 1 || data.$fieldHtml.length !== 1)
                    return;

                $section.find('.content-field').on('langChanged.translation', onFieldUpdated);

                data.$fieldToggle.on('change', onFieldTypeChanged);

                data.$fieldText.find('> div.commands').each(function () {
                    $(this).css('left', '65px');
                });

                $section.data('mdhtml', data);

                onFieldTypeChanged.call(data.$fieldToggle.get(0));

                setTimeout(() => {
                    data.$fieldText.find('.CodeMirror').each(function () {
                        this.CodeMirror.refresh();
                    });
                    data.$fieldHtml.find('.CodeMirror').each(function () {
                        this.CodeMirror.refresh();
                    });
                }, 0);
            };

            function onFieldTypeChanged() {
                var $section = $(this).closest('.content-section-mdhtml');
                var data = $section.data('mdhtml');
                var $activeField = null;

                if (data.$fieldToggle.prop('checked')) {
                    $activeField = data.$fieldText.removeClass('d-none');
                    data.$fieldHtml.addClass('d-none');
                } else {
                    data.$fieldText.addClass('d-none');
                    $activeField = data.$fieldHtml.removeClass('d-none');
                }

                data.$fieldToggle.trigger('shown.mdhtml.toggle');

                if ($activeField !== null) {
                    var lang = $activeField.find('> div.commands > .lang-out').data('code');
                    if (lang)
                        $section.trigger('updated.content-section', [lang]);
                }
            }

            function onFieldUpdated(e, lang) {
                e.preventDefault();
                e.stopPropagation();

                var $eventField = $(this);
                var $section = $eventField.closest('.content-section-mdhtml');
                var data = $section.data('mdhtml');

                var $activeField = null;
                if (!data.$fieldText.hasClass('d-none')) {
                    $activeField = data.$fieldText;
                } else if (!data.$fieldHtml.hasClass('d-none')) {
                    $activeField = data.$fieldHtml;
                }

                if ($activeField !== null && $activeField.is($eventField))
                    $section.trigger('updated.content-section', [lang]);
            }
        })();
    </script>
</insite:PageHeadContent>
