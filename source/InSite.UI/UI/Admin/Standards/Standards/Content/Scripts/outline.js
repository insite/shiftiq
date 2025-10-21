(function () {
    var currentMode = null;
    var selectedNumber = null;

    var $tree = null;
    var $showSummaryComboBox = null;
    
    var $viewMode = null;

    Sys.Application.add_load(onAppLoad);

    // initialization

    function initReorder() {
        $('.editor-container ul[data-reorder]').each(function () {
            var $ul = $(this);
            if ($ul.data('reorderState'))
                return;

            var $stateInput = $($ul.data('reorder'));
            if ($stateInput.length !== 1) {
                $ul.data('reorderState', false);
                return;
            }

            $ul.data('reorderState', {
                $input: $stateInput
            });

            $ul.sortable({
                items: '> li',
                containment: $ul.closest('.editor-container'),
                cursor: 'grabbing',
                opacity: 0.65,
                tolerance: 'pointer',
                stop: function (e, a) {
                    var data = [];
                    var $ul = a.item.closest('ul');

                    $ul.find('> li').each(function () {
                        var number = parseInt($(this).data('number'));
                        if (!isNaN(number))
                            data.push(number);
                    });

                    $ul.data('reorderState').$input.val(JSON.stringify(data));
                },
            });
        });
    }
    
    // event handlers

    function onAppLoad() {
        $tree = $('.tree-view-container > .tree-view');
        const isTreeInited = inSite.common.treeView.init($tree, {
            expand: outline.settings.expandAllButtonSelector,
            collapse: outline.settings.collapseAllButtonSelector,
            level: outline.settings.expandLevelComboBoxSelector,
            state: outline.settings.stateKey,
            defaultLevel: 2
        });

        if (isTreeInited)
            $tree.find('[data-action]').on('click', onCommand);

        $showSummaryComboBox = $(outline.settings.showSummarySelector).off('change', onShowSummaryChanged);
        $viewMode = $(outline.settings.viewModeSelector);

        $viewMode.find("input").off('change', onModeChanged);

        if ($showSummaryComboBox.find('option').length == 0) {
            $showSummaryComboBox.append($('<option value="Hide" selected="selected">Hide Summary</option>'));
            $showSummaryComboBox.append($('<option value="Show">Show Summary</option>'));
            $showSummaryComboBox.selectpicker('refresh');
        }

        initReorder();

        var $editorContainer = $('.row-content .editor-container');
        outline.helpers.containerPosition.register($editorContainer);
        outline.helpers.containerPosition.update();

        $viewMode.find("input").on('change', onModeChanged);
        $showSummaryComboBox.on('change', onShowSummaryChanged);

        $viewMode.find('input[type="radio"]').each(function () {
            var $this = $(this);
            var $label = $(this).closest('label.btn');

            if ($this.prop('checked'))
                $label.addClass('active');
            else
                $label.removeClass('active');
        });

        onModeChanged(true);
    }

    function onModeChanged(isInit) {
        var prevMode = currentMode;

        var $radio = $viewMode.find('input[type="radio"]:checked');
        if ($radio.length !== 1)
            $radio = $viewMode.find('input[type="radio"]:first');

        currentMode = $radio.data('mode');

        var $panel = $tree.closest('.row-content').removeClass('view-tree view-edit');

        if (currentMode === 'edit' || currentMode === 'translate')
            $panel.addClass('view-edit');
        else
            $panel.addClass('view-tree');

        if (isInit !== true) {
            if (currentMode !== 'view' && prevMode !== 'view') {
                $tree.find('[name="node-select"]:checked').prop('checked', false);
                selectedNumber = null;

                document.getElementById(outline.settings.editorUpdatePanelId).ajaxRequest(JSON.stringify({
                    mode: 'unload'
                }));
            }
        }
    }

    function onShowSummaryChanged(e) {
        var val = $showSummaryComboBox.selectpicker('val');
        if (val == 'Show') {
            $tree.find('.node-summary').show();
        } else {
            $tree.find('.node-summary').hide();
        }
    }
    
    function onCommand(e) {
        var $this = $(this);
        var $item = $this.closest('li');

        var number = parseInt($item.data('key'));
        if (isNaN(number))
            return;

        var action = $this.data('action');

        if (action === 'show-info') {
            outline.info.load(number);
        } else if (action === 'select') {
            if (selectedNumber !== number) {
                document.getElementById(outline.settings.editorUpdatePanelId).ajaxRequest(JSON.stringify({
                    mode: currentMode,
                    number: number,
                    path: inSite.common.treeView.getItemPath($item)
                }));

                selectedNumber = number;
            }
        }
    }
})();


