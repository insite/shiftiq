(function () {
    var instance = window.uiValidation = window.uiValidation || {};
    var customSetupFn = null;

    instance.isPageValidation = false;

    instance.setCustomSetup = function (fn) {
        if (typeof fn === 'function')
            customSetupFn = fn;
    };

    $(document).ready(function () {
        if (typeof (Page_ClientValidate) !== 'function')
            return;

        var __Page_ClientValidate = Page_ClientValidate;
        var __ValidatorOnChange = ValidatorOnChange;
        var __ValidatorUpdateDisplay = ValidatorUpdateDisplay;
        var __ValidationSummaryOnSubmit = ValidationSummaryOnSubmit;

        var validationStates = null;

        Page_ClientValidate = function () {
            instance.isPageValidation = true;

            onBeforeValidation();

            const result = __Page_ClientValidate.apply(this, arguments);

            onAfterValidation();

            instance.isPageValidation = false;

            return result;
        };

        ValidatorOnChange = function () {
            onBeforeValidation();

            __ValidatorOnChange.apply(this, arguments);

            onAfterValidation();
        };

        ValidatorUpdateDisplay = function (val) {
            __ValidatorUpdateDisplay.apply(this, arguments);

            if (!validationStates || !val.controltovalidate) {
                return;
            }

            if (!val.isvalid || validationStates[val.controltovalidate] !== false)
                validationStates[val.controltovalidate] = val.isvalid;
        };

        ValidationSummaryOnSubmit = function () {
            var __scrollTo = window.scrollTo;
            window.scrollTo = function () { return true; };
            var result = __ValidationSummaryOnSubmit.apply(this, arguments);
            window.scrollTo = __scrollTo;

            if (typeof Page_ValidationSummaries != 'undefined') {
                for (i = 0; i < Page_ValidationSummaries.length; i++) {
                    var summary = Page_ValidationSummaries[i];
                    if (summary.getAttribute('data-alert') != 1)
                        continue;

                    var $alert = $(summary).closest('.alert');

                    if (summary.style.display == 'none')
                        $alert.addClass('d-none');
                    else
                        $alert.removeClass('d-none');
                }
            }

            return result;
        };

        function onBeforeValidation() {
            validationStates = {};
        }

        function onAfterValidation() {
            if (customSetupFn !== null) {
                var data = [];

                for (var id in validationStates) {
                    if (!validationStates.hasOwnProperty(id))
                        continue;

                    data.push({
                        $el: $('#' + id),
                        isValid: validationStates[id]
                    });
                }

                customSetupFn(data);
            } else {
                for (let id in validationStates) {
                    if (!validationStates.hasOwnProperty(id))
                        continue;

                    let element = document.getElementById(id);
                    if (!element)
                        continue;

                    if (element.tagName == 'SELECT') {
                        if (element.parentNode.classList.contains('insite-combobox'))
                            element = element.parentNode;
                        else if (!element.classList.contains('form-select'))
                            continue;
                    } else if (element.tagName == 'INPUT') {
                        if (element.type == 'hidden') {
                            if (element.parentNode.classList.contains('insite-findentity'))
                                element = element.parentNode;
                            else
                                continue;
                        } else if (element.type == 'text' && element.classList.contains('flatpickr-input') && element.parentNode.classList.contains('datetimeoffset')) {
                            element = element.parentNode;
                        } else if (!element.classList.contains('form-control'))
                            continue;
                    }

                    if (validationStates[id])
                        element.classList.remove('is-invalid');
                    else
                        element.classList.add('is-invalid');
                }
            }
        }
    });
})();