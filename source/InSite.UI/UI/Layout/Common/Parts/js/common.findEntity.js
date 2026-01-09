(function () {
    const instance = inSite.common.findEntity = inSite.common.findEntity || {};

    class PageData {
        #num;
        #groups;
        #items;

        constructor(num, items) {
            if (typeof num != 'number' || isNaN(num) || num <= 0)
                throw new Error('Invalid page number');

            if (!(items instanceof Array))
                items = [];

            this.#num = num;

            let isGrouped = false;
            for (let i = 0; i < items.length; i++) {
                const item = items[i];
                if (item.group && item.items) {
                    isGrouped = true;
                    break;
                }
            }

            if (isGrouped) {
                this.#groups = [];
                this.#items = [];

                for (let i = 0; i < items.length; i++) {
                    const gData = items[i];
                    const group = PageData.#createGroup(gData);

                    for (let j = 0; j < gData.items.length; j++) {
                        const item = PageData.#createItem(gData.items[j]);
                        item.index = this.#items.length;
                        group.items.push(item);
                        this.#items.push(item);
                    }

                    this.#groups.push(group);
                }
            } else {
                this.#groups = null;
                this.#items = [];

                for (let i = 0; i < items.length; i++) {
                    const item = PageData.#createItem(items[i]);
                    item.index = this.#items.length;
                    this.#items.push(item);
                }
            }
        }

        get num() {
            return this.#num;
        }

        get isGrouped() {
            return this.#groups != null;
        }

        getGroups() {
            return this.#groups;
        }

        getItems() {
            return this.#items;
        }

        static #createGroup(data) {
            if (typeof data.group != 'string' || data.group.length == 0 || !(data.items instanceof Array) || data.items.length == 0)
                throw new Error('Invalid group data: ' + JSON.stringify(data));

            return {
                name: data.group,
                items: []
            };
        }

        static #createItem(data) {
            if (typeof data.value != 'string' || data.value.length == 0)
                throw new Error('Invalid item data: ' + JSON.stringify(data));

            const result = {
                value: data.value,
                text: ''
            };

            if (typeof data.text == 'string' && data.value.length > 0)
                result.text = data.text;

            return result;
        }
    }

    const pageClusterSize = 10;

    let modalIdIndex = Date.now();
    let theForm = null;

    $(document).ready(function () {
        theForm = document.getElementsByTagName('FORM');
        if (theForm.length > 0)
            theForm = theForm[0];
        else
            alert('Form not found');
    });

    instance.init = function (settings) {
        if (!settings || typeof settings.id != 'string' || typeof settings.name != 'string')
            return;

        const input = document.getElementById(settings.id);
        if (!input || getData(input))
            return;

        const data = {
            name: settings.name,
            input: 'radio',
            multiple: false,
            autoSave: settings.autoSave != false,
            placeholder: '',
            pageSize: 10,

            getUrl: null,

            callback: {},
            strings: {
                modalHeader: 'Find and Select',
                tableHeader: 'Name',
                entityPlural: 'Items',
                clearTitle: 'Clear',
                cancelTitle: 'Cancel',
                keywordText: 'Keyword',
                clearFilterText: 'Clear Filter',
                applyFilterText: 'Apply Filter',
                noItems: 'No Items'
            },
        };

        data.$input = $(input);
        data.$container = data.$input.closest('.insite-findentity');

        data.$input.parents('label').each(function () {
            const $this = $(this);
            if (!$this.attr('for'))
                $this.attr('for', '');
        });

        data.items = data.$input.val();
        if (data.items)
            data.items = JSON.parse(data.items);
        else
            data.items = [];

        if (settings.url)
            data.getUrl = Function.apply(null, [
                'item',
                'return "' + settings.url.replace(/\{([a-zA-Z-_]+)\}/g, '" + item.$1 + "') + '";'
            ]);

        if (typeof settings.maxSelect == 'number' && (settings.maxSelect == 0 || settings.maxSelect > 1)) {
            data.input = 'checkbox';
            data.multiple = true;
            data.autoSave = false;
            data.maxSelect = settings.maxSelect;
        }

        if (settings.pageSize && settings.pageSize >= 5)
            data.pageSize = settings.pageSize;

        if (settings.maxPageCount && settings.maxPageCount > 0)
            data.maxPageCount = settings.maxPageCount;

        if (settings.placeholder)
            data.placeholder = settings.placeholder;

        if (settings.strings) {
            if (settings.strings.modalHeader)
                data.strings.modalHeader = settings.strings.modalHeader;

            if (settings.strings.tableHeader)
                data.strings.tableHeader = settings.strings.tableHeader;

            if (settings.strings.entityPlural)
                data.strings.entityPlural = settings.strings.entityPlural;

            if (settings.strings.clearTitle)
                data.strings.clearTitle = settings.strings.clearTitle;

            if (settings.strings.cancelTitle)
                data.strings.cancelTitle = settings.strings.cancelTitle;

            if (settings.strings.keywordText)
                data.strings.keywordText = settings.strings.keywordText;

            if (settings.strings.clearFilterText)
                data.strings.clearFilterText = settings.strings.clearFilterText;

            if (settings.strings.applyFilterText)
                data.strings.applyFilterText = settings.strings.applyFilterText;

            if (settings.strings.noItems)
                data.strings.noItems = settings.strings.noItems;
        }

        if (settings.callback) {
            if (settings.callback.onChange)
                data.callback.onChange = settings.callback.onChange;

            if (settings.callback.postBack)
                data.callback.postBack = new Function(settings.callback.postBack);
        }

        input.show = function () {
            modal.open(getData(this));
        };

        input.disable = function (disable) {
            output.disable(getData(this), disable !== false);
        };

        input.enable = function (enable) {
            output.disable(getData(this), !(enable !== false));
        };

        input.hasValue = function () {
            const data = getData(this);
            return data.items.length > 0;
        };

        input.getItem = function () {
            const data = getData(this);

            if (!data.multiple && data.items.length > 0)
                return $.extend({}, data.items[0]);

            return null;
        };

        input.getSelectedCount = function () {
            const data = getData(this);
            return data.items.length;
        };

        input.getItems = function () {
            const data = getData(this);

            if (data.items.length == 0)
                return null;

            const result = [];

            for (let i = 0; i < data.items.length; i++)
                result.push($.extend({}, data.items[i]));

            return result;
        };

        input.clearSelection = function () {
            const data = getData(this);
            data.items = [];
            output.update(data);
        };

        input.refresh = function () {
            const data = getData(this);
            if (data.modal.$modal.hasClass('show'))
                modal.open(data);
        };

        output.create(settings, data);
        modal.create(settings, data);

        data.$container.data('findEntity', data);

        output.update(data);
    };

    const output = (function () {
        const helper = {};

        helper.create = function (settings, data) {
            if (settings.output == 'list') {
                data.list = {};

                data.list.$checkAll = $('<button type="button" class="btn btn-sm btn-icon btn-default" data-action="check-all"><i class="far"></i></button>');
                data.list.$clear = $('<button type="button" class="btn btn-sm btn-icon btn-default" data-action="clear"><i class="far fa-trash-alt"></i></button>');
                data.list.$buttons = $('<div class="fe-buttons">').append(
                    data.list.$checkAll,
                    $('<button type="button" class="btn btn-sm btn-icon btn-default" data-action="add"><i class="far fa-plus-circle"></i></button>'),
                    data.list.$clear
                );
                data.list.$table = $('<table class="table table-hover table-sm">').append(
                    $('<tbody>')
                );

                if (data.placeholder)
                    data.list.$message = $('<div class="fe-message" style="display:none;">').text(data.placeholder);

                data.$container.append(
                    data.list.$buttons,
                    data.list.$table,
                    data.list.$message
                );

                data.list.$buttons.find('[data-action]').on('click', onListButtonClick);
            } else if (!settings.output || settings.output == 'button') {
                data.$output = $('<div>');
                data.$button = $('<button type="button" tabindex="-1" class="btn dropdown-toggle">').append(
                    $('<div>').append(data.$output),
                ).on('click', onButtonClick);

                data.$container.append(
                    data.$button
                );
            }

            if (settings.disabled)
                helper.disable(data);
        };

        helper.disable = function (data, disable) {
            if (data.list) {
                if (disable == false) {
                    data.list.$buttons.show();
                    data.list.$table.removeClass('disabled');
                } else {
                    data.list.$buttons.hide();
                    data.list.$table.addClass('disabled');
                }
            } else if (data.$button) {
                if (disable == false) {
                    data.$button.removeClass('disabled');
                } else {
                    data.$button.addClass('disabled');
                }
            }
        };

        helper.update = function (data) {
            {
                const newItems = data.items;
                const oldItems = JSON.parse(data.$input.val() || '[]');

                let isChanged = newItems.length != oldItems.length;

                if (!isChanged && newItems.length > 0) {
                    for (let x = 0; x < newItems.length; x++) {
                        const nValue = newItems[x].value;
                        let isFound = false;

                        for (let y = 0; y < oldItems.length; y++) {
                            if (oldItems[y].value == nValue) {
                                isFound = true;
                                break;
                            }
                        }

                        if (!isFound) {
                            isChanged = true;
                            break;
                        }
                    }
                }

                if (isChanged) {
                    if (data.items.length == 0)
                        data.$input.val('');
                    else
                        data.$input.val(JSON.stringify(data.items));

                    data.$input.trigger('change');

                    let allowPostBack = true;

                    if (data.callback.onChange) {
                        const fn = inSite.common.getObjByName(data.callback.onChange);
                        if (typeof fn == 'function' && fn.call(data.$input[0]) == false)
                            allowPostBack = false;
                    }

                    if (allowPostBack && data.callback.postBack)
                        data.callback.postBack();
                }
            }

            if (data.list) {
                if (data.items.length == 0) {
                    data.$container.addClass('fe-empty');

                    data.list.$checkAll.hide();
                    data.list.$clear.hide();
                    data.list.$table.hide();

                    if (data.list.$message)
                        data.list.$message.show();
                } else {
                    data.$container.removeClass('fe-empty');

                    data.list.$checkAll.show();
                    data.list.$clear.hide();
                    data.list.$table.show();

                    if (data.list.$message)
                        data.list.$message.hide();

                    const $tbody = data.list.$table.find('> tbody').empty();

                    for (let i = 0; i < data.items.length; i++) {
                        const item = data.items[i];
                        const $input = $('<input>')
                            .attr('type', 'checkbox')
                            .addClass('form-check-input')
                            .val(i)
                            .on('click', onTableInputClick);

                        const $textCell = $('<td>');

                        if (data.getUrl) {
                            const url = data.getUrl(item);
                            $textCell.append(
                                $('<a target="_blank">').attr('href', url).text(item.text)
                            );
                        } else {
                            $textCell.text(item.text);
                        }

                        $tbody.append(
                            $('<tr>').append(
                                $('<td class="fe-input">').append($input),
                                $textCell
                            )
                        );
                    }

                    data.list.$checkAll.prop('checked', false)
                    onCheckAllChanged(data);
                }
            } else if (data.$button) {
                if (data.items.length == 0) {
                    data.$container.addClass('fe-empty');
                    data.$button.removeAttr('title');
                    data.$output.text(data.placeholder);
                    return;
                }

                data.$container.removeClass('fe-empty');

                if (data.items.length == 1) {
                    const item = data.items[0];
                    data.$button.removeAttr('title');
                    data.$output.text(item.text);
                    return;
                }

                let title = ''; {
                    let length = data.items.length;

                    const isMaxExceeded = length > 10;
                    if (isMaxExceeded)
                        length = 10;

                    for (let i = 0; i < length; i++) {
                        if (title)
                            title += '\r\n';

                        title += '• ' + data.items[i].text;
                    }

                    if (isMaxExceeded)
                        title += '\r\n...';
                }

                data.$button.attr('title', title);
                data.$output.text(String(data.items.length) + ' ' + data.strings.entityPlural);
            }
        };

        return helper;

        // event handlers

        function onButtonClick() {
            if ($(this).hasClass('disabled'))
                return;

            modal.open(getData(this));
        }

        function onListButtonClick(e) {
            const action = e.currentTarget.dataset.action;

            if (action === 'check-all') {
                e.stopPropagation();

                if (typeof this.checked == 'undefined')
                    this.checked = false;

                this.checked = !this.checked;

                const data = getData(this);
                onCheckAllChanged(data);
                data.list.$table.find('> tbody input').prop('checked', this.checked);

                if (this.checked)
                    data.list.$clear.show();
                else
                    data.list.$clear.hide();
            } else if (action === 'add') {
                e.stopPropagation();

                modal.open(getData(this));
            } else if (action === 'clear') {
                e.stopPropagation();

                const data = getData(this);
                const $selected = data.list.$table.find('> tbody input[type="checkbox"]:checked');
                if ($selected.length == 0)
                    return;

                if (confirm('Are you sure you want delete the selected ' + data.strings.entityPlural.toLowerCase() + '?')) {
                    const indexes = {};

                    $selected.each(function () {
                        indexes[this.value] = true;
                    });

                    const oldItems = data.items;
                    const newItems = [];

                    for (let i = 0; i < oldItems.length; i++) {
                        if (!indexes.hasOwnProperty(i))
                            newItems.push(oldItems[i]);
                    }

                    data.items = newItems;

                    helper.update(data);
                }
            }
        }

        function onCheckAllChanged(data) {
            const $i = data.list.$checkAll.find('> i');
            if (data.list.$checkAll.prop('checked') === true)
                $i.removeClass('fa-square').addClass('fa-check-square');
            else
                $i.removeClass('fa-check-square').addClass('fa-square');
        }

        function onTableInputClick(e) {
            const data = getData(this);

            let hasChecked = false;
            let hasUnchecked = false;

            data.list.$table.find('> tbody input[type="checkbox"]').each(function () {
                if (this.checked)
                    hasChecked = true;
                else
                    hasUnchecked = true;

                return !(hasChecked && hasUnchecked);
            });

            data.list.$checkAll.prop('checked', hasChecked && !hasUnchecked);
            onCheckAllChanged(data);

            if (hasChecked)
                data.list.$clear.show();
            else
                data.list.$clear.hide();
        }

        // methods

    })();

    const modal = (function () {
        const helper = {};

        const modalState = {
            loading: 1,
            message: 2,
            data: 3
        };

        helper.create = function (settings, data) {
            const modalId = 'findentity_' + String(modalIdIndex++);
            const modalHeaderId = modalId + '_header';

            data.modal = {};

            data.modal.$filter = $('<div class="findentity-filter mb-3">'); {
                createDefaultCriteria('keyword', data.strings.keywordText);

                data.modal.$filter.find('[data-action]').on('click', onFilterClick);
            }

            data.modal.$checkAll = null;
            if (data.multiple && data.maxSelect == 0)
                data.modal.$checkAll = $('<input class="form-check-input" type="checkbox">').on('click', onCheckAllClick);

            data.modal.$table = $('<table class="table table-hover table-sm">').append(
                $('<thead>').append(
                    $('<tr>').append(
                        $('<th class="fe-group-indent">'),
                        $('<th class="fe-input">').append(data.modal.$checkAll),
                        $('<th>').text(data.strings.tableHeader)
                    )
                ),
                $('<tbody>')
            );

            data.modal.$pagination = $('<ul class="pagination pagination-sm">');
            data.modal.$message = $('<div class="fe-message">');
            data.modal.$loading = $('<div class="loading-panel"><div><div><i class="fa fa-spinner fa-pulse fa-3x"></i></div></div></div>');

            let $modalHeader = null;
            if (settings.showHeader !== false) {
                $modalHeader = $('<div class="modal-header">').append(
                    $('<h5 class="modal-title">').attr('id', modalHeaderId).text(data.strings.modalHeader),
                    $('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>')
                );
            }

            let $modalFooter = null;
            if (settings.showFooter !== false) {
                $modalFooter = $('<div class="modal-footer">');

                if (!data.autoSave)
                    $modalFooter.append(
                        $('<button type="button" class="btn btn-sm btn-primary" data-action="save"><i class="fas fa-check me-1"></i> OK</button>')
                    );

                if (settings.showClear !== false)
                    $modalFooter.append(
                        $(`<button type="button" class="btn btn-sm btn-default" data-action="clear"><i class="far fa-undo me-1"></i> ${data.strings.clearTitle}</button>`)
                    );

                $modalFooter.append(
                    $(`<button type="button" class="btn btn-sm btn-default" data-action="close"><i class="fas fa-ban me-1"></i> ${data.strings.cancelTitle}</button>`)
                );

                $modalFooter.find('[data-action]').on('click', onFooterClick);
            }

            data.modal.$modal = $('<div class="modal modal-findentity insite-modal fade" tabindex="-1" role="dialog" data-content="static">')
                .attr('id', modalId)
                .attr('aria-labelledby', modalHeaderId)
                .append(
                    $('<div class="modal-dialog modal-fullscreen-md-down" role="document">').append(
                        $('<div class="modal-content">').append(
                            $modalHeader,
                            $('<div class="modal-body">').append(
                                data.modal.$filter,
                                data.modal.$table,
                                data.modal.$pagination,
                                data.modal.$message
                            ),
                            $modalFooter,
                            data.modal.$loading
                        )
                    )
                )
                .on('hidden.bs.modal', onModalHidden);

            data.$container.append(data.modal.$modal);

            data.$input[0].dispatchEvent(new CustomEvent('windows-created.findentity', {
                bubbles: true
            }));

            function createDefaultCriteria(name, placeholder) {
                const $inputGroup = $('<div class="input-group">').append(
                    $('<input type="text" class="insite-text form-control">')
                        .attr('data-name', name)
                        .attr('placeholder', placeholder)
                        .on('keydown', onFilterKeyDown)
                        .on('keyup', onFilterKeyUp),
                    $(`<button class="btn btn-icon btn-outline-primary" data-action="clear" type="button" title="${data.strings.clearFilterText}"><i class="fas fa-times"></i></button>`),
                    $(`<button class="btn btn-icon btn-outline-primary" data-action="filter" type="button" title="${data.strings.applyFilterText}"><i class="fas fa-filter"></i></button>`)
                );

                data.modal.$filter.append(
                    $('<div>').append(
                        $inputGroup
                    )
                );
            }
        };

        helper.open = function (data) {
            initItem(data);

            data.modal.$modal.modal('show');

            loadData(data);
        };

        return helper;

        // event handlers

        function onFilterClick(e) {
            const action = e.currentTarget.dataset.action;

            if (action === 'clear') {
                e.stopPropagation();

                setFilter(getData(this));
            } else if (action === 'filter') {
                e.stopPropagation();

                loadData(getData(this));
            }
        }

        function onFilterKeyDown(e) {
            if (e.which === 13) {
                e.preventDefault();
                e.stopPropagation();
                loadData(getData(this));
            }
        }

        function onFilterKeyUp(e) {
            if (e.which === 13) {
                e.preventDefault();
                e.stopPropagation();
            }
        }

        function onCheckAllClick(e) {
            const data = getData(this);
            const page = getPage(data, data.modal.pages.num);

            if (!page) {
                this.checked = !this.checked;
                return;
            }

            const checked = this.checked;

            data.modal.$table.find('> tbody input').prop('checked', checked);

            const items = page.getItems();
            for (let i = 0; i < items.length; i++)
                setItem(data, items[i], checked);
        }

        function onTableRowClick(e) {
            $(this).find('input:first').click();
        }

        function onTableInputClick(e) {
            if (e)
                e.stopPropagation();

            if (this.disabled)
                return;

            const index = parseInt(this.value);
            if (isNaN(index))
                return;

            const data = getData(this);
            const page = getPage(data, data.modal.pages.num);
            if (!page)
                return;

            const items = page.getItems();
            if (!setItem(data, items[index], this.checked))
                this.checked = !this.checked;

            if (this.checked && data.autoSave)
                saveModal(data);

            onTableInputChanged(data);
        }

        function onTableInputChanged(data) {
            if (data.modal.$checkAll) {
                const isAllChecked = data.modal.$table.find('> tbody input:not(:checked)').length == 0;
                data.modal.$checkAll.prop('checked', isAllChecked);
            }

            if (data.multiple && data.maxSelect > 1) {
                data.modal.$table.find('> tbody input:not(:checked)').prop('disabled', data.modal.itemCount >= data.maxSelect);
            }
        }

        function onPaginationClick(e) {
            e.preventDefault();

            const page = parseInt(e.target.dataset.page);

            if (!isNaN(page)) {
                e.stopPropagation();
                loadData(getData(this), page);
            }
        }

        function onFooterClick(e) {
            const action = e.currentTarget.dataset.action;

            if (action == 'close') {
                e.stopPropagation();

                const data = getData(this);
                data.modal.$modal.modal('hide');
            } else if (action == 'clear') {
                e.stopPropagation();

                const data = getData(this);
                data.modal.$table.find('> tbody input').prop('checked', false);
                clearItem(data);
                onTableInputChanged(data);

                if (data.autoSave)
                    saveModal(data);

            } else if (action == 'save') {
                e.stopPropagation();

                const data = getData(this);

                saveModal(data);
            }
        }

        function onModalHidden(e) {
            const data = getData(this);

            setFilter(data);

            data.modal.$table.find('> tbody').empty();
            data.modal.$pagination.empty();
            data.modal.$message.empty();

            delete data.modal.pages;
            delete data.modal.item;
            delete data.modal.items;
            delete data.modal.itemCount;
        }

        // filter methods

        function setFilter(data, values) {
            if (!values || typeof values != 'object')
                values = {};

            data.modal.$filter.find('[data-name]').each(function () {
                const name = this.dataset.name;

                let value = '';
                if (values.hasOwnProperty(name))
                    value = values[name];

                if (this.tagName == 'INPUT' && this.type == 'text') {
                    this.value = value;
                } else {
                    alert('Unexpected input: ' + this.tagName);
                }
            });
        }

        function getFilter(data) {
            const values = {};

            data.modal.$filter.find('[data-name]').each(function () {
                const name = this.dataset.name;

                if (this.tagName == 'INPUT' && this.type == 'text') {
                    value = this.value;
                } else {
                    alert('Unexpected input: ' + this.tagName);
                }

                if (value)
                    values[name] = value;
            });

            return values;
        }

        // modal methods

        function loadData(data, pageNum) {
            if (typeof pageNum == 'undefined' || typeof data.modal.filter == 'undefined')
                data.modal.filter = getFilter(data);

            const postData = {
                filter: data.modal.filter
            };

            if (pageNum && pageNum >= 1) {
                if (pageNum == data.modal.pages.num)
                    return;

                if (pageNum > data.modal.pages.count)
                    pageNum = data.modal.pages.count;

                if (hasPage(data, pageNum)) {
                    renderPage(data, pageNum);
                    return;
                }

                postData.page = pageNum;
            } else {
                pageNum = 1;
                data.modal.pages = {
                    num: -1,
                    count: 0
                };
            }

            const formData = new FormData(theForm);
            formData.append('__FINDENTITYID', data.name);
            formData.append('__FINDENTITYPR', JSON.stringify(postData));

            setState(data, modalState.loading);

            $.ajax({
                type: 'POST',
                data: formData,
                dataType: 'json',

                cache: false,
                contentType: false,
                processData: false,

                success: function (response) {
                    if (!postData.page) {
                        if (!response.count || response.count <= 0) {
                            setState(data, modalState.message);
                            data.modal.$message.text(data.strings.noItems);
                            return;
                        }

                        data.modal.pages.count = Math.ceil(response.count / data.pageSize);
                    }

                    const page = new PageData(pageNum, response.items);

                    setPage(data, page);
                    renderPage(data, pageNum);
                },
                error: function () {
                    setState(data, modalState.message);
                    data.modal.$message.text('An error occurred during operation');
                }
            });
        }

        function renderPage(data, pageNum) {
            const pageCount = data.modal.pages.count;
            if (pageCount <= 0 || !hasPage(data, pageNum)) {
                setState(data, modalState.message);
                data.modal.$message.text('Page Not Found');
                return;
            }

            const page = getPage(data, pageNum);
            const isGrouped = page.isGrouped;
            const $tbody = data.modal.$table.find('> tbody').empty();
            const inputName = data.input == 'radio' ? 'fe-' + String(Date.now()) : null;

            if (isGrouped) {
                data.modal.$table.addClass('table-fe-grouped');

                const groups = page.getGroups();
                for (let i = 0; i < groups.length; i++) {
                    const group = groups[i];

                    $tbody.append(
                        $('<tr class="group-header">').append(
                            $('<td colspan="3">').text(group.name),
                        )
                    );

                    for (let j = 0; j < group.items.length; j++)
                        renderItem(group.items[j]);
                }
            } else {
                data.modal.$table.removeClass('table-fe-grouped');

                const items = page.getItems();
                for (let i = 0; i < items.length; i++)
                    renderItem(items[i]);
            }

            onTableInputChanged(data);
            setState(data, modalState.data);

            if (pageCount > 1) {
                const newPageCluster = Math.floor((pageNum - 1) / pageClusterSize);
                const oldPageCluster = Math.floor((data.modal.pages.num - 1) / pageClusterSize);

                if (newPageCluster != oldPageCluster) {
                    data.modal.$pagination.empty();

                    const hasMaxPage = !!data.maxPageCount;
                    const startPage = newPageCluster * pageClusterSize + 1;

                    let endPage = startPage + pageClusterSize - 1;
                    if (endPage > pageCount)
                        endPage = pageCount;

                    if (newPageCluster >= 1)
                        addPage(startPage - 1, '#prev', '...');

                    for (let i = startPage; i <= endPage; i++)
                        addPage(i, '#page' + String(i), String(i));

                    if (endPage < pageCount)
                        addPage(endPage + 1, '#next', '...');

                    data.modal.$pagination.find('[data-page]').on('click', onPaginationClick);

                    function addPage(num, href, text) {
                        const $a = $('<a class="page-link">').attr('href', href).text(text);
                        const $li = $('<li class="page-item">').append($a);

                        if (hasMaxPage && num > data.maxPageCount) {
                            $li.addClass('disabled');
                            $a.attr('onclick', 'return false;')
                        } else {
                            $a.attr('data-page', String(num))
                        }

                        data.modal.$pagination.append($li);
                    }
                }

                data.modal.$pagination
                    .find('li.active').removeClass('active').end()
                    .find('a[data-page="' + String(pageNum) + '"]').closest('li').addClass('active');
                data.modal.$pagination.show();
            }

            data.modal.pages.num = pageNum;

            const $input = data.modal.$filter.find('input[data-name][type="text"]:first');
            if ($input.length == 1)
                setTimeout(function (input) { input.focus() }, 500, $input[0]);

            function renderItem(item) {
                const $input = $('<input>')
                    .attr('type', data.input)
                    .attr('name', inputName)
                    .addClass('form-check-input')
                    .val(item.index)
                    .prop('checked', getItem(data, item.value) != null)
                    .on('click', onTableInputClick);

                $tbody.append(
                    $('<tr>').append(
                        isGrouped ? $('<td>') : null,
                        $('<td>').append($input),
                        $('<td>').text(item.text)
                    ).on('click', onTableRowClick)
                );
            }
        }

        function saveModal(data) {
            saveItem(data);

            output.update(data);

            data.modal.$modal.modal('hide');
        }

        // helpers

        function initItem(data) {
            clearItem(data);

            if (data.multiple) {
                for (let i = 0; i < data.items.length; i++) {
                    const item = data.items[i];

                    const key = item.value.toUpperCase();
                    if (!data.modal.items.hasOwnProperty(key)) {
                        data.modal.itemCount += 1;
                        data.modal.items[key] = item;
                    }
                }
            } else {
                if (data.items.length > 0)
                    data.modal.item = data.items[0];
            }
        }

        function saveItem(data) {
            data.items = [];

            if (data.multiple) {
                const items = data.modal.items;

                for (let value in items) {
                    if (items.hasOwnProperty(value))
                        data.items.push(items[value]);
                }

                data.items.sort(function (a, b) {
                    if (a.text < b.text)
                        return -1;

                    if (a.text > b.text)
                        return 1;

                    return 0;
                });
            } else {
                if (data.modal.item)
                    data.items.push(data.modal.item);
            }
        }

        function clearItem(data) {
            if (data.multiple) {
                data.modal.itemCount = 0;
                data.modal.items = {};
            } else {
                data.modal.item = null;
            }
        }

        function setItem(data, item, add) {
            const exists = !!getItem(data, item.value);

            if (add === true) {
                if (!exists) {
                    if (data.multiple) {
                        if (data.maxSelect > 1 && data.modal.itemCount >= data.maxSelect)
                            return false;

                        data.modal.itemCount += 1;
                        data.modal.items[item.value.toUpperCase()] = item;
                    } else {
                        data.modal.item = item;
                    }

                    return true;
                }
            } else if (add === false) {
                if (exists) {
                    if (data.multiple) {
                        data.modal.itemCount -= 1;
                        delete data.modal.items[item.value.toUpperCase()];
                    } else {
                        data.modal.item = null;
                    }

                    return true;
                }
            }

            return false;
        }

        function getItem(data, value) {
            if (data.multiple) {
                const key = value.toUpperCase();
                if (data.modal.items.hasOwnProperty(key))
                    return data.modal.items[key];
            } else if (data.modal.item != null && data.modal.item.value == value) {
                return data.modal.item;
            }

            return null;
        }

        function setState(data, state) {
            if (modalState.loading == state) {
                data.modal.$loading.show();
                return;
            }

            data.modal.$table.hide();
            data.modal.$pagination.hide();
            data.modal.$message.hide();
            data.modal.$loading.hide();

            if (modalState.message == state) {
                data.modal.$message.show();
            } else if (modalState.data == state) {
                data.modal.$table.show();
            }
        }

        function hasPage(data, pageNum) {
            return data.modal.pages.hasOwnProperty('page_' + String(pageNum));
        }

        function getPage(data, pageNum) {
            if (hasPage(data, pageNum))
                return data.modal.pages['page_' + String(pageNum)];
            else
                return null;
        }

        function setPage(data, page) {
            data.modal.pages['page_' + String(page.num)] = page;
        }
    })();

    function getData(el) {
        return $(el).closest('.insite-findentity').data('findEntity');
    }
})();