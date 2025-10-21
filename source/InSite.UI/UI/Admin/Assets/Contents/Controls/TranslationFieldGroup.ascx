<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TranslationFieldGroup.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.TranslationFieldGroup" %>

<%@ Register Src="TranslationWindow.ascx" TagName="TranslationWindow" TagPrefix="uc" %>

<uc:TranslationWindow runat="server" ID="TranslationWindow" />

<asp:Button runat="server" ID="UpdateButton" style="display:none;" />

<div class="form-group mb-3" id="<%= ClientID %>">
    <label class="form-label">
        <%= LabelText %>
        <span runat="server" id="LanguageOutput" class="label" style="background-color:#aaa; margin:0 5px;"></span>
        <insite:CustomValidator runat="server" ID="ClientStateRequiredValidator" Enabled="false" Display="Dynamic" />
        <insite:IconButton runat="server" ID="EditButton" CssClass="btn-edit" Name="pencil" ToolTip="Edit" style="margin:0 5px;" />
    </label>
    <div runat="server" id="InputWrapper">
        <insite:TextBox runat="server" ID="TranslationText" Width="100%" />
        <asp:HiddenField runat="server" ID="StateInput" />
    </div>
    <div class="form-text" style="margin-top: 0px;"><%= HelpText %></div>
</div>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        body.drag-active .translation-dropmessage {
            display: block !important;
        }

        .translation-dropmessage,
        .translation-loadingmessage {
            z-index: 501;
        }

        .translation-dropmessage > div > div {
            font-size: 1.1em;
        }

        .translation-dropmessage.drag-over > div > div {
            font-weight: bold;
        }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">

        (function () {
            // public methods

            var instance = window.translationField = window.translationField || {};

            function ControlState($langOutput, $textInput, $stateInput, type) {
                var obj = this;
                var ensureInited = null;
                var setValue = null;
                var getValue = null;

                $textInput.on('change', onTextChanged);

                setValue = function (value) { $textInput.val(value); };
                getValue = function () { return $textInput.val(); };

                if (ensureInited !== null) {
                    ensureInited();

                    $textInput.parents('.tab-pane').each(function () {
                        $('[data-bs-target="#' + this.id + '"][data-bs-toggle]').on('shown.bs.tab', function () {
                            ensureInited();
                        });
                    });
                }

                obj.update = function (json) {
                    $stateInput.val(json);

                    obj.refresh();
                };

                obj.refresh = function () {
                    var text = instance.bind($textInput.data('simple-mde'), $stateInput, $langOutput, function (lang) {
                        translationField.setLanguage($stateInput, lang);

                        obj.refresh();
                    });

                    setValue(text);
                }

                function onTextChanged() {
                    translationField.setText($stateInput, getValue());
                }
            }

            instance.getState = function ($input) {
                var state = $input.val();

                if (state) {
                    try {
                        return JSON.parse(state);
                    } catch (e) {
                        return null;
                    }
                }

                return null;
            };

            instance.bind = function (simplemde, $text, $lang, onSelect) {
                var state = instance.getState($text);
                if (!state || !state.data)
                    return;

                var lang = state.lang;
                if (!lang)
                    lang = 'en';

                lang = lang.toUpperCase();

                $lang.text(lang);

                var result = '';
                var hasData = false;

                var $table = $('<table style="margin-top:5px;">');

                var data = state.data;
                for (var key in data) {
                    if (!data.hasOwnProperty(key))
                        continue;

                    hasData = true;

                    var name = key.toUpperCase();
                    if (name !== lang) {
                        var html = simplemde != null ? simplemde.options.previewRender(data[key]) : data[key];

                        $table.append(
                            $('<tr>').append(
                                $('<td style="font-weight:bold; width:40px; padding:5px; vertical-align:top;">').append(
                                    $('<a href="#">').data('key', key).on('click', function (e) {
                                        e.preventDefault();

                                        var key = $(this).data('key');
                                        if (!key)
                                            return;

                                        onSelect(key.toUpperCase());
                                    }).text(name)
                                ),
                                $('<td style="padding:5px; word-break:break-word;">').html(html)
                            )
                        );
                    } else {
                        result = data[key];
                    }
                }

                $text.next('table').remove();

                if (hasData)
                    $text.after($table);

                return result;
            };

            instance.setText = function ($input, value) {
                var state = instance.getState($input);
                if (!state)
                    return;

                if (!state.data)
                    state.data = {};

                if (value)
                    value = value.trim();

                state.data[state.lang.toLowerCase()] = value;

                var json = JSON.stringify(state);

                $input.val(json);
            };

            instance.setLanguage = function ($input, value) {
                var state = instance.getState($input);
                if (!state)
                    return;

                if (value)
                    value = value.trim();

                state.lang = value;

                var json = JSON.stringify(state);

                $input.val(json);
            };

            instance.init = function (options) {
                var stateInputSelector = '';
                if (typeof options.stateId === 'string' && options.stateId.length > 0)
                    stateInputSelector = '#' + options.stateId;

                var $stateInput = $(stateInputSelector);
                if ($stateInput.length !== 1)
                    return;

                var isRefresh = false;

                var ctrlState = $stateInput.data('state');
                if (!ctrlState) {
                    var langInputSelector = '';
                    if (typeof options.langId === 'string' && options.langId.length > 0)
                        langInputSelector = '#' + options.langId;

                    var textInputSelector = '';
                    if (typeof options.textId === 'string' && options.textId.length > 0)
                        textInputSelector = '#' + options.textId;

                    var $lang = $(langInputSelector);
                    var $text = $(textInputSelector);

                    ctrlState = new ControlState($lang, $text, $stateInput, options.type);
                    $stateInput.data('state', ctrlState);
                    isRefresh = true;
                }

                if (options.state) {
                    ctrlState.update(options.state);
                    isRefresh = false;
                }

                if (isRefresh)
                    ctrlState.refresh();
            };
        })();

    </script>
</insite:PageFooterContent>
