(function () {
    var instance = inSite.common.multiField = inSite.common.multiField || {};

    instance.init = function (settings) {
        var $el = document.getElementById(settings.id);
        if (!$el)
            return;

        $el = $($el);
        if ($el.data('multiField'))
            return;

        var data = {
            $state: $('<input type="hidden">').attr('name', settings.name + '$index').appendTo($el),
            $views: $el.find('> div').each(function () {
                var $view = $(this);

                $view.hide();

                var inputs = $view.data('inputs');
                if (inputs) {
                    $view.removeAttr('data-inputs');

                    inputs = inputs.split(',');

                    for (var i = 0; i < inputs.length; i++) {
                        var el = document.getElementById(inputs[i]);
                        if (el)
                            inputs[i] = el;
                        else
                            inputs.splice(i--, 1);
                    }

                    if (inputs.length > 0)
                        $view.data('inputs', inputs);
                    else
                        inputs = null;
                }

                if (settings.validators !== false && typeof Page_Validators != 'undefined' && inputs) {
                    var validators = [];

                    for (var x = 0; x < inputs.length; x++) {
                        var id = inputs[x].id;
                        if (!id)
                            continue;

                        for (var y = 0; y < Page_Validators.length; y++) {
                            var val = Page_Validators[y];
                            if (val.controltovalidate == id)
                                validators.push(val);
                        }
                    }

                    if (validators.length > 0)
                        $view.data('validators', validators);
                }

                enableValidators($view, false);
            })
        };

        $el.data('multiField', data);

        {
            var el = $el[0];
            el.nextView = function () {
                instance.nextView(this);
            };
            el.prevView = function () {
                instance.prevView(this);
            };
            el.setView = function (index) {
                instance.setView(this, index);
            };
        }

        setView(data, settings.index);
    };

    instance.nextView = function (el) {
        var data = findData(el);
        if (data)
            setView(data, data.index + 1);
    };

    instance.prevView = function (el) {
        var data = findData(el);
        if (data)
            setView(data, data.index - 1);
    };

    instance.setView = function (el, index) {
        if (typeof index != 'number')
            return;

        var data = findData(el);
        if (data)
            setView(data, index);
    };

    function findData(el) {
        if (el)
            return $(el).closest('.multi-field').data('multiField');
    }

    function setView(data, index) {
        if (data.$views.length == 0)
            return;

        if (typeof index != 'number')
            index = 0;

        if (index < 0)
            index = data.$views.length - 1;

        if (index >= data.$views.length)
            index = 0;

        if (data.$active)
            setViewVisibility(data.$active, false);

        data.index = index;
        data.$active = data.$views.eq(data.index);

        setViewVisibility(data.$active, true);

        data.$state.val(data.index);
    }

    function setViewVisibility($view, visible) {
        if (visible)
            $view.show();
         else 
            $view.hide();

        enableValidators($view, visible);
    }

    function enableValidators($view, enable) {
        var validators = $view.data('validators');
        if (!validators)
            return;

        for (var i = 0; i < validators.length; i++) {
            var v = validators[i];

            ValidatorEnable(v, enable);

            if (enable)
                v.style.visibility = 'hidden';

            setElementVisibility(v);
            setSiblingVisibility(v.previousSibling);
            setSiblingVisibility(v.nextSibling);

            function setElementVisibility(el) {
                var $el = $(el);
                if (enable)
                    $el.show();
                else
                    $el.hide();
            }

            function setSiblingVisibility(sibling) {
                if (sibling.tagName == 'SUP')
                    setElementVisibility(sibling);
            }
        }
    }
})();