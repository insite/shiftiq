(function () {
    var instance = window.modalManager = window.modalManager || {};
    var bodySelector = '> .modal-dialog > .modal-content > .modal-body';

    $(window).on('message', onWindowMessage);

    instance.load = function (id, url) {
        var $modal = $getModal(id);
        if ($modal.length === 0 || $modal.data('content') === 'static')
            return null;

        $modal
            .off('show.bs.modal', onModalOpening)
            .on('show.bs.modal', onModalOpening)
            .off('shown.bs.modal', onModalOpened)
            .on('shown.bs.modal', onModalOpened)
            .off('hide.bs.modal', onModalClosing)
            .on('hide.bs.modal', onModalClosing)
            .off('hidden.bs.modal', onModalClosed)
            .on('hidden.bs.modal', onModalClosed)
            .addClass('loading')
            .data('content', 'frame');

        var $modalBody = $modal.find(bodySelector);

        $modalBody.append($(
            '<div class="loading-panel">' +
            '<div>' +
            '<div>' +
            '<i class="fa fa-spinner fa-pulse fa-3x"></i>' +
            '</div>' +
            '</div>' +
            '</div>'
        ));

        setTimeout(function ($modal) {
            $modal.modal('show');
        }, 0, $modal);

        var $frame = $('<iframe frameborder="0" class="w-100 border-0">').on('load', onFrameLoaded);

        $frame.attr('src', url);

        $modalBody.append($frame);

        return $modal[0];
    };

    instance.show = function (id) {
        var $modal = $getModal(id);
        if ($modal.length === 0 || $modal.data('content') !== 'static')
            return null;

        $modal
            .off('show.bs.modal', onModalOpening)
            .on('show.bs.modal', onModalOpening)
            .off('shown.bs.modal', onModalOpened)
            .on('shown.bs.modal', onModalOpened)
            .off('hide.bs.modal', onModalClosing)
            .on('hide.bs.modal', onModalClosing)
            .off('hidden.bs.modal', onModalClosed)
            .on('hidden.bs.modal', onModalClosed);

        setTimeout(function ($modal) { $modal.modal('show'); }, 0, $modal);

        return $modal[0];
    };

    instance.closeModal = function (args) {
        if (isInFrame())
            window.parent.postMessage('insite.modal:' + JSON.stringify({ cmd: 'close', args: args }), '*');
    };

    instance.close = function (id, args) {
        $getModal(id).data('eventArgs', args).modal('hide');
    };

    instance.setTitle = function (id, value) {
        var $modal = $getModal(id);
        if ($modal.length === 1)
            $modal.find('.modal-content .modal-header .modal-title').html(value);
    };

    instance.setBodyHtml = function (id, value) {
        var $modal = $getModal(id);
        if ($modal.length === 1) {
            var $body = $modal.find('.modal-content .modal-body');
            if (value)
                $body.html(value);
            else
                $body.empty();
        }
    };

    instance.setModalTitle = function (value) {
        if (isInFrame())
            window.parent.postMessage('insite.modal:' + JSON.stringify({ cmd: 'set-title', title: value }), '*');
    };

    instance.updateModalHeight = function () {
        if (isInFrame()) {
            var $body = $('body');
            window.parent.postMessage('insite.modal:' + JSON.stringify({ cmd: 'set-size', height: $body.prop('scrollHeight') }), '*');
        }
    };

    instance.setModalWidth = function (width) {
        if (typeof width !== 'number' || width < 0)
            return;

        if (isInFrame()) {
            window.parent.postMessage('insite.modal:' + JSON.stringify({ cmd: 'set-size', width: width }), '*');
        }
    };

    // methods

    function $getModal(id) {
        var idType = typeof id;

        if (idType === 'undefined')
            return $();

        if (typeof id === 'string' && id.length > 0)
            return $('div#' + id + '.insite-modal.modal');

        var $element = null;

        if (id !== null) {
            if (id instanceof HTMLElement)
                $element = $(id);
            else if (id instanceof jQuery)
                $element = id;
            else if (typeof id === 'object')
                return $(id);

            if (!$element.hasClass('insite-modal'))
                $element = $element.closest('div.insite-modal')
        }

        return $element === null ? $() : $element;
    }

    function setFrameSize($frame, height) {
        var minHeight = parseInt($frame.closest('.modal-body').css('min-height'));
        if (isNaN(minHeight))
            minHeight = 150;

        if (height < minHeight)
            height = minHeight;

        $frame.height(height);
    }

    function isInFrame() {
        try {
            return window.self !== window.top;
        } catch (e) {
            return true;
        }
    }

    function setModalState($modal, state) {
        var modalClass = $modal.attr('class');

        if (typeof modalClass == 'string' && modalClass.length > 0) {
            var values = modalClass.split(' ');

            modalClass = '';

            for (var i = 0; i < values.length; i++) {
                var value = values[i];
                if (value.length != 0 && !value.startsWith('wms-'))
                    modalClass += ' ' + value;
            }

            modalClass = modalClass.substring(1);
        }

        $modal.attr('class', modalClass + ' ' + 'wms-' + String(state));
    }

    // event handlers

    function onFrameLoaded() {
        setTimeout(function ($frame) {
            var $body = null;

            try {
                $body = $frame.contents().find('body');
            } catch (ex) {

            }

            if ($body != null) {
                $frame.height('');
                $body.css('overflow', 'hidden');
                setFrameSize($frame, $body.prop('scrollHeight') + 18);
                $body.css('overflow', '');

                $body.find('form').on('submit', { $modal: $modal }, function (e) {
                    e.data.$modal.addClass('loading');
                });
            }

            var $modal = $frame
                .closest('div.modal.insite-modal')
                .removeClass('loading')
                .modal('handleUpdate');
            $modal.trigger('loaded.modal.insite', [$modal[0], null]);
        }, 100, $(this));
    }

    function onModalOpening() {
        var $modal = $(this);

        setModalState($modal, 'opening');
    }

    function onModalOpened() {
        var $modal = $(this);

        setModalState($modal, 'opened');
    }

    function onModalClosing() {
        var $modal = $(this);

        setModalState($modal, 'closing');

        var args = $modal.data('eventArgs');
        if (typeof args === 'undefined')
            args = null;

        $modal.trigger('closing.modal.insite', [this, args]);
    }

    function onModalClosed() {
        var $modal = $(this);

        setModalState($modal, 'closed');

        if ($modal.data('content') === 'frame') {
            $modal.data('content', null);
            $modal.find(bodySelector).empty();
        }

        var args = $modal.data('eventArgs');
        if (typeof args === 'undefined')
            args = null;
        else
            $modal.data('eventArgs', null)

        $modal.trigger('closed.modal.insite', [this, args]);
    }

    function onWindowMessage(e) {
        var eventData = String(e.originalEvent.data);
        if (!eventData.startsWith('insite.modal:'))
            return;

        var $modal = $('.modal.insite-modal.wms-opening,.modal.insite-modal.wms-opened');
        if ($modal.length !== 1)
            return;

        var eventData = JSON.parse(eventData.substring(13));

        if (eventData.cmd === 'close')
            instance.close($modal, eventData.args);
        else if (eventData.cmd === 'set-title')
            instance.setTitle($modal, eventData.title);
        else if (eventData.cmd === 'set-size') {
            if (typeof eventData.height === 'number') {
                var $frame = $modal.find('iframe');
                if ($frame.length === 1)
                    setFrameSize($frame, eventData.height);
            }

            if (typeof eventData.width === 'number') {
                $modal.find('.modal-dialog').width(eventData.width);
            }
        }
    }
})($);