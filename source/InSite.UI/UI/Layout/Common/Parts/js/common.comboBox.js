(function () {
    const instance = inSite.common.comboBox = inSite.common.comboBox || {};

    const _defaultWindowPadding = [100, 0, 20, 0];
    const _invisibleElements = [];

    $(function () { setTimeout(refreshInvisibleElements, 0); });
    $(window).one('load', function () { setTimeout(refreshInvisibleElements, 0); });

    // public methods

    instance.init = function (settings) {
        if (!settings)
            return;

        const idObj = settings.id;
        const idType = typeof idObj;

        let $elem = null;

        if (idType === 'string') {
            if (idObj.length > 0) {
                if (idObj[0] === '#') {
                    $elem = $(idObj);
                } else if (typeof idObj === 'string' && idObj.length > 0) {
                    $elem = $(document.getElementById(idObj));
                }
            }
        } else if (idType === 'object') {
            if (idObj instanceof HTMLElement)
                $elem = $(idObj);
            else if (idObj instanceof jQuery)
                $elem = idObj;
        }

        if ($elem?.length !== 1 || !($elem[0] instanceof HTMLSelectElement) || $elem.data('selectpicker'))
            return;

        if (!$elem.is(':visible')) {
            if (_invisibleElements.length == 0)
                $(document).on('shown.bs.collapse shown.bs.tab', refreshInvisibleElements);

            _invisibleElements.push($elem);
        }

        const options = {
            iconBase: '',
            tickIcon: 'fas fa-check',
            windowPadding: _defaultWindowPadding
        };
        const data = options.insite = {
            callback: {}
        };

        if (settings.noSelect)
            options.noneSelectedText = settings.noSelect;
        else
            options.noneSelectedText = '';

        if (settings.noSearch)
            options.noneResultsText = settings.noSearch;

        if (settings.search)
            options.liveSearch = true;

        options.showContent = settings.showContent === true;

        if (settings.style)
            options.style = settings.style;

        if (settings.width)
            options.width = settings.width;

        if (settings.callback) {
            if (settings.callback.onChange)
                data.callback.onChange = settings.callback.onChange;

            if (settings.callback.postBack)
                data.callback.postBack = new Function(settings.callback.postBack);
        }

        if (settings.menu) {
            const menu = settings.menu;

            if (menu.header)
                options.header = menu.header;

            if (menu.size)
                options.size = menu.size;

            if (menu.container)
                options.container = menu.container;

            if (menu.dropDir) {
                options.dropupAuto = false;

                if (menu.dropDir == 'up')
                    $elem.addClass('dropup');
            }

            if (menu.width)
                data.menuWidth = menu.width;
        }

        if (settings.multi) {
            const multi = settings.multi;

            if (multi.max)
                options.maxOptions = multi.max;

            if (multi.text)
                options.selectedTextFormat = multi.text;

            if (multi.actions)
                options.actionsBox = true;

            if (multi.selectAll)
                options.selectAllText = multi.selectAll;

            if (multi.deselectAll)
                options.deselectAllText = multi.deselectAll;

            if (multi.countText || multi.countManyText || multi.countAllText) {
                options.countSelectedText = buildCountText;

                data.count = {
                    one: '{0} item selected',
                    many: '{0} items selected',
                    all: null
                };

                if (multi.countText)
                    data.count.one = multi.countText;

                if (multi.countManyText)
                    data.count.many = multi.countManyText;

                if (multi.countAllText)
                    data.count.all = multi.countAllText;
            }

            if (multi.maxAllText || multi.maxAllManyText || multi.maxGroupText || multi.maxGroupManyText) {
                options.maxOptionsText = buildMultiMaxText;

                data.max = {
                    all: {
                        one: 'Limit reached ({n} item max)',
                        many: 'Limit reached ({n} items max)',
                    },
                    group: {
                        one: 'Group limit reached ({n} item max)',
                        many: 'Group limit reached ({n} items max)',
                    }
                };

                if (multi.maxAllText)
                    data.max.all.one = multi.maxAllText;

                if (multi.maxAllManyText)
                    data.max.all.many = multi.maxAllManyText;

                if (multi.maxGroupText)
                    data.max.group.one = multi.maxGroupText;

                if (multi.maxGroupManyText)
                    data.max.group.many = multi.maxGroupManyText;
            }
        }

        initSelectpicker($elem, options);
    };

    function initSelectpicker($elem, options) {
        {
            const existData = $elem.data('selectpicker');
            if (existData) {
                if (existData.insite)
                    return;

                $elem.selectpicker('destroy');
            }
        }

        $elem.removeClass('form-select');

        if (!options.style)
            options.style = 'btn-combobox';

        if (options.insite.menuWidth)
            $elem.on('show.bs.select', onMenuShow);

        if (options.insite.callback) {
            if ($elem.prop('multiple')) {
                $elem.on('show.bs.select', onMultiShow);
                $elem.on('hide.bs.select', onMultiHide);
            } else {
                $elem.on('changed.bs.select', onComboChange);
            }
        }

        $elem.selectpicker(options);

        setTimeout(function () {
            $elem.selectpicker('refresh');
        });

        $elem.data('selectpicker').$menu
            .find('> .bs-actionsbox .btn-group .btn-light')
            .addClass('btn-outline-secondary')
            .removeClass('btn-light');
    }

    function refreshInvisibleElements() {
        for (let i = 0; i < _invisibleElements.length; i++) {
            const $el = _invisibleElements[i];

            if (!document.body.contains($el[0])) {
                _invisibleElements.splice(i--, 1);
            } else if ($el.is(':visible')) {
                const data = $el.data('selectpicker');

                if (data)
                    data.setSize(true);

                _invisibleElements.splice(i--, 1);
            }
        }

        if (_invisibleElements.length == 0)
            $(document).off('shown.bs.collapse shown.bs.tab', refreshInvisibleElements);
    }

    function buildCountText(numSelected, numTotal) {
        if (this.insite && this.insite.count) {
            const count = this.insite.count;

            if (numSelected >= numTotal && count.all)
                return count.all;

            if (numSelected > 1)
                return count.many;
            else
                return count.one;
        }

        return '';
    }

    function buildMultiMaxText(numAll, numGroup) {
        const allText = ''
        const groupText = '';

        if (this.insite && this.insite.max) {
            const max = this.insite.max;

            if (numAll > 1)
                return max.all.many;
            else
                return max.all.one;

            if (numGroup > 1)
                return max.group.many;
            else
                return max.group.one;
        }

        return [allText, groupText];
    }

    function onMenuShow() {
        const data = $(this).data('selectpicker');
        if (data.options.insite.menuWidth)
            data.$menu.css('width', data.options.insite.menuWidth);
    }

    function onComboChange() {
        const data = $(this).data('selectpicker').options.insite;
        return onComboChanged(this, data);
    }

    function onMultiShow() {
        const data = $(this).data('selectpicker').options.insite;
        data.state = getSelectedIndices(this);
    }

    function onMultiHide() {
        const data = $(this).data('selectpicker').options.insite;
        if (!data.state)
            return;

        const state = getSelectedIndices(this);

        let isChanged = state.length != data.state.length;

        if (!isChanged) {
            for (let i = 0; i < state.length; i++) {
                if (state[i] != data.state[i]) {
                    isChanged = true;
                    break;
                }
            }
        }

        data.state = null;

        if (isChanged)
            return onComboChanged(this, data);
    }

    function onComboChanged(el, data) {
        if (data.callback.onChange) {
            const fn = inSite.common.getObjByName(data.callback.onChange);
            if (typeof fn == 'function' && fn.call(el) == false)
                return false;
        }

        if (data.callback.postBack)
            return data.callback.postBack();
    }

    function getSelectedIndices(select) {
        const result = [];
        const options = select.options;

        for (let i = 0; i < options.length; i++) {
            const o = options[i];
            if (o.selected)
                result.push(i);
        }

        return result;
    }
})();