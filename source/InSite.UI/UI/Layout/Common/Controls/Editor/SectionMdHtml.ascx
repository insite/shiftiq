<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SectionMdHtml.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.Editor.SectionMdHtml" %>

<%@ Register Src="Field.ascx" TagName="ContentEditorField" TagPrefix="uc" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="content-section-mdhtml">
            <div runat="server" id="BodyToggleWrapper" style="position:absolute; right:10px; margin-top:-4px; z-index:1;">
                <insite:Toggle runat="server" ID="IsBodyMarkdown" Size="Mini" Width="74px" Height="21px" StyleOn="Primary" StyleOff="Primary"
                    TextOn="<i class='fab fa-markdown'></i>" TextOff="<i class='far fa-code'></i>" />
            </div>
            <uc:ContentEditorField runat="server" ID="EditorFieldText" />
            <uc:ContentEditorField runat="server" ID="EditorFieldHtml" />
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageHeadContent runat="server" ID="CommonScript">
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

                $section.find('.content-field').on('updated.content-field', onFieldUpdated);

                data.$fieldToggle.on('change', onFieldTypeChanged);

                data.$fieldText.find('> div.commands').each(function () {
                    $(this).css('right', '51px');
                });

                $section.data('mdhtml', data);

                onFieldTypeChanged.call(data.$fieldToggle.get(0));
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

                if ($activeField !== null) {
                    var lang = $activeField.find('> div.commands > .lang-out').text();
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
