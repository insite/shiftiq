(function () {
    var instance;
    {
        var inSite = window.inSite = window.inSite || {};
        var common = inSite.common = inSite.common || {};
        instance = common.validators = common.validators || {};
    }

    var isTranslationExist = false;

    $(document).ready(function () {
        isTranslationExist = inSite.common.getObjByName('inSite.common.editorTranslation') !== null;
    });

    instance.fileExtensionValidator = (function () {
        var result = {};

        result.validate = function (source, argument) {
            if (typeof argument.Value != 'string' || argument.Value == null || argument.Value.length == 0)
                return;

            if (typeof source.fileExtensions != 'string' || source.fileExtensions == null || source.fileExtensions.length == 0)
                return;

            argument.IsValid = false;

            var ext = instance.fileExtensionValidator.getFileExtension(argument.Value);
            if (ext == null)
                return;

            var validateExtensionsPattern = new RegExp(source.fileExtensions, 'i');
            argument.IsValid = validateExtensionsPattern.test(ext);
        };

        result.getFileExtension = function (value) {
            var result = /.+(\..+)$/g.exec(value);
            if (result == null || result.length == 0)
                return null;

            return result[result.length - 1];
        };

        return result;
    })();

    instance.requiredValidator = (function () {
        var result = {};

        result.validate = function (val) {
            var value = '';

            if (typeof val.controltovalidate == 'string' && val.controltovalidate.length > 0) {
                var control = document.getElementById(val.controltovalidate);
                if (control != null) {
                    var $control = $(control);
                    if ($control.hasClass('datetimeoffset')) {
                        value = $control.find('> .datepickerinput').val();
                    } else if ($control.hasClass('insite-input-audio')) {
                        value = control.inputAudio?.fileName;
                        if (!value)
                            value = '';
                    } else if ($control.hasClass('find-entity')) {
                        value = '';

                        var json = $control.find('> input[type="hidden"]:first').val();
                        if (typeof json === 'string' && json.length > 0) {
                            var obj = JSON.parse(json);
                            if (obj instanceof Array && obj.length > 0) {
                                var fVal = String(obj[0]);
                                if (fVal !== '00000000-0000-0000-0000-000000000000')
                                    value = fVal;
                            }
                        }
                    } else if (isTranslationExist && inSite.common.editorTranslation.exists(control.id)) {
                        value = inSite.common.editorTranslation.getDefaultText(control.id);
                        if (!value)
                            value = '';
                    } else {
                        value = ValidatorGetValue(val.controltovalidate);
                    }
                }
            }

            var initialValue = val.initialvalue;
            if (!initialValue)
                initialValue = '';

            return ValidatorTrim(value) != ValidatorTrim(initialValue);
        };

        return result;
    })();

    instance.dateTimeCompareValidator = (function () {
        var result = {};

        result.validate = function (val) {
            var validateDate = getControlDate(val.controltovalidate)
            if (validateDate === null)
                return true;

            var compareDate = null;
            if (val.controltocompare) {
                compareDate = getControlDate(val.controltocompare)
            } else if (val.valuetocompare) {
                compareDate = moment(val.valuetocompare);
            }

            if (!compareDate || !compareDate.isValid())
                return true;

            switch (val.operator) {
                case "NotEqual":
                    return !validateDate.isSame(compareDate);
                case "GreaterThan":
                    return validateDate.isAfter(compareDate);
                case "GreaterThanEqual":
                    return validateDate.isSame(compareDate) || validateDate.isAfter(compareDate);
                case "LessThan":
                    return validateDate.isBefore(compareDate);
                case "LessThanEqual":
                    return validateDate.isSame(compareDate) || validateDate.isBefore(compareDate);
                default:
                    return validateDate.isSame(compareDate);
            }

            function getControlDate(id) {
                if (typeof id !== 'string' || id.length == 0)
                    return null;

                var $el = $('#' + id);
                if ($el.length !== 1)
                    return null;

                var dtPicker = $el.data('DateTimePicker');
                if (!dtPicker)
                    return null;

                return dtPicker.date();
            }
        };

        return result;
    })();
})();


