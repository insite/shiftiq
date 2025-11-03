(function () {
    var instance = null;
    {
        var inSite = window.inSite = window.inSite || {};
        var common = inSite.common = inSite.common || {};

        if (common.dateTimeOffsetSelector)
            return;

        instance = common.dateTimeOffsetSelector = {};
    }

    // static fields

    const dtosId = 'dtos' + String(Date.now());
    const defaultEvents = {
        blur: null,
        click: null,
        focus: null,
        keydown: null,
        keypress: null,
        keyup: null
    };
    const dirClass = Object.freeze({
        up: 'dropup',
        down: null,
        left: null,
        right: null
    });
    const flatpickrCache = [];

    let isInitializationStarted = false;
    let timeZones = [{ title: 'UTC', abbrv: 'UTC', moment: 'UTC' }];
    let $timeZoneMenu = null;
    let destroyGarbageHandler = null;

    $(document).on('click', onDocumentClick);

    // public methods

    instance.tz = function (array) {
        if (isInitializationStarted)
            return;

        timeZones = array;

        for (let i = 0; i < timeZones.length; i++)
            timeZones[i].index = i;
    };

    instance.init = function (options) {
        let input = document.getElementsByName(options.name);
        if (!input || input.length != 1)
            return;

        isInitializationStarted = true;

        input = input[0];

        let inputData;

        if (!input.hasOwnProperty(dtosId)) {
            input[dtosId] = inputData = {
                render: false,
                init: false
            };
        } else {
            inputData = input[dtosId];
        }

        options.readOnly = options.readOnly === true;
        options.disabled = options.disabled === true;
        options.enableDate = options.enableDate === true;
        options.enableTime = options.enableTime === true;
        options.enableTz = options.enableTz === true;

        if (inputData.render !== true) {
            inputData.render = true;
            render(input, options);

            if (destroyGarbageHandler !== null)
                clearTimeout(destroyGarbageHandler);
                
            destroyGarbageHandler = setTimeout(destroyGarbage, 100);
        }

        setup(input, options);
    };

    function render(input, options) {
        const $input = $(input);
        const $group = $('<div class="input-group datetimeoffset">').insertAfter($input);

        $input.detach();

        $group.append(function () {
            const elements = [
                $('<input type="text" class="form-control">').attr('id', options.id),
                $input,
            ];

            if (options.enableTz)
                elements.push(
                    $('<button class="btn btn-outline-secondary btn-icon btn-timezone" type="button" title="Select Time Zone">').append(
                        $('<span>'),
                        $('<i class="far fa-globe-americas">')
                    )
                );

            if (options.enableDate)
                elements.push(
                    $('<button class="btn btn-outline-secondary btn-icon btn-datepicker" type="button" title="Select Date/Time">').append(
                        $('<i class="far fa-calendar-alt">')
                    )
                );

            return elements;
        });
    }

    function setup(input, options) {
        if (typeof $.fn.flatpickr !== 'function')
            throw new Error('flatpickr is not loaded');

        const inputData = input[dtosId];
        if (inputData.render !== true)
            return;

        const $input = $(input);
        const $group = $input.closest('.input-group'); {
            $group.prop('class', 'input-group datetimeoffset');
            if (options.class)
                $group.addClass(options.class);

            if (options.style)
                $group.prop('style', options.style);
            else
                $group.removeAttr('style');
        }

        const $inputText = $group.find('> input[type="text"]'); {
            if (options.placeholder)
                $inputText.attr('placeholder', options.placeholder);
            else
                $inputText.removeAttr('placeholder');

            const inputText = $inputText[0];
            const events = $.extend({}, defaultEvents, options.events || {});
            for (let e in events) {
                if (e === 'change')
                    continue;

                const func = events[e];
                if (!func)
                    continue;

                const eName = 'on' + e;
                if (eName in inputText)
                    inputText[eName] = new Function(func);
            }

            inputText.getDate = getDateByElement;
            inputText.getTimeZone = getTimeZoneByElement;
            inputText.clearDate = clearDateByElement;
        }

        let data;

        if (inputData.init !== true) {
            inputData.init = true;

            data = {
                $output: $inputText,
                $input: $input,
                $openbutton: $group.find('> .btn-datepicker').on('click', onOpenClick),
                preset: (options && options.preset) || null
            };

            const pickerOptions = {
                clickOpens: false,
                allowInput: true,
                enableTime: options.enableTime,
                position: 'auto right',
                disableMobile: 'true',

                formatDate: formatDate,
                parseDate: parseDate,

                onClose: onClose,
                onChange: onDateChange
            };

            if (data.preset === 'since') {
                pickerOptions.defaultHour = 0;
                pickerOptions.defaultMinute = 0;
            } else if (data.preset === 'before') {
                pickerOptions.defaultHour = 23;
                pickerOptions.defaultMinute = 59;
            }

            if (options.data.fullFormat)
                pickerOptions.dateFormat = options.data.fullFormat;

            if (options.data.shortFormat)
                pickerOptions.altFormat = pickerOptions.ariaDateFormat = options.data.shortFormat;

            if (options.data.date) {
                const dateParsed = moment(options.data.date);
                if (dateParsed.isValid())
                    pickerOptions.defaultDate = dateParsed.toDate();
            }

            if (options.enableTz) {
                let timeZone = getTimeZone(options.data.timeZone);
                if (!timeZone)
                    timeZone = getTimeZone('UTC');

                pickerOptions.timeZone = timeZone.moment;
                data.$timezonebutton = $group.find('> .btn-timezone').on('click', onTimeZoneClick);

                if (timeZone)
                    data.$timezonelabel = data.$timezonebutton.find('> span').text(timeZone.abbrv);

                data.$timezonebutton[0][dtosId] = data;
            }

            if (options.events && options.events.change)
                data.onChange = new Function(options.events.change);

            flatpickrCache.push(data.datePicker = $inputText.flatpickr(pickerOptions));

            const inputText = $inputText[0];
            inputText[dtosId] = data;
            data.datePicker.config.lastValue = inputText.getDate();
            updateInput(data);
        } else {
            data = $inputText[0][dtosId];
        }

        data.$output.prop('readonly', options.readOnly);
        data.$output.prop('disabled', options.disabled);
        data.$openbutton.prop('disabled', options.readOnly || options.disabled);

        if (data.$timezonebutton)
            data.$timezonebutton.prop('disabled', options.readOnly || options.disabled);
    }

    function getTimeZone(value) {
        if (typeof value === 'number' && !isNaN(value) && value >= 0 && value < timeZones.length) {
            return timeZones[value];
        } else if (typeof value == 'string' && value.length > 0) {
            value = value.toUpperCase();

            for (let i = 0; i < timeZones.length; i++) {
                const tzData = timeZones[i];
                if (tzData.abbrv.toUpperCase() == value || tzData.moment.toUpperCase() == value)
                    return tzData;
            }
        }

        return null;
    }

    function createTimeZoneMenu(selected) {
        const $list = $('<ul class="dropdown-menu" style="width:auto;">');

        for (let i = 0; i < timeZones.length; i++) {
            const tzData = timeZones[i];
            const $listItem = $('<li>').data('index', tzData.index).append(
                $('<a href="javascript:void(0);">').text(tzData.title)
            );

            if (tzData.moment === selected)
                $listItem.addClass('active');

            $list.append($listItem);
        }

        return $list;
    }

    function setTimeZoneMenuPosition($menu, dirClass) {
        const $opener = $menu.data('opener');
        if (!$opener)
            return;

        const $window = $(window);
        const $offsetParent = $menu.offsetParent();
        const offset = $offsetParent.offset();

        let vertical = 'bottom';
        if (offset.top + $menu.height() * 1.5 >= $window.height() + $window.scrollTop() && $menu.height() + $offsetParent.outerHeight() < offset.top)
            vertical = 'top';

        let horizontal = 'left';

        const position = { top: 'auto', right: 'auto', bottom: 'auto', left: 'auto' };

        if (vertical === 'top') {
            position.bottom = $offsetParent.outerHeight();

            if (dirClass.up)
                $menu.addClass(dirClass.up);

            if (dirClass.down)
                $menu.removeClass(dirClass.down);
        } else {
            position.top = $offsetParent.outerHeight();

            if (dirClass.up)
                $menu.removeClass(dirClass.up);

            if (dirClass.down)
                $menu.addClass(dirClass.down);
        }

        if (horizontal === 'left') {
            position.right = $offsetParent.outerWidth() - $opener.position().left - 14 - $opener.outerWidth() / 2;

            if (dirClass.right)
                $menu.addClass(dirClass.right);

            if (dirClass.left)
                $menu.removeClass(dirClass.left);
        } else {
            position.left = $opener.position().left + 4;

            if (dirClass.right)
                $menu.removeClass(dirClass.right);

            if (dirClass.left)
                $menu.addClass(dirClass.left);
        }

        $menu.css(position);
    }

    // event handlers

    function onDocumentClick(e) {
        if (!$timeZoneMenu || !$timeZoneMenu.data('opener'))
            return;

        if (e.target && ($timeZoneMenu.is(e.target) || $.contains($timeZoneMenu.get(0), e.target)))
            return;

        hideTimeZoneMenu();
    }

    function onOpenClick(e) {
        e.preventDefault();

        const $this = $(this);
        if ($this.prop('disabled'))
            return;

        const $input = $this.closest('.datetimeoffset').find('> input[type="text"]');
        if ($input.length == 1)
            $input.get(0)[dtosId].datePicker.open();
    }

    function onTimeZoneClick(e) {
        e.preventDefault();
        e.stopPropagation();

        const $this = $(this);
        if ($this.prop('disabled'))
            return;

        showTimeZoneMenu($this);
    }

    function onTimeZoneSelected(e) {
        e.preventDefault();
        e.stopPropagation();

        const $listItem = $(this).closest('li');

        const tzData = getTimeZone($listItem.data('index'));
        if (tzData) {
            const data = $listItem.closest('div.datetimeoffset').find('> input[type="text"]').get(0)[dtosId];
            if (data.datePicker.config.timeZone != tzData.moment) {
                data.datePicker.config.timeZone = tzData.moment;
                data.$timezonelabel.text(tzData.abbrv);

                const date = getDate(data.datePicker);
                if (date) {
                    data.datePicker.setDate(date.local(true).toDate());
                    onChange(data);
                } else {
                    updateInput(data)
                }
            }
        }

        hideTimeZoneMenu();
    }

    function onClose() {
        onChange(this.element[dtosId]);
    }

    function onDateChange() {
        if (this.isOpen)
            return;

        onChange(this.element[dtosId]);
    }

    function onChange(data) {
        const newValue = getDate(data.datePicker);
        const isLastNull = !data.datePicker.config.lastValue;
        const isNewNull = !newValue;

        if (isLastNull != isNewNull || !isNewNull && !data.datePicker.config.lastValue.isSame(newValue)) {
            updateInput(data);

            if (data.onChange)
                data.onChange();
        }

        data.datePicker.config.lastValue = newValue;
    }

    // methods

    function getDateByElement(el) {
        if (!el)
            el = this;

        if (!el.hasOwnProperty(dtosId))
            return;

        return getDate(el[dtosId].datePicker);
    }

    function clearDateByElement(el) {
        if (!el)
            el = this;

        if (!el.hasOwnProperty(dtosId))
            return;

        el[dtosId].datePicker.clear();
    }

    function getDate(datePicker) {
        if (datePicker.selectedDates.length == 1) {
            const date = moment(datePicker.selectedDates[0]);

            if (datePicker.config.timeZone)
                date.tz(datePicker.config.timeZone, true);
            else
                date.local(true);

            return date;
        }

        return null;
    }

    function getTimeZoneByElement(el) {
        if (!el)
            el = this;

        if (!el.hasOwnProperty(dtosId))
            return;

        return el[dtosId].datePicker.config.timeZone;
    }

    function showTimeZoneMenu(opener) {
        if (!$timeZoneMenu)
            $timeZoneMenu = createTimeZoneMenu()
                .find('a').addClass('dropdown-item').on('click', onTimeZoneSelected).end();

        const $opener = $(opener);
        const $container = $opener.closest('.datetimeoffset');

        if ($timeZoneMenu.data('opener')) {
            hideTimeZoneMenu();

            if ($.contains($container.get(0), $timeZoneMenu.get(0)))
                return;
        }

        const data = $container.find('> input[type="text"]').get(0)[dtosId];
        const tzIndex = getTimeZone(data.datePicker.config.timeZone).index;

        $timeZoneMenu.find('li').each(function () {
            const $listItem = $(this);
            if ($listItem.data('index') == tzIndex)
                $listItem.addClass('active');
            else
                $listItem.removeClass('active');
        });

        $timeZoneMenu.detach().data('opener', $opener).insertAfter($opener).addClass('show');

        moveTimeZoneMenu();
    }

    function hideTimeZoneMenu() {
        $timeZoneMenu.removeClass('show').data('opener', null);
    }

    function updateInput(data) {
        const inputData = {
            value: null,
        };

        const date = getDate(data.datePicker);

        if (date) {
            if (data.preset === 'before') {
                date.seconds(59).milliseconds(0);
            } else if (data.preset === 'since') {
                date.seconds(0).milliseconds(0);
            }
            inputData.value = date.format();
        }

        if (data.datePicker.config.timeZone) {
            const tz = getTimeZone(data.datePicker.config.timeZone);
            if (tz)
                inputData.timeZone = tz.abbrv;
        }

        data.$input.val(JSON.stringify(inputData));
    }

    function formatDate(date, format) {
        const m = moment(date);

        this.timeZone
            ? m.tz(this.timeZone, true)
            : m.local(true);

        return m.format(format);
    }

    function parseDate(datestr, format) {
        let date;

        if (this.timeZone) {
            const localTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

            date = moment.tz(datestr, format, true, localTimeZone);

            if (!date.isValid()) {
                date = moment.tz(datestr, format, localTimeZone);

                if (!date.isValid())
                    date = moment.tz(datestr, localTimeZone);
            }

        } else {
            date = moment(datestr, format, true);

            if (!date.isValid())
                date = moment(datestr);

            if (date.isValid()) {
                date.local(true);
            }
        }

        if (!date.isValid() && this.lastValue)
            date = this.lastValue;

        if (date.isValid())
            return date.toDate();
    }

    function moveTimeZoneMenu() {
        setTimeZoneMenuPosition($timeZoneMenu, dirClass);
    }

    function destroyGarbage() {
        if (destroyGarbageHandler !== null)
            clearTimeout(destroyGarbageHandler);

        destroyGarbageHandler = null;

        for (let i = 0; i < flatpickrCache.length; i++) {
            let item = flatpickrCache[i];
            let isGarbage = false;

            try {
                if (!document.body.contains(item.input)) {
                    item.destroy();
                    isGarbage = true;
                }
            } catch (e) {
                console.error(e);
                isGarbage = true;
            }

            if (isGarbage)
                flatpickrCache.splice(i--, 1);
        }
    }
})();