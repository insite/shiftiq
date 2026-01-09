(function (window) {
    var ncsha = window.ncsha = window.ncsha || {};

    if (!String.prototype.endsWith) {
        String.prototype.endsWith = function (search, this_len) {
            if (this_len === undefined || this_len > this.length) {
                this_len = this.length;
            }
            return this.substring(this_len - search.length, this_len) === search;
        };
    }

    ncsha.web = (function () {
        var web = {};

        web.showStatus = function (category, title, message) {
            const $alert = $('<div class="alert alert-' + category + ' alert-dismissible fade show" role="alert">' +
                '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' + 
                '<strong>' + title + '</strong> ' + message +
                '</div>');
            
            $('#app-growl').append($alert);
            
            if (category === 'success')
                setTimeout(function($a) { $a.alert('close'); }, 5000, $alert);
        };
        return web;
    })();

    ncsha.validator = (function () {
        var validator = {};

        validator.onFormInit = function ($form) {
            $form.find($.fn.validator.Constructor.INPUT_SELECTOR).filter(function () {
                var errors = $(this).data('bs.validator.errors');
                return typeof errors !== 'undefined' && errors !== null && errors instanceof Array && errors.length > 0;
            }).each(function () {
            });
        };

        validator.onInvalid = function (e) {
        };

        validator.onValid = function (e) {
        };

        function setupTooltip($i, $t) {
            var errors = $i.data('bs.validator.errors');
            if (typeof errors === 'undefined' || errors == null || !(errors instanceof Array) || errors.length == 0)
                return;

            var message;

            if (errors.length > 1) {
                message = '<ul>';

                for (var i = 0; i <= errors.length; i++)
                    message += '<li>' + String(errors[i]) + '</li>';

                message += '</ul>';
            } else {
                message = String(errors[0]);
            }

            $t.tooltip({ html: true, title: String(message) });
            if ($t.is(':focus'))
                $t.tooltip('show');
        }

        function getValidationTooltipElement(element) {
            var $element = $(element);

            if ($element.prop('tagName') === 'SELECT') {
                var $toggle = $element.siblings('button.dropdown-toggle');
                if ($toggle.length > 0)
                    $element = $toggle;
            }

            return $element;
        }

        return validator;
    })();
})(window);