(function () {
    $.ajaxSetup({ cache: false });

    $.fn.validator.Constructor.FOCUS_OFFSET = 60;
    $.fn.validator.Constructor.INPUT_SELECTOR = ':input:not([type="hidden"], [type="submit"], [type="reset"], button, .disable-validation)';

    const settings = {
        initElement: window.attempts.initElement,
        strings: {
            voiceRemoveConfirm: _answerStrings.composedVoice.removeConfirm
        },
    };
    const $form = $('form').validator({
        focus: false,
        disable: false,
        feedback: {
            success: 'fa fa-check',
            error: 'fa fa-times'
        },
    }).on('invalid.bs.validator', function (e) {
        const $input = $(e.relatedTarget);
        if ($input.length == 0)
            return;

        $input.closest('.card-question').addClass('has-error');
    }).on('valid.bs.validator', function (e) {
        const $input = $(e.relatedTarget);
        if ($input.length == 0
            || $input.hasClass('boolean-input') && $input.closest('tbody').find('> tr > td.input .form-group.has-error').length > 0
            || $input.hasClass('match-input') && $input.closest('tbody').find('> tr > td.right .form-group.has-error').length > 0)
            return;

        $input.closest('.card-question').removeClass('has-error');
    });

    $form.data('bs.validator').focusError = function () {
        if (this.options.focus)
            helper.focusError(this.$element);
    };

    let selectorIndex = Date.now();
    let composedInputTimeoutHandler = null;

    $(window).on('attempts:init', function () {
        $form.validator('update');

        $('.card-body > .form-group.radio-list input[type="radio"]').each(function () {
            settings.initElement(this, 'answer:input', function (el) {
                $(el).on('change', function () {
                    setRadioValue(this, true);
                });
            });
        });

        $('.card-body > .form-group.checkbox-list input[type="checkbox"]').each(function () {
            settings.initElement(this, 'answer:input', function (el) {
                $(el).on('change', function (e) {
                    setCheckBoxListValue(this, e);
                });
            });
        });

        $('.card-body > .form-group.boolean-list input[type="radio"]').each(function () {
            settings.initElement(this, 'answer:input', function (el) {
                $(el).on('change', function () {
                    setBooleanListValue(this, true);
                }).on('click', function (e) {
                    testBooleanListValueLimit(this, e);
                });
            });
        });

        $('.card-body > .form-group.match-list select.match-input').each(function () {
            settings.initElement(this, 'answer:input', function (el) {
                $(el).addClass('insite-combobox').each(function () {
                    if (!this.id)
                        this.id = 'selectpicker' + String(selectorIndex++);

                    inSite.common.comboBox.init({
                        id: this.id,
                        width: '100%'
                    });
                }).on('change', function () {
                    setMatchListValue(this, true);
                });
            });
        });

        $('.card-body > .form-group.composed-essay-input > textarea').each(function () {
            settings.initElement(this, 'answer:input', function (el) {
                $(el).on('input', function () {
                    if (composedInputTimeoutHandler != null)
                        clearTimeout(composedInputTimeoutHandler);
                    composedInputTimeoutHandler = setTimeout(function (el) { $(el).trigger('change'); }, 1000, this);
                }).on('change', function () {
                    if (composedInputTimeoutHandler != null) {
                        clearTimeout(composedInputTimeoutHandler);
                        composedInputTimeoutHandler = null;
                    }
                });
            });
        });

        $('.card-body > .form-group.composed-voice-input > .insite-input-audio').each(function () {
            settings.initElement(this, 'answer:input', function (recorder) {
                inSite.common.inputAudio.init(recorder);

                const $recorder = $(recorder);
                const $group = $recorder.closest('.form-group');
                const player = $group.find('> .insite-output-audio')[0];
                const input = $group.find('> input')[0];

                $recorder.data('player', player);
                $recorder.data('input', input);
                $group.data('recorder', recorder);

                $recorder.on(inSite.common.inputAudio.event.started, function () {
                    this.inputAudio.disableButton(inSite.common.inputAudio.button.startStop);
                    setComposedVoiceCommand(this, 'start', true);
                }).on(inSite.common.inputAudio.event.stopped, function () {
                    this.inputAudio.disable();
                    setComposedVoiceFile(this);
                });

                $group.on('attempts:input:submitted', function () {
                    const recorder = $(this).data('recorder');
                    const $recorder = $(recorder);

                    const input = $recorder.data('input');
                    const recorderObj = recorder.inputAudio;
                    const playerObj = $recorder.data('player').outputAudio;

                    let isFileSubmitting = false;

                    if (input.type == 'file' && recorderObj.mediaData != null && input.files.length == 1) {
                        const file = input.files[0];
                        isFileSubmitting = file.size > 0 && file.size == recorderObj.mediaData.size;
                    }

                    if (isFileSubmitting) {
                        playerObj.show();
                        recorderObj.hide();

                        playerObj.setData(recorderObj.mediaData);
                        playerObj.loadData();

                        recorderObj.clear();

                        setComposedVoiceCommand(recorder, 'has_data');
                    } else {
                        setComposedVoiceCommand(recorder, '');
                    }

                    recorderObj.enable();
                    recorderObj.enableButton(inSite.common.inputAudio.button.startStop);
                });
            });
        });

        $('.card-body > .form-group.composed-voice-input > .insite-output-audio').each(function () {
            settings.initElement(this, 'answer:input', function (player) {
                inSite.common.outputAudio.init(player);

                const $player = $(player);
                const $group = $player.closest('.form-group');
                const recorder = $group.find('> .insite-input-audio')[0];

                const playerObj = player.outputAudio;
                const recorderObj = recorder.inputAudio;

                $player.data('recorder', recorder);
                $group.data('player', player);

                $player.on(inSite.common.outputAudio.event.dataError, function () {
                    const player = this;
                    const recorder = $(player).data('recorder');
                    const playerObj = player.outputAudio;
                    const recorderObj = recorder.inputAudio;

                    playerObj.hide();
                    recorderObj.clear();
                    recorderObj.show();
                }).on(inSite.common.outputAudio.event.dataLoaded, function () {
                    const player = this;
                    const recorder = $(player).data('recorder');
                    const playerObj = player.outputAudio;
                    const recorderObj = recorder.inputAudio;

                    playerObj.attemptLimit = recorder.inputAudio.attemptLimit;
                    playerObj.attemptNow = recorder.inputAudio.attemptNow;

                    playerObj.show();
                    recorderObj.hide();
                }).on(inSite.common.outputAudio.event.delete, function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    const player = this;
                    const recorder = $(player).data('recorder');
                    const playerObj = player.outputAudio;
                    const recorderObj = recorder.inputAudio;

                    helper.showConfirm(settings.strings.voiceRemoveConfirm).done(function () {
                        playerObj.setData(null);
                        playerObj.hide();
                        recorderObj.clear();
                        recorderObj.show();
                        setComposedVoiceCommand(recorder, '');
                    });
                });

                if (playerObj.hasData) {
                    playerObj.show();
                    recorderObj.hide();
                    playerObj.loadData();

                    setComposedVoiceCommand(recorder, 'has_data');
                } else {
                    playerObj.hide();
                    recorderObj.clear();
                    recorderObj.show();
                }
            });
        });

        $('.card-body > .form-group.likert-matrix input[type="radio"]').each(function () {
            settings.initElement(this, 'answer:input', function (el) {
                $(el).on('change', function () {
                    setLikertRowValue(this, true);
                });
            });
        });

        $('.card-body > .form-group.ordering-list > .ordering-list-container').each(function () {
            settings.initElement(this, 'answer:input', function (el) {
                $(el).sortable({
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
                        var $container = a.item.closest('.ordering-list-container');
                        var $group = $container.closest('.ordering-list');
                        var $input = $group.find('> input');

                        var result = '';
                        $container.find('> div').each(function () {
                            result += ';' + String($(this).data('id'));
                        });

                        $input.val(result.length > 0 ? result.substring(1) : '').trigger('change');
                    },
                });
            });
        });

        $('.card-body > .form-group.radio-list > input[type="text"]').val('');
        $('.card-body > .form-group.checkbox-list > input[type="text"]').val('');
        $('.card-body > .form-group.boolean-list > input[type="text"]').val('');
        $('.card-body > .form-group.match-list > input[type="text"]').val('');
        $('.card-body > .form-group.likert-matrix > .table-likert > tbody > tr > td.text > input[type="text"]').val('');

        $('.card-body > .form-group.radio-list input[type="radio"]:checked').each(function () {
            setRadioValue(this, false);
        });

        $('.card-body > .form-group.checkbox-list').each(function () {
            const $checkbox = $(this).find('input[type="checkbox"]:first');
            if ($checkbox.length === 1)
                setCheckBoxListValue($checkbox[0]);
        });

        $('.card-body > .form-group.boolean-list').each(function () {
            const $el = $(this).find('input[type="radio"]:first');
            if ($el.length === 1)
                setBooleanListValue($el[0], false);
        });

        $('.card-body > .form-group.match-list select.match-input').each(function () {
            setMatchListValue(this, false);
        });

        $('.card-body > .form-group.likert-matrix input[type="radio"]:checked').each(function () {
            setLikertRowValue(this, false);
        });

        $('.card-body > .form-group > .table-option > tbody > tr > td.text').on('click', function () {
            $(this).closest('tr').find('input[type="radio"],input[type="checkbox"]').first().trigger('click');
        });
    });

    function setRadioValue(input, isChange) {
        const $input = $(input);
        const $value = $input.closest('.form-group').find('> input[type="text"]');

        $value.val($input.val());

        if (isChange === true)
            $value.trigger('change');
    }

    function setCheckBoxListValue(input, e) {
        const isChange = !!e;

        if (isChange && isCheckBoxListLimitExceeded(input)) {
            e.preventDefault();
            e.stopPropagation();

            input.checked = false;

            alert('It is not possible to make additional selection');

            return;
        }

        const $group = $(input).closest('.form-group');
        const $input = $group.find('> input[type="text"]');

        const values = [];
        $group.find('input[type="checkbox"]:checked').each(function () {
            values.push($(this).val());
        });

        $input.val(values.join(','));

        if (isChange === true)
            $input.trigger('change');
    }

    function isCheckBoxListLimitExceeded(input) {
        const $group = $(input).closest('.form-group');
        const limitAnswers = Number($group.data('limit'));

        if (isNaN(limitAnswers) || limitAnswers <= 0)
            return false;

        let count = 0;
        $group.find('input[type="checkbox"]:checked').each(function () {
            count++;
        });

        return count > limitAnswers;
    }

    function testBooleanListValueLimit(input, e) {
        if ($(input).val().endsWith('0')) {
            return;
        }

        const $group = $(input).closest('div.form-group.boolean-list');
        const limitAnswers = Number($group.data('limit'));

        if (limitAnswers == 0) {
            return;
        }

        let count = 0;
        $group.find('table.table-boolean > tbody > tr > td.input > div.form-group').each(function () {
            const $this = $(this);
            const $radio = $this.find('input[type="radio"]:checked');

            if ($radio.length === 1 && $radio.val().endsWith('1')) {
                count++;
            }
        });

        if (count > limitAnswers) {
            alert('It is not possible to make additional selection');
            e.preventDefault();
            e.stopPropagation();
        }
    }

    function setBooleanListValue(input, isChange) {
        const $group = $(input).closest('div.form-group.boolean-list');
        const $input = $group.find('> input[type="text"]');

        const values = [];
        $group.find('table.table-boolean > tbody > tr > td.input > div.form-group').each(function () {
            const $this = $(this);
            const $radio = $this.find('input[type="radio"]:checked');
            const $text = $this.find('input[type="text"]');

            if ($radio.length === 1) {
                values.push($radio.val());
                $text.val($radio.val());
            } else {
                $text.val('');
            }

            if (isChange === true)
                $text.trigger('change');
        });

        $input.val(values.join(','));
        if (isChange === true)
            $input.trigger('change');
    }

    function setMatchListValue(input, isChange) {
        const $group = $(input).closest('div.form-group.match-list');
        const $input = $group.find('> input[type="text"]');

        const values = [];
        $group.find('table.table-match > tbody > tr > td.right > div.form-group select').each(function () {
            const value = $(this).selectpicker('val');

            if (value)
                values.push(value);
        });

        $input.val(JSON.stringify(values));
        if (isChange === true)
            $input.trigger('change');
    }

    function setLikertRowValue(input, isChange) {
        const $input = $(input);
        const $value = $input.closest('tr').find('> td.text > input[type="text"]');

        $value.val($input.val());

        if (isChange === true)
            $value.trigger('change');
    }

    function setComposedVoiceFile(recorder) {
        const input = $(recorder).data('input');

        if (input.type !== 'file')
            input.type = 'file';

        const data = recorder.inputAudio.mediaData;
        if (data) {
            const file = new File([data], recorder.inputAudio.fileName);

            const transfer = new DataTransfer();
            transfer.items.add(file);

            input.files = transfer.files;

            $(input).trigger('change');
        } else {
            $(input).val('');
        }
    }

    function setComposedVoiceCommand(recorder, command, send) {
        const input = $(recorder).data('input');
        
        if (input.type !== 'text')
            input.type = 'text';

        if (send === true) {
            input.value = command;
            $(input).trigger('change');
        } else {
            setTimeout(function (v) { input.value = v; }, 0, command);
        }
    }
})();