(function () {
    var instance;

    {
        var outline = window.outline = window.outline || {};
        instance = outline.helpers = outline.helpers || {};
    }

    instance.containerPosition = (function () {
        var instance = {};

        var windowDelayedScrollHandler = null;
        var $containers = [];

        $(window).on('scroll', function () {
            if (windowDelayedScrollHandler != null)
                clearTimeout(windowDelayedScrollHandler);

            windowDelayedScrollHandler = setTimeout(onWindowDelayedScroll, 350);
        });

        instance.register = function ($el) {
            if (!$el || !($el instanceof jQuery) || $el.length !== 1)
                return;

            var isFound = false;

            for (var i = 0; i < $containers.length; i++) {
                var $cel = $containers[i];

                if (!isFound)
                    isFound = $cel.is($el);

                if (!document.body.contains($cel[0]))
                    $containers.splice(i--, 1);
            }

            if (!isFound)
                $containers.push($el);
        };

        instance.update = updateContainerPosition;

        function onWindowDelayedScroll() {
            windowDelayedScrollHandler = null;

            var $container = getVisibleContainer();
            if ($container === null)
                return;

            var $window = $(window);
            var winScrollTop = $window.scrollTop();
            var winHeight = $window.height();

            var $document = $(document);
            var docHeight = $document.height();

            var panelTop = $container.offset().top;
            var panelHeight = $container.height();

            if (winScrollTop == 0 || docHeight - winScrollTop - winHeight <= 0 || panelTop + panelHeight - winScrollTop < 250 || winScrollTop + winHeight - panelTop < 350)
                updateContainerPosition();
        }

        function updateContainerPosition() {
            var $container = getVisibleContainer();
            if ($container === null)
                return;

            var offset = $(window).scrollTop() - 117;

            var maxOffset = $(document).height() - Math.ceil($container.height()) - 480;
            if (offset > maxOffset)
                offset = maxOffset;

            if (offset < 0)
                offset = 0;

            $container.animate({ marginTop: String(offset) + 'px' }, 250);
        }

        function getVisibleContainer() {
            for (var i = 0; i < $containers.length; i++) {
                var $el = $containers[i];
                if ($el.is(':visible'))
                    return $el;
            }

            return null;
        }

        return instance;
    })();
})();

(function () {
    var info = window.outline.info = window.outline.info || {};

    var allowCloseInfoWindow = true;

    $(window).on('message', onWindowMessage);

    // public methods

    info.load = function (number) {
        if ($.active > 0)
            return;

        allowCloseInfoWindow = true;

        var wnd = modalManager.load(outline.settings.infoWindowId, '/ui/admin/standards/info?asset=' + String(number));

        modalManager.setTitle(wnd, 'Loading...');

        $(wnd)
            .on('hide.bs.modal', function (e, s, a) {
                if (!allowCloseInfoWindow) {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    return false;
                }
            })
            .one('closing.modal.insite', function (e, s, a) {
                if (a === null)
                    return;

                if (a.action === 'redirect')
                    window.location = a.url;
                else if (a.action === 'refresh') {
                    outline.settings.reloadTree();
                }
            });
    };

    // event handlers

    function onWindowMessage(e) {
        var eventData = String(e.originalEvent.data);
        if (!eventData.startsWith('insite.assetSummary:'))
            return;

        var command = eventData.substring(19);

        if (command === 'disable-close')
            allowCloseInfoWindow = false;
        else if (command === 'enable-close')
            allowCloseInfoWindow = true;
    }
})();